using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Yaft.Processor;

namespace Yaft.InvertedIndex
{
    public class BigramWrapper
    {
        private const string url = PreprocessClient.BaseUrl + "index_words?reset=true";
        public void IndexTokens(List<string> tokens)
        {
            var request = SerializeRequest(tokens);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(url, content).Result;
                response.EnsureSuccessStatusCode();


                //var responseJson = response.Content.ReadAsStringAsync().Result;
            }
        }

        private string SerializeRequest(List<string> tokens)
        {
            return "{\"words\": " + JsonConvert.SerializeObject(tokens) + "}";
        }
    }
}
