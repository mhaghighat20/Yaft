using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Yaft.Processor;

namespace Yaft.Storage
{
    public class CompressUtility
    {
        private const string CompressUrl = PreprocessClient.BaseUrl + "compress?type=";
        private const string DecompressUrl = PreprocessClient.BaseUrl + "decompress?type=";

        public static CompressMode Mode = CompressMode.gamma;

        public void CompressIntList(List<RawDataMapping> mapping)
        {
            var i = 0;

            foreach (var item in mapping)
                item.Id = i++;

            Dictionary<int, RawDataMapping> mappingDic = mapping.ToDictionary(x => x.Id);

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
            }
        }

        public void DecompressList(List<RawDataMapping> mapping)
        {
            var i = 0;

            foreach (var item in mapping)
                item.Id = i++;

            Dictionary<int, RawDataMapping> mappingDic = mapping.ToDictionary(x => x.Id);

            var request = SerializeDecompressRequest(mapping);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(DecompressUrl + Mode, content).Result;
#if !DEBUG
                response.EnsureSuccessStatusCode();
#endif


                var responseJson = response.Content.ReadAsStringAsync().Result;
                var deserializedResponse = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(responseJson);

                var result = new List<DocumentTokens>();

                foreach (var decompressedList in deserializedResponse)
                {
                    var id = Convert.ToInt32(decompressedList.Key);
                    mappingDic[id].RawData = decompressedList.Value;
                }
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

            return "{\"compressed_values\": " + JsonConvert.SerializeObject(request) + "}";
        }
    }

    public class RawDataMapping
    {
        public List<int> RawData { get; set; }
        public string CompressedData { get; set; }
        public int Id { get; set; }

        public string Token { get; set; }

        public RawDataMapping(string token, List<int> rawData)
        {
            RawData = rawData;
            Token = token;
        }

        public RawDataMapping(string token, string compressedData)
        {
            CompressedData = compressedData;
            Token = token;
        }
    }


    public enum CompressMode
    {
        gamma,
        varbyte
    }
}
