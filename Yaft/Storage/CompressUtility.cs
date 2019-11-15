using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Yaft.Processor;

namespace Yaft.Storage
{
    class CompressUtility
    {
        private const string CompressUrl = PreprocessClient.BaseUrl + "compress?type=";
        private const string DecompressUrl = PreprocessClient.BaseUrl + "decompress?type=";

        private readonly CompressMode Mode = CompressMode.gamma;

        public List<string> CompressIntList(List<List<int>> rawDataBulk)
        {
            var i = 0;
            var mapping = rawDataBulk.Select(x => new RawDataMapping(x, i++)).ToList();
            var mappingDic = mapping.ToDictionary(x => x.Id);

            var request = SerializeCompressRequest(mapping);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(CompressUrl + Mode, content).Result;
#if !DEBUG
                response.EnsureSuccessStatusCode();
#endif


                var responseJson = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson);

                var result = new List<DocumentTokens>();

                foreach (var compressedList in deserializedResponse)
                {
                    var id = Convert.ToInt32(compressedList.Key);
                    mappingDic[id].CompressedData = compressedList.Value;
                }

                return mapping.Select(x => x.CompressedData).ToList();
            }
        }

        public List<List<int>> DecompressList(List<string> compressedData)
        {
            var i = 0;
            var mapping = compressedData.Select(x => new RawDataMapping(x, i++)).ToList();
            var mappingDic = mapping.ToDictionary(x => x.Id);

            var request = SerializeDecompressRequest(mapping);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(CompressUrl + Mode, content).Result;
#if !DEBUG
                response.EnsureSuccessStatusCode();
#endif


                var responseJson = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson);

                var result = new List<DocumentTokens>();

                foreach (var compressedList in deserializedResponse)
                {
                    var id = Convert.ToInt32(compressedList.Key);
                    mappingDic[id].CompressedData = compressedList.Value;
                }

                return mapping.Select(x => x.RawData).ToList();
            }
        }

        private string SerializeCompressRequest(List<RawDataMapping> input)
        {
            var request = new Dictionary<string, List<int>>();

            foreach (var item in input)
            {
                request.Add(item.Id.ToString(), item.RawData);
            }

            return "{\"integer_lists\": " + JsonConvert.SerializeObject(request) + "}";
        }

        private string SerializeDecompressRequest(List<RawDataMapping> input)
        {
            var request = new Dictionary<string, string>();

            foreach(var item in input)
            {
                request.Add(item.Id.ToString(), item.CompressedData);
            }

            return "{\"string_list\": " + JsonConvert.SerializeObject(request) + "}";
        }
    }

    class RawDataMapping
    {
        public List<int> RawData { get; set; }
        public string CompressedData { get; set; }
        public int Id { get; set; }

        public RawDataMapping(List<int> rawData, int id)
        {
            RawData = rawData;
            Id = id;
        }

        public RawDataMapping(string compressedData, int id)
        {
            CompressedData = compressedData;
            Id = id;
        }
    }


    public enum CompressMode
    {
        gamma,
        varbyte
    }
}
