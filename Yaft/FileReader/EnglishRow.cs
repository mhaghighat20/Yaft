using System;
using System.Collections.Generic;

namespace Yaft.FileReader
{
    public class EnglishRow
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public string ID { get; set; }

        public byte Tag { get; set; }

        public Document ToDocument(int id)
        {
            var keyValues = new Dictionary<string, string>()
                {
                    { "Title", Title },
                    { "Text", Text },
                    { "Tag", Tag.ToString() }
                };

            return new Document(id, keyValues);
        }
    }
}
