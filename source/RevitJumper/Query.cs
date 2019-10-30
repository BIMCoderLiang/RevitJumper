
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace RevitJumper
{
    public class Query
    {
        private static readonly string documenturl ="https://ac.cnstrc.com/autocomplete";
        public JArray GetSearchResult(string query)
        {
            JArray array = null;
            var geturl = $"{documenturl}/{query}?autocomplete_key=key_yyAC1mb0cTgZTwSo&c=ciojs-1.57.1&num_results=30&i=62b7b575-880d-48a0-98e7-90ef437be6c1&s=3&query={query}&_dt=1572328914509";
            try
            {                
                var httpresult = HttpControl.HttpClient.GetAsync(new Uri(geturl)).Result;
                var result = httpresult.Content.ReadAsStringAsync().Result;
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                var sections = jo["sections"].ToString();
                JObject products = (JObject)JsonConvert.DeserializeObject(sections);
                var productsinfo = products["Products"].ToString();
                array = JArray.Parse(productsinfo);               
            }
            catch
            {
               
            }
            return array;
        }
    }
}
