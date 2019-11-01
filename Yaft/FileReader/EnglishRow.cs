using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.FileReader
{
    public class EnglishRow
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public Document ToDocument()
        {
            return new Document()
            {
                KeyValues = new Dictionary<string, string>()
                {
                    { "Title", Title },
                    { "Text", Text }
                }
            };
        }
    }
}
