using Newtonsoft.Json;
using System.Collections.Generic;

namespace Yaft.FileReader
{
    public class Document
    {
        public string Id;
        public Dictionary<string, string> KeyValues;

        public Document(string id, Dictionary<string, string> keyValues)
        {
            Id = id;
            KeyValues = keyValues;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(KeyValues);
        }

        public string GetText()
        {
            return KeyValues["Text"];
        }
    }
}