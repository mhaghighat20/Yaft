using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.FileReader;
using Yaft.InvertedIndex;
using Yaft.Processor;

namespace Yaft.Storage
{
    public class DocumentWrapper
    {
        public Document Document { get; }
        public byte? ClassifiedTag { get; set; }

        public DocumentTokens TitleTokens { get; private set; }
        public DocumentTokens ContentTokens { get; private set; }

        public TfIdfVector TitleVector { get; private set; }
        public TfIdfVector ContentVector { get; private set; }


        public DocumentWrapper(Document document)
        {
            Document = document;
        }

        public void SetTokens(DocumentTokens titleTokens, DocumentTokens contentTokens)
        {
            TitleTokens = titleTokens;
            ContentTokens = contentTokens;
        }

        public void CreateVector(PositionalIndex TitleIndex, PositionalIndex ContentIndex)
        {
            TitleVector = new TfIdfVector(TitleIndex, GetTokens(TitleTokens), Document.Id);
            ContentVector = new TfIdfVector(ContentIndex, GetTokens(ContentTokens), Document.Id);
        }

        internal TfIdfVector CreateClassificationVector(TokenMapper mapper)
        {
            var result = TitleVector.Add(ContentVector, 2);
            result.ConvertTokenToId(mapper);

            return result;
        }

        private List<string> GetTokens(DocumentTokens documentTokens)
        {
            return documentTokens.TokensByPosition.Select(x => x.token).Distinct().ToList();
        }
    }
}
