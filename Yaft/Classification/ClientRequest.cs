using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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

        [JsonProperty(PropertyName = "class", NullValueHandling = NullValueHandling.Ignore)]
        byte? Tag { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        int? Id { get; set; }

        public VectorWithTag(TfIdfVector vector, byte? tag, int? id)
        {
            Vector = vector.IntVector.ToDictionary(kvp => kvp.Key, kvp => (float)kvp.Value); ;
            Tag = tag;
            Id = id;
        }
    }
}
