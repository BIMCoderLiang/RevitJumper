
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace RevitJumper
{
    public class Query
    {
        private static readonly string apidocumenturl = "https://ac.cnstrc.com/autocomplete";
        private static readonly string apiforumurl = "https://forums.autodesk.com/t5/forums/forumpage.searchformv3.messagesearchfield.messagesearchfield:autocomplete";
        public List<SearchResult> GetSearchResult(string query, string engine)
        {
            var results = new List<SearchResult>();
            var geturl = string.Empty;
            if (engine.Equals(DescriptionAttributeUtility.GetDescriptionFromEnumValue(Engines.Revitapidocs)))
            {
                geturl = $"{apidocumenturl}/{query}?autocomplete_key=key_yyAC1mb0cTgZTwSo&c=ciojs-1.57.1&num_results=30&i=62b7b575-880d-48a0-98e7-90ef437be6c1&s=3&query={query}&_dt=1572328914509";
                try
                {
                    var httpresult = HttpControl.HttpClient.GetAsync(new Uri(geturl)).Result;
                    var result = httpresult.Content.ReadAsStringAsync().Result;
                    JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                    var sections = jo["sections"].ToString();
                    JObject products = (JObject)JsonConvert.DeserializeObject(sections);
                    var productsinfo = products["Products"].ToString();
                    var array = JArray.Parse(productsinfo);
                    if (array != null)
                    {
                        foreach (var info in array)
                        {
                            var relatedkey = info["value"].ToString();
                            var data = info["data"].ToString();
                            JObject datas = (JObject)JsonConvert.DeserializeObject(data);
                            var description = datas["description"].ToString();
                            var url = datas["url"].ToString();

                            var model = new SearchResult()
                            {
                                RelatedKey = relatedkey,
                                Description = description,
                                Url = url,
                            };
                            results.Add(model);
                        }
                    }
                }
                catch
                {

                }
            }
            else if (engine.Equals(DescriptionAttributeUtility.GetDescriptionFromEnumValue(Engines.RevitAPIForum)))
            {
                geturl = $"{apiforumurl}?t:ac=board-id/160&t:cp=search/contributions/page&q={query}&searchContext=160%7Cforum-board";
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var httpresult = HttpControl.HttpClient.GetAsync(new Uri(geturl)).Result;           
                    var result = httpresult.Content.ReadAsStringAsync().Result;
                    var array = JArray.Parse(result);
                    if (array != null)
                    {
                        foreach (var info in array)
                        {
                            var relatedkey = info["result"].ToString();
                            var data = info["value"].ToString();
                            JObject datas = (JObject)JsonConvert.DeserializeObject(data);
                            var url = datas["url"].ToString();

                            var model = new SearchResult()
                            {
                                RelatedKey = relatedkey,
                                Description = string.Empty,
                                Url = url,
                            };
                            results.Add(model);
                        }
                    }
                }
                catch(Exception ex)
                {

                }
            }
            return results;
        }

        public void GoSearch(string engine, string version ,string url)
        {
            var finalurl = string.Empty;
            if (engine.Equals(DescriptionAttributeUtility.GetDescriptionFromEnumValue(Engines.Revitapidocs)))
            {
                var revitdocs = "https://www.revitapidocs.com";
                finalurl = $"{revitdocs}/{version}/{url}";
            }
            else if(engine.Equals(DescriptionAttributeUtility.GetDescriptionFromEnumValue(Engines.RevitAPIForum)))
            {
                finalurl = url;
            }
            if (!string.IsNullOrEmpty(finalurl) && !string.IsNullOrWhiteSpace(finalurl))
            {
                System.Diagnostics.Process.Start(finalurl);
            }           
        }
    }

    public class SearchResult
    {
        public string RelatedKey { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
