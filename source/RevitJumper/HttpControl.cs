using System;
using System.Net.Http;

namespace RevitJumper
{
    public class HttpControl
    {
        private static readonly string documenturl = "https://ac.cnstrc.com/autocomplete";
        public static HttpClient HttpClient = null;

        static HttpControl()
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(documenturl)
            };
        }
    }
}
