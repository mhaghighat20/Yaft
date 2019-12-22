using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Classification;
using Yaft.FileReader;
using Yaft.InvertedIndex;
using Yaft.Processor;
using Yaft.Storage;

namespace Yaft
{
    class Phase2Business
    {
        PositionalIndex TitleIndex { get; set; }
        PositionalIndex ContentIndex { get; set; }
        TokenMapper tokenMapper = new TokenMapper();

        Dictionary<int, DocumentWrapper> Documents { get; set; }

        public void Run()
        {
            var classifier = new KnnClassifierClient();

            //PrepareData(true);
            //classifier.Train(Documents.Values.Select(x => (x.CreateClassificationVector(tokenMapper), x.Document.Tag)).ToList());

            PrepareData(false);
            var result = classifier.Classify(Documents.Values.Select(x => x.CreateClassificationVector(tokenMapper)).ToList());

            for (int i = 0; i < result.Count; i++)
            {
                Documents.Values.ElementAt(i).ClassifiedTag = result[i];
            }

            var docs = Documents.Values;
            var correct = docs.Count(x => x.IsClassifiedCorrectly());
            var precision =  correct * 1.0 / docs.Count;


            Console.WriteLine(classifier.GetType().Name + " result:");
            Console.WriteLine(nameof(correct) + ": " + correct);
            Console.WriteLine(nameof(docs) + ": " + docs.Count);
            Console.WriteLine(nameof(precision) + ": " + precision);


            Console.ReadLine();
        }

        private void PrepareData(bool isTrain)
        {
            TitleIndex = new PositionalIndex();
            ContentIndex = new PositionalIndex();

            var reader = new FileReaderFactory().GetEnglishReaderForPhase2(isTrain);
            Documents = reader.ReadFile().Select(x => new DocumentWrapper(x)).ToDictionary(x => x.Document.Id);

            CreateTokensAndIndex();

            foreach (var document in Documents.Values)
                document.CreateVector(TitleIndex, ContentIndex);
        }

        private void CreateTokensAndIndex()
        {
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
        }
    }
}
