using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.FileReader;
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
            var documentTokens = ppc.GetTokens(documents);
            
            foreach(var doc in documentTokens.First().TokenList)
            {
                Console.WriteLine(doc.ToString());
            }

        }
    }
}
