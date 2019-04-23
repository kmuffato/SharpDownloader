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


/// <summary>
/// Some of the methods used on the sources can be found on this link https://gist.github.com/abombss/2720757
/// Special thanks to abombss for making our life easier
/// </summary>

namespace SharpDownloader.Extensions
{
    /// <summary>
    /// Static class that provides cloning capabilities for <see cref="WebRequest"/>
    /// </summary>
    public static class WebRequestExtensions
    {
        /// <summary>
        /// Clones a <see cref="HttpWebRequest"/> with all details except the content stream
        /// </summary>
        /// <param name="originalRequest">The Original <see cref="HttpWebRequest"/> needed to be cloned</param>
        /// <param name="newUri">The new <see cref="Uri"/> that will be used to generate the new request for</param>
        /// <returns><see cref="HttpWebRequest"/></returns>
        public static HttpWebRequest CloneRequest(this HttpWebRequest originalRequest, Uri newUri)
        {
            return CloneHttpWebRequest(originalRequest, newUri);
        }
        /// <summary>
        /// Converts the <see cref="HttpWebRequest"/> to a <see cref="WebRequest"/>
        /// </summary>
        /// <param name="originalRequest">The original <see cref="WebRequest"/> to be cloned</param>
        /// <param name="newUri">The new <see cref="Uri"/> that will be used to generate the new request for</param>
        /// <returns><see cref="WebRequest"/></returns>
        public static WebRequest CloneRequest(this WebRequest originalRequest, Uri newUri)
        {
            if (originalRequest is HttpWebRequest httpWebRequest) return CloneHttpWebRequest(httpWebRequest, newUri);
            return CloneWebRequest(originalRequest, newUri);
        }

        /// <summary>
        /// Clones <see cref="WebRequest"/> Properties, <see cref="HttpWebRequest"/> properties, and <see cref="HttpHeaderCollection"/> for the <see cref="HttpWebRequest"/>
        /// </summary>
        /// <param name="old">The original <see cref="WebRequest"/></param>
        /// <param name="newUri">The new <see cref="Uri"/>, <see cref="WebRequest"/> to be cloned for</param>
        /// <returns><see cref="HttpWebRequest"/></returns>
        private static HttpWebRequest CloneHttpWebRequest(HttpWebRequest old, Uri newUri)
        {
            var @new = (HttpWebRequest)WebRequest.Create(newUri);
            CopyWebRequestProperties(old, @new);
            CopyHttpWebRequestProperties(old, @new);
            CopyHttpWebRequestHeaders(old, @new);
            return @new;
        }

        /// <summary>
        /// Clones <see cref="WebRequest"/> Properties and <see cref="WebHeaderCollection"/> for the <see cref="WebRequest"/>
        /// </summary>
        /// <param name="old">The original <see cref="WebRequest"/></param>
        /// <param name="newUri">The new <see cref="Uri"/>, <see cref="WebRequest"/> to be cloned for</param>
        /// <returns><see cref="WebRequest"/></returns>
        private static WebRequest CloneWebRequest(WebRequest old, Uri newUri)
        {
            var @new = WebRequest.Create(newUri);
            CopyWebRequestProperties(old, @new);
            CopyWebRequestHeaders(old, @new);
            return @new;
        }
        /// <summary>
        /// Copies properties from the original <see cref="WebRequest"/> to a new <see cref="WebRequest"/>
        /// </summary>
        /// <param name="old">"/>Origninal <see cref="WebRequest"/></param>
        /// <param name="new">New <see cref="WebRequest"/></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old"></param>
        /// <param name="new"></param>
        private static void CopyWebRequestHeaders(WebRequest old, WebRequest @new)
        {
            string[] allKeys = old.Headers.AllKeys;
            foreach (var key in allKeys)
            {
                @new.Headers[key] = old.Headers[key];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old"></param>
        /// <param name="new"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old"></param>
        /// <param name="new"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BaseUri"></param>
        /// <param name="EmptyHeaders"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BaseUri"></param>
        /// <param name="QueryParameters"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BaseUri"></param>
        /// <param name="SubDomain"></param>
        /// <param name="item"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
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

    
}
