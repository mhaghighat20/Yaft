using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.FileReader;
using Yaft.InvertedIndex;
using Yaft.Processor;

namespace Yaft
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        void Run()
        {
            var reader = new FileReaderFactory().GetPersianReader();
            var documents = reader.ReadFile();

            var ppc = new PreprocessClient();
            var documentTokensBulk = ppc.GetTokens(documents);

            var invertedIndex = new PositionalIndex();

            foreach (var docTokens in documentTokensBulk)
            {
                invertedIndex.AddDocumentToIndex(docTokens);
            }

            var tokenRepeats = invertedIndex.GetAllTokensRepeats()
                .OrderByDescending(x => x.repeatCount)
                .ToList();


        }
    }
}
