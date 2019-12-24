using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.InvertedIndex;
using Yaft.Storage;

namespace Yaft.Processor
{
    public class VectorGenerator
    {
        PositionalIndex TitleIndex { get; set; }
        PositionalIndex ContentIndex { get; set; }

        Dictionary<int, DocumentWrapper> Documents { get; set; }

        public VectorGenerator(Dictionary<int, DocumentWrapper> documents)
        {
            Documents = documents;
        }

        public void Process()
        {
            TitleIndex = new PositionalIndex();
            ContentIndex = new PositionalIndex();

            var pureDocs = Documents.Values.Select(x => x.Document).ToList();

            var titlePreprocessClient = new PreprocessClient(true, true);
            var titleTokensBulk = titlePreprocessClient.GetTokens(pureDocs).ToDictionary(x => x.DocumentId);

            var contentPreprocessClient = new PreprocessClient(false, true);
            var contentTokensBulk = contentPreprocessClient.GetTokens(pureDocs).ToDictionary(x => x.DocumentId);


            foreach (var docId in titleTokensBulk.Keys.OrderBy(x => x))
            {
                TitleIndex.AddDocumentToIndex(titleTokensBulk[docId]);
                ContentIndex.AddDocumentToIndex(contentTokensBulk[docId]);

                Documents[docId].SetTokens(titleTokensBulk[docId], contentTokensBulk[docId]);
            }

            foreach (var document in Documents.Values)
                document.CreateVector(TitleIndex, ContentIndex);
        }
    }
}
