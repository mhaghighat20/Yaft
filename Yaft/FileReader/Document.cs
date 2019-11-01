using Newtonsoft.Json;
using System.Collections.Generic;

namespace Yaft.FileReader
{
    public class Document
    {
        public string Id;
        public Dictionary<string, string> KeyValues;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(KeyValues);
        }
    }
}