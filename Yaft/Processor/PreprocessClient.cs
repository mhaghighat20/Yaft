using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Yaft.FileReader;

namespace Yaft.Processor
{
    public class PreprocessClient
    { 
        public const string BaseUrl = "http://82.196.6.128:8001/api/v1/";
        private const string EnglishUrl = BaseUrl + "preprocess_documents?lang=en";
        private const string PersianUrl = BaseUrl + "preprocess_documents?lang=fa";


        public List<DocumentTokens> GetTokens(List<Document> input, bool isEnglish)
        {
            var request = SerializeRequest(input);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var url = PersianUrl;

                if (isEnglish)
                    url = EnglishUrl;

                var response = client.PostAsync(url, content).Result;
#if !DEBUG
            response.EnsureSuccessStatusCode();
#endif

                var responseJson = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, string>>>(responseJson);

                var result = new List<DocumentTokens>();

                foreach (var docTokens in deserializedResponse)
                {
                    result.Add(new DocumentTokens(Convert.ToInt32(docTokens.Key), docTokens.Value));
                }

                return result;
            }
        }

        private string SerializeRequest(List<Document> input)
        {
            var request = new Dictionary<int, string>();

            foreach(var doc in input)
            {
                request.Add(doc.Id, doc.Text);
            }

            return "{\"documents\": " + JsonConvert.SerializeObject(request) + "}";
        }
    }
}
