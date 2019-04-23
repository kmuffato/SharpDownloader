using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpDownloader
{
    public class DownloaderWebRequest
    {

        public WebRequest WebRequest { get; set; }

        public Dictionary<string, string> FormData { get; set; }

        public string GetFormDataCollection { get { return BuildFormData(); } }

        public bool UsingParams { get; set; }

        public string CurrentValue { get; set; }

        public DownloaderWebRequest(WebRequest webRequest, Dictionary<string, string> formdata)
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
