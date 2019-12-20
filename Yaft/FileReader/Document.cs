using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Yaft.FileReader
{
    public class Document
    {
        public int Id;
        private Dictionary<string, string> KeyValues;

        public string Text => KeyValues["Text"];

        public string Title => KeyValues["Title"];

        public byte Tag => Convert.ToByte(KeyValues["Tag"]);

        public Document(int id, Dictionary<string, string> keyValues)
        {
            Id = id;
            KeyValues = keyValues;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(KeyValues);
        }
    }
}