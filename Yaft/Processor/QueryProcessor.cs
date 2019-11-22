using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.Processor
{
    public class QueryProcessor
    {
        private string RawQuery { get; set; }

        public List<string> Tokens { get; private set; }

        public QueryProcessor(string rawQuery)
        {
            RawQuery = rawQuery;
        }

        public void Preprocess()
        {
            var request = SerializeRequest(RawQuery);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(url, content).Result;
                response.EnsureSuccessStatusCode();


                var responseJson = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<List<string>>(responseJson);
                Tokens = deserializedResponse;
            }
        }

        private string SerializeRequest(string rawQuery)
        {
            return "{\"query\": " + JsonConvert.SerializeObject(rawQuery) + "}";
        }

        private const string url = PreprocessClient.BaseUrl + "preprocess_query";
    }
}
