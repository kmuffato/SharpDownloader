using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDownloader;
using SharpDownloader.Extensions;
namespace SharpCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            SharpDownloaderManager downloaders = new SharpDownloaderManager(new SharpDownloaderSettings());

            downloaders.Add(new Down("https:// .... link", false));

            downloaders.StartDownloading();

        }
    }

    internal class Down : Downloader
    {

        public Down(string baselink,bool testing):base(baselink)
        {
            this.OnBuildRequests += Down_OnBuildRequests;
            this.OnResponseReturned += Down_OnResponseReturned;
            this.OnProxyChange += Down_OnProxyChange;
        }

        

        private Task<string> Down_OnProxyChange()
        {
            return Task.Run(() => {


                return "";
            });
        }
        int down = 0;
        private Task Down_OnResponseReturned(string _baseLink, string content)
        {
            return Task.Run(() 
                => 
            {

                string a = ""+_baseLink;
                string b = content;
                Interlocked.Increment(ref down);
                //Console.WriteLine(down);

            });
        }

        private Task<int> Down_OnBuildRequests()
        {
            return Task.Run(() => { 
                foreach (string item1 in System.IO.File.ReadAllLines(""))
                {

                    string[] item = item1.Split('|');
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(item[0]);
                    request.Timeout = 20 * 1000 * 60 * 60;
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0";
                    request.Accept = "text/html";
                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                    request.KeepAlive = true;
                    request.Headers.Add(HttpRequestHeader.Cookie, "CFID=1691589; CFTOKEN=38950381; PHPSESSID=55fd6ab2-5f81-11e9-8000-001e679e2c6d-en-MTI3LjAuMC4x0AFC0AFB0AFA");
                    request.Headers.Add("Upgrade-Insecure-Requests: 1");
                    request.Referer = this.BaseLink.ToString() ;
                    WebRequests.Add(new DWebRequest(request, null));
                    ///////
                }

                return 1;

            });

        }
    }

}
