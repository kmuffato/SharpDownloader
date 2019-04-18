using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDownloader.Extensions;

namespace SharpDownloader
{
    public enum StatusEnum { Idle = 0, Running = 1, ChangeProxy = 2, Finished = 3 }
    public class Downloader
    {
        public static readonly string[] ReportingHeader = { "Base Link", "Total Count", "Current Link", "Total Downloaded", "Status" };

        public string[] ReportValues => new string[] { BaseLink.ToString(), Total_Count.ToString(), CurrentID?.ToString(), Total_Downloaded.ToString(), Status };

        public delegate Task<int> BuildRequests();
        public event BuildRequests OnBuildRequests;

        public delegate Task<int> ReleaseResources();
        public event ReleaseResources OnReleaseResources;

        public delegate Task<string> ProxyChange();
        public event ProxyChange OnProxyChange;

        public delegate Task ResponseReturend(string _baseLink, string content);
        public event ResponseReturend OnResponseReturned;

        public SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public BlockingCollection<DWebRequest> WebRequests = new BlockingCollection<DWebRequest>();

        protected BlockingCollection<string> SavedLinks = new BlockingCollection<string>();

        #region Reporting
        public Uri BaseLink { get; set; }
        public int Total_Count { get; set; }
        public string CurrentID = "";

        public int Total_Downloaded { get { return total_downloaded; } }

        public int total_downloaded;
        public string Status { get; set; }
        #endregion

        #region Synch
        public Object _LockObject = new object();
        #endregion

        public int NumberOfProcessors { get; set; }

        public string CookieContainer { get; set; }
        public bool Testing { get; set; }

        public Downloader(string _BaseLink, bool Testing = false, int _NumberOfProcessors = 0, bool Expect100 = false)
        {

            BaseLink = new Uri(_BaseLink);

            if (_NumberOfProcessors == 0)
            {
                NumberOfProcessors = Environment.ProcessorCount;
            }
            else
            {
                NumberOfProcessors = _NumberOfProcessors;
            }

            this.Testing = Testing;
            Status = StatusEnum.Idle.ToString();
            if (Expect100)
            {
                SetExpect100();
            }

        }

        private void SetExpect100()
        {
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;
        }

        public async Task StartDownloadAsync()
        {



            //await GetCookies();
            Console.WriteLine($"Starting WebRequest Build Process, Current Count {0}");
            Total_Count = await OnBuildRequests();
            Console.WriteLine($"WebRequest Build Process Finished, Current Count {WebRequests.Count}");
            if (!Testing)
            {
                await ProcessLinksAsync();
            }
            else
            {
                await ProcessLinksTesting();
            }

            await OnReleaseResources();

        }


        public async Task ProcessLinksAsync()
        {

            if (WebRequests.Count == 0)
            {
                Console.WriteLine("No WebRequests Provided");
                return;
            }

            List<Task> RunningTasks = new List<Task>();
            for (int i = 0; i < NumberOfProcessors; i++)
            {
                var task = new Task(() => ProcessRequestAsync(), creationOptions: TaskCreationOptions.LongRunning);
                task.Start();
                RunningTasks.Add(task);
            }
            Status = StatusEnum.Running.ToString();
            await Task.WhenAll(RunningTasks.ToArray());
            Status = StatusEnum.Finished.ToString();
        }

        public async Task ProcessLinksTesting()
        {

            if (WebRequests.Count == 0)
            {
                Console.WriteLine("No WebRequests Provided");
                return;
            }
            Status = StatusEnum.Running.ToString();
            List<Task> RunningTasks = new List<Task>();
            for (int i = 0; i < 1; i++)
            {

                RunningTasks.Add(Task.Run(() => ProcessRequestAsync()));
            }
            await Task.WhenAll(RunningTasks.ToArray());
            Status = StatusEnum.Finished.ToString();
        }

        public void ProcessRequestAsync()
        {
            try
            {
                String Proxy = "";
                while (!WebRequests.IsAddingCompleted)
                {
                    if (WebRequests.Count > 0)
                    {
                        var Request = WebRequests.Take();
                        if (Request != null)
                        {
                            Request.WebRequest.Proxy = Proxy == "" ? null : new WebProxy(Proxy);
                            //Process
                            int Result = -1;
                            try
                            {
                                Result = SendRequestAsync(Request, Proxy).Result;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            switch (Result)
                            {
                                //Terminate
                                case 0: { WebRequests.CompleteAdding(); } break;
                                //Mire
                                case 1: { continue; }
                                //NdrroProxy
                                case 2:
                                    {
                                        semaphore.Wait();
                                        if (OnProxyChange != null)
                                        {
                                            Proxy = OnProxyChange().Result;
                                            if (!WebRequests.IsAddingCompleted)
                                            {
                                                WebRequests.Add(new DWebRequest(Request.WebRequest.CloneRequest(Request.WebRequest.RequestUri), Request.FormData));
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Request {Request.WebRequest.RequestUri.ToString() } Failed with Status 403");
                                            Console.WriteLine($"Proxy Change event has not been set, failed requests will not be retried..");
                                        }
                                        semaphore.Release();
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        WebRequests.CompleteAdding();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message + BaseLink);
            }
        }

        public Task<int> SendRequestAsync(DWebRequest request, string Proxy = "")
        {
            return Task.Run(async () =>
            {
                HttpWebResponse response = null;
                var responseString = String.Empty;
                try
                {
                    CurrentID = request.CurrentValue?.ToString();
                    CurrentID = CurrentID ?? request.WebRequest.RequestUri.ToString();
                    switch (request.WebRequest.Method)
                    {
                        case WebRequestMethods.Http.Post:
                            {
                                if (request.FormData != null && !request.UsingParams)
                                {
                                    byte[] bytedata = Encoding.UTF8.GetBytes(request.GetFormDataCollection);
                                    request.WebRequest.ContentLength = bytedata.Length;

                                    using (Stream requestStream = request.WebRequest.GetRequestStream())
                                    {
                                        requestStream.Write(bytedata, 0, bytedata.Length);
                                    }
                                    using (response = (HttpWebResponse)request.WebRequest.GetResponse())
                                    {
                                        Stream dataStream = response.GetResponseStream();
                                        StreamReader reader = new StreamReader(dataStream);
                                        responseString = reader.ReadToEnd();
                                        await OnResponseReturned?.Invoke(request.WebRequest.RequestUri.ToString(), responseString);
                                        Interlocked.Increment(ref total_downloaded);

                                        reader.Close();
                                        dataStream.Close();
                                    }
                                }

                                if (request.FormData != null && request.UsingParams)
                                {

                                    var data = Encoding.UTF8.GetBytes(request.FormData.BuildFormDataFromDictionary());
                                    request.WebRequest.ContentLength = data.Length;
                                    string s = Encoding.UTF8.GetString(data);

                                    using (var stream = request.WebRequest.GetRequestStream())
                                    {
                                        stream.Write(data, 0, data.Length);
                                    }
                                    using (response = (HttpWebResponse)request.WebRequest.GetResponse())
                                    {
                                        Stream dataStream = response.GetResponseStream();
                                        StreamReader reader = new StreamReader(dataStream);
                                        responseString = reader.ReadToEnd();
                                        await OnResponseReturned?.Invoke(request.WebRequest.RequestUri.ToString(), responseString);
                                        Interlocked.Increment(ref total_downloaded);
                                        reader.Close();
                                        dataStream.Close();
                                    }
                                }
                            }
                            break;

                        case WebRequestMethods.Http.Get:
                            {
                                using (response = (HttpWebResponse)request.WebRequest.GetResponse())
                                {
                                    Stream dataStream = response.GetResponseStream();
                                    StreamReader reader = new StreamReader(dataStream);
                                    responseString = reader.ReadToEnd();
                                    //await RequestCompleted(ServiceLink, responseString);
                                    await OnResponseReturned?.Invoke(request.WebRequest.RequestUri.ToString(), responseString);
                                    Interlocked.Increment(ref total_downloaded);
                                    reader.Close();
                                    dataStream.Close();
                                }

                            }
                            break;
                    }
                    return 1;
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        response = (HttpWebResponse)ex.Response;
                        if (response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            return 2;
                        }
                    }
                    else if (ex.Status == WebExceptionStatus.KeepAliveFailure || ex.Status == WebExceptionStatus.Timeout ||
                    ex.Status == WebExceptionStatus.ConnectFailure)
                    {
                        return 2;
                    }
                    else
                    {
                        Console.Write("Error: {0} ", ex.Status + " " + BaseLink.Host);
                        await Task.Delay(3000);
                    }
                    return 2;
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            });



        }

        public async Task<string> GetCookies()
        {
            int RetryTimes = 5;
            string Proxy = "";
            HttpWebResponse response = null;
            Start:
            try
            {
                CookieContainer jar = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)WebRequestExtensions.BuildRequest(BaseLink);

                request.Proxy = Proxy == "" ? null : new WebProxy(Proxy);
                request.CookieContainer = jar;
                HttpWebResponse TheRespone = (HttpWebResponse)request.GetResponse();
                String setCookieHeader = TheRespone.Headers[HttpResponseHeader.SetCookie];
                var a = response.Cookies;
                var t = jar.GetCookies(new Uri(request.Host));
                return "";

            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)ex.Response;
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        RetryTimes--;
                        if (RetryTimes > 0)
                        {

                            try
                            {
                                Proxy = OnProxyChange().Result;
                            }
                            finally
                            {

                            }
                            goto Start;
                        }
                    }
                }
                else if (ex.Status == WebExceptionStatus.ConnectFailure)
                {
                    Console.WriteLine($"Provided Link is not active {BaseLink.Host}");
                }
                else
                {
                    Console.Write("Error: {0}", ex.Status + BaseLink.Host);
                }




            }



            return "";
        }

        



    }

}

