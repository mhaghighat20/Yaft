using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Classification
{
    public class ClientTrainRequest
    {
        [JsonProperty(PropertyName = "vectors")]
        public List<VectorWithTag> Vectors { get; set; }
    }

    public class VectorWithTag
    {
        [JsonProperty(PropertyName = "vector")]
        Dictionary<int, float> Vector { get; set; }

        [JsonProperty(PropertyName = "class")]
        byte? Tag { get; set; }

        [JsonProperty(PropertyName = "id")]
        int? Id { get; set; }

        public VectorWithTag(TfIdfVector vector, byte? tag, int? id)
        {
            Vector = vector.IntVector.ToDictionary(kvp => kvp.Key, kvp => (float)kvp.Value); ;
            Tag = tag;
            Id = id;
        }
    }
}
