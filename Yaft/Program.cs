using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.FileReader;

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
            var englishReader = new FileReaderFactory().GetEnglishReader();
            var documents = englishReader.ReadFile();
            
            foreach(var doc in documents)
            {
                Console.WriteLine(doc.ToString());
            }
        }
    }
}
