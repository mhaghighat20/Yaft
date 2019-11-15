using Newtonsoft.Json;
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
        public const string BaseUrl = "http://95.85.2.25:8000/api/v1/";
        private const string EnglishUrl = BaseUrl + "preprocess_documents?lang=en";
        private const string PersianUrl = BaseUrl + "preprocess_documents?lang=fa";


        public List<DocumentTokens> GetTokens(List<Document> input)
        {
            var request = SerializeRequest(input);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(PersianUrl, content).Result;
#if !DEBUG
            response.EnsureSuccessStatusCode();
#endif

                var responseJson = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, string>>>(responseJson);

                var result = new List<DocumentTokens>();

                foreach (var docTokens in deserializedResponse)
                {
                    result.Add(new DocumentTokens(docTokens.Key, docTokens.Value));
                }

                return result;
            }
        }

        private string SerializeRequest(List<Document> input)
        {
            var request = new Dictionary<string, string>();

            foreach(var doc in input)
            {
                request.Add(doc.Id, doc.Text);
            }

            return "{\"documents\": " + JsonConvert.SerializeObject(request) + "}";
        }
    }
}
