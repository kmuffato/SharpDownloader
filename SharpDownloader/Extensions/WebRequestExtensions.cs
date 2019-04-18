using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SharpDownloader.Extensions
{
    public static class WebRequestExtensions
    {
        public static HttpWebRequest CloneRequest(this HttpWebRequest originalRequest, Uri newUri)
        {
            return CloneHttpWebRequest(originalRequest, newUri);
        }

        public static WebRequest CloneRequest(this WebRequest originalRequest, Uri newUri)
        {

            var httpWebRequest = originalRequest as HttpWebRequest;
            if (httpWebRequest != null) return CloneHttpWebRequest(httpWebRequest, newUri);
            return CloneWebRequest(originalRequest, newUri);
        }

        private static HttpWebRequest CloneHttpWebRequest(HttpWebRequest old, Uri newUri)
        {
            var @new = (HttpWebRequest)WebRequest.Create(newUri);
            CopyWebRequestProperties(old, @new);
            CopyHttpWebRequestProperties(old, @new);
            CopyHttpWebRequestHeaders(old, @new);
            return @new;
        }

        private static WebRequest CloneWebRequest(WebRequest old, Uri newUri)
        {
            var @new = WebRequest.Create(newUri);
            CopyWebRequestProperties(old, @new);
            CopyWebRequestHeaders(old, @new);
            return @new;
        }

        private static void CopyWebRequestProperties(WebRequest old, WebRequest @new)
        {
            @new.AuthenticationLevel = old.AuthenticationLevel;
            @new.CachePolicy = old.CachePolicy;
            @new.ConnectionGroupName = old.ConnectionGroupName;
            @new.ContentType = old.ContentType;
            @new.Credentials = old.Credentials;
            @new.ImpersonationLevel = old.ImpersonationLevel;
            @new.Method = old.Method;
            @new.PreAuthenticate = old.PreAuthenticate;
            @new.Proxy = old.Proxy;
            @new.Timeout = old.Timeout;
            @new.UseDefaultCredentials = old.UseDefaultCredentials;

            if (old.ContentLength > 0) @new.ContentLength = old.ContentLength;
        }

        private static void CopyWebRequestHeaders(WebRequest old, WebRequest @new)
        {
            string[] allKeys = old.Headers.AllKeys;
            foreach (var key in allKeys)
            {
                @new.Headers[key] = old.Headers[key];
            }
        }

        private static void CopyHttpWebRequestProperties(HttpWebRequest old, HttpWebRequest @new)
        {
            @new.Accept = old.Accept;
            @new.AllowAutoRedirect = old.AllowAutoRedirect;
            @new.AllowWriteStreamBuffering = old.AllowWriteStreamBuffering;
            @new.AutomaticDecompression = old.AutomaticDecompression;
            @new.ClientCertificates = old.ClientCertificates;
            @new.SendChunked = old.SendChunked;
            @new.TransferEncoding = old.TransferEncoding;
            //@new.Connection = old.Connection;
            @new.ContentType = old.ContentType;
            @new.ContinueDelegate = old.ContinueDelegate;
            @new.CookieContainer = old.CookieContainer;
            @new.Date = old.Date;
            @new.Expect = old.Expect;
            @new.Host = old.Host;
            @new.IfModifiedSince = old.IfModifiedSince;
            @new.KeepAlive = old.KeepAlive;
            @new.MaximumAutomaticRedirections = old.MaximumAutomaticRedirections;
            @new.MaximumResponseHeadersLength = old.MaximumResponseHeadersLength;
            @new.MediaType = old.MediaType;
            @new.Pipelined = old.Pipelined;
            @new.ProtocolVersion = old.ProtocolVersion;
            @new.ReadWriteTimeout = old.ReadWriteTimeout;
            @new.Referer = old.Referer;
            @new.Timeout = old.Timeout;
            @new.UnsafeAuthenticatedConnectionSharing = old.UnsafeAuthenticatedConnectionSharing;
            @new.UserAgent = old.UserAgent;
        }

        private static void CopyHttpWebRequestHeaders(HttpWebRequest old, HttpWebRequest @new)
        {
            var allKeys = old.Headers.AllKeys;
            foreach (var key in allKeys)
            {
                switch (key.ToLower(CultureInfo.InvariantCulture))
                {
                    // Skip all these reserved headers because we have to set them through properties
                    case "accept":
                    case "connection":
                    case "content-length":
                    case "content-type":
                    case "date":
                    case "expect":
                    case "host":
                    case "if-modified-since":
                    case "range":
                    case "referer":
                    case "transfer-encoding":
                    case "user-agent":
                    case "proxy-connection":
                        break;
                    default:
                        @new.Headers[key] = old.Headers[key];
                        break;
                }
            }
        }

        public static WebRequest BuildRequest(Uri BaseUri, bool EmptyHeaders = false, String httpMethod = "GET")
        {
            HttpWebRequest @request = (HttpWebRequest)WebRequest.Create(BaseUri);
            @request.Method = httpMethod;
            @request.Host = BaseUri.Host;
            if (EmptyHeaders)
            {
                return request;
            }
            @request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0";
            @request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            @request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            @request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            @request.KeepAlive = true;
            return @request;
        }

        public static WebRequest BuildRequest(Uri BaseUri, NameValueCollection QueryParameters)
        {
            var parameters = QueryParameters.BuildFormData();

            HttpWebRequest @request = (HttpWebRequest)WebRequest.Create(BaseUri + "?" + parameters);
            @request.Method = HttpMethod.Get.ToString();
            @request.Host = BaseUri.Host;
            @request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0";
            @request.Accept = "text/html,application/xhtml+xml,application/xml,application/json;q=0.9,*/*;q=0.8";
            @request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            //@request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            @request.KeepAlive = true;
            return @request;
        }

        public static WebRequest BuildRequest(Uri BaseUri, string SubDomain, string item)
        {

            HttpWebRequest @request = (HttpWebRequest)WebRequest.Create(BaseUri.ToString().Replace(SubDomain, item));
            @request.Method = HttpMethod.Get.ToString();
            @request.Host = BaseUri.Host;
            @request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0";
            @request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            @request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            //@request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            @request.KeepAlive = true;
            return @request;
        }
        public static string BuildFormData(this NameValueCollection collection)
        {

            string query = "";
            if (collection != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    query += collection.AllKeys[i] + "=" + HttpUtility.UrlEncode(collection[i]);
                    if (i != collection.Count - 1)
                    {
                        query += "&";
                    }
                }
            }
            return query;
        }
        public static string BuildFormDataFromDictionary(this Dictionary<string, string> collection)
        {
            string itemBuild = "";
            foreach (KeyValuePair<string, string> item in collection)
            {
                itemBuild += item.Key + "=" + item.Value;
                itemBuild += "&";
            }

            if (itemBuild.EndsWith("&"))
                itemBuild = itemBuild.Substring(0, itemBuild.LastIndexOf("&"));
            return itemBuild;
        }
    }

    public class DWebRequest
    {

        public WebRequest WebRequest { get; set; }

        public Dictionary<string, string> FormData { get; set; }

        public string GetFormDataCollection { get { return BuildFormData(); } }

        public bool UsingParams { get; set; }

        public string CurrentValue { get; set; }

        public DWebRequest(WebRequest webRequest, Dictionary<string, string> formdata)
        {
            this.WebRequest = webRequest;
            this.FormData = formdata;
        }


        private string BuildFormData()
        {
            string query = "";
            if (FormData != null)
            {
                query = new FormUrlEncodedContent(FormData).ToString();
            }
            return query;
        }
    }
}
