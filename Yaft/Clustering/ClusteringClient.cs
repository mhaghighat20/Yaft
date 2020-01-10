using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yaft.Classification;
using Yaft.Processor;

namespace Yaft.Clustering
{
    class ClusteringClient : VectorClustering
    {
        private const string trainUrl = PreprocessClient.BaseUrl + "collect_data_set?reset=true";
        private string ClassifyUrl { get; } = PreprocessClient.BaseUrl + "classify?method=";
        private Mode Algorithm { get; }

        public ClusteringClient(Mode algorithm, float param)
        {
            Algorithm = algorithm;
            ClassifyUrl += Algorithm.ToString().ToLower() + "&param=" + param;
        }

        public void Train(List<TfIdfVector> data)
        {
            var request = SerializeTrainRequest(data);
            SendRequest(request, trainUrl);
        }

        public List<byte> Classify(List<TfIdfVector> vectorList)
        {
            var request = SerializeClassifyRequest(vectorList);
            var responseJson = SendRequest(request, ClassifyUrl);

            var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<int, int>>(responseJson);

            return deserializedResponse.OrderBy(x => x.Key).Select(x => (byte)x.Value).ToList();
        }

        private static string SendRequest(string request, string url)
        {
            var i = 0;

            while (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent("{\"vectors\": []}", Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;
                    // response.EnsureSuccessStatusCode();

                    if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Console.WriteLine("Waiting... " + i++);
                        Task.Delay(1000).Wait();
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        break;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Status Code: " + response.StatusCode);
                    }
                }
            }

            {
                var content = new StringContent(request, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient() { Timeout = new TimeSpan(0, 0, 1200) })
                {
                    var response = client.PostAsync(url, content).Result;
#if !DEBUG
                response.EnsureSuccessStatusCode();
#endif

                    var responseJson = response.Content.ReadAsStringAsync().Result;
                    return responseJson;
                }
            }
        }

        private string SerializeTrainRequest(List<TfIdfVector> input)
        {
            var request = new ClientTrainRequest()
            {
                Vectors = input.Select(x => new VectorWithTag(x, null, null)).ToList()
            };

            return JsonConvert.SerializeObject(request);
        }

        private string SerializeClassifyRequest(List<TfIdfVector> input)
        {
            var i = 0;
            var request = new ClientTrainRequest()
            {
                Vectors = input.Select(x => new VectorWithTag(x, null, i++)).ToList()
            };

            return JsonConvert.SerializeObject(request);
        }

    }

    public enum Mode
    {
        kmeans,
        gmm,
        hierarchical
    }
}
