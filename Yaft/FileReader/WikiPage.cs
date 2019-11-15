using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Yaft.FileReader
{
    public class WikiPage
    {
        public string Id;

        public string Title;

        public string Text;

        public WikiPage(XmlNode xml, XmlNamespaceManager nsManager)
        {
            Id = xml.SelectSingleNode("ns:id", nsManager).InnerText;

            Title = xml.SelectSingleNode("ns:title", nsManager).InnerText;

            Text = xml.SelectSingleNode("ns:revision", nsManager)
                .SelectSingleNode("ns:text", nsManager).InnerText;
        }

        public Document ToDocument(int id)
        {
            var keyValues = new Dictionary<string, string>()
                {
                    { "Title", Title },
                    { "Text", Text }
                };

            return new Document(id, keyValues);
        }
    }
}
