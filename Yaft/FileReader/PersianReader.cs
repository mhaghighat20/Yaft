using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Yaft.FileReader
{
    public class PersianReader : IFileReader
    {
        private readonly string FilePath;

        public PersianReader(string filePath)
        {
            FilePath = filePath;
        }

        public List<Document> ReadFile()
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(FilePath);

            var nsManager = new XmlNamespaceManager(xmlFile.NameTable);
            nsManager.AddNamespace("ns", "http://www.mediawiki.org/xml/export-0.10/");

            //var nodes = xmlFile.SelectNodes("//a:Applications/a:Application", nsManager);
            XmlNodeList pages = xmlFile.ChildNodes[0].ChildNodes;

            var result = new List<Document>();

            foreach(XmlNode node in pages)
            {
                var wikiPage = new WikiPage(node, nsManager);

                result.Add(wikiPage.ToDocument());
            }

            return result;
        }
    }
}
