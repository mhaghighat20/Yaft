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

        private bool HasTitle { get; set; }

        public VectorGenerator(Dictionary<int, DocumentWrapper> documents, bool hasTitle)
        {
            Documents = documents;
            HasTitle = hasTitle;
        }

        public void Process()
        {
            if (HasTitle)
                TitleIndex = new PositionalIndex();

            ContentIndex = new PositionalIndex();

            var pureDocs = Documents.Values.Select(x => x.Document).ToList();

            Dictionary<int, DocumentTokens> titleTokensBulk = null;

            if (HasTitle)
            {
                var titlePreprocessClient = new PreprocessClient(true, true);
                titleTokensBulk = titlePreprocessClient.GetTokens(pureDocs).ToDictionary(x => x.DocumentId);
            }

            var contentPreprocessClient = new PreprocessClient(false, true);
            var contentTokensBulk = contentPreprocessClient.GetTokens(pureDocs).ToDictionary(x => x.DocumentId);


            foreach (var docId in contentTokensBulk.Keys.OrderBy(x => x))
            {
                if (HasTitle)
                    TitleIndex.AddDocumentToIndex(titleTokensBulk[docId]);

                ContentIndex.AddDocumentToIndex(contentTokensBulk[docId]);

                if (HasTitle)
                    Documents[docId].SetTokens(titleTokensBulk[docId], contentTokensBulk[docId]);
                else
                    Documents[docId].SetTokens(null, contentTokensBulk[docId]);
            }

            foreach (var document in Documents.Values)
                document.CreateVector(TitleIndex, ContentIndex);
        }
    }
}
