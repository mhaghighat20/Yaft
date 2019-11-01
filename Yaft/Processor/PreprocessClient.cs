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
        //static readonly 
        private const string BaseUrl = "http://95.85.2.25:8000/api/v1/preprocess_documents/?lang=";
        private const string EnglishUrl = BaseUrl + "en";
        private const string PersianUrl = BaseUrl + "fa";


        public List<DocumentTokens> GetTokens(List<Document> input)
        {
            var req = Serialize(input);
            var content = new StringContent(req, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(PersianUrl  , content).Result;
#if !DEBUG
            response.EnsureSuccessStatusCode();
#endif

                var json = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<List<List<string>>>(json);

                var result = new List<DocumentTokens>();

                foreach (var tokenList in deserializedResponse)
                {
                    result.Add(new DocumentTokens(tokenList));
                }

                return result;
            }

            //using (var request = new HttpRequestMessage(HttpMethod.Post, EnglishUrl))
            //{
            //    request.Headers.Accept.Clear();
            //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    request.Content = input.ToString();
            //    HttpResponseMessage response = SendRequest(request);

            //    CheckResponse(response);


            //}
        }

        private string Serialize(List<Document> input)
        {
            return "{\"documents\": " + JsonConvert.SerializeObject(input.Take(2).Select(x => x.GetText()).ToList()) + "}";
        }
    }
}
