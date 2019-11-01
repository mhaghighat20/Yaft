using System;
using System.Collections.Generic;

namespace Yaft.FileReader
{
    public class EnglishRow
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public Document ToDocument()
        {
            var id = Guid.NewGuid().ToString();

            var keyValues = new Dictionary<string, string>()
                {
                    { "Title", Title },
                    { "Text", Text }
                };

            return new Document(id, keyValues);
        }
    }
}
