using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.FileReader;
using Yaft.InvertedIndex;
using Yaft.Processor;
using Yaft.Storage;

namespace Yaft
{
    class Program
    {
        PositionalIndex MainIndex;
        CompressedIndex CompressedMainIndex;

        void Run()
        {
            MainIndex = new PositionalIndex();

            IndexEnglishFiles();
            //IndexPersianFiles();

            var tokenRepeats = MainIndex.GetAllTokensRepeats()
                .OrderByDescending(x => x.repeatCount)
                .ToList();

            Console.WriteLine(string.Join(",", tokenRepeats));
            Console.WriteLine("Token count is: " + tokenRepeats.Count);
            WriteInFile(MainIndex, "MainIndex");
            GenerateBiword();

            //Console.ReadLine();
            //Compress();
            //Console.ReadLine();

            //Decompress();
            //Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Enter your query: ");
                var query = Console.ReadLine();

                if (string.IsNullOrEmpty(query))
                {
                    Console.WriteLine("Your query is empty.");
                    continue;
                }

                var queryExecuter = new QueryExecuter(query, MainIndex);
                var result = queryExecuter.Execute();

                Console.WriteLine("Query after preprocess: " + JsonConvert.SerializeObject(queryExecuter.Query));
                Console.WriteLine(string.Join(Environment.NewLine, result));
                Console.WriteLine("---------------------------------" + Environment.NewLine);
            }
        }

        private void GenerateBiword()
        {
            Console.WriteLine("Generating Biword Index...");
            new BigramWrapper().IndexTokens(MainIndex.GetAllTokens());
            Console.WriteLine("Completed Generating Biword Index...");
        }

        private void Compress()
        {
            Console.WriteLine("Compressing...");
            CompressedMainIndex = new IndexCompressor().Compress(MainIndex);
            WriteInFile(CompressedMainIndex, "CompressedMainIndex");

            Console.WriteLine("Compressed Successfully");
        }

        private void Decompress()
        {
            Console.WriteLine("decompressing...");
            var decompressedIndex = new IndexCompressor().Decompress(CompressedMainIndex);
            WriteInFile(decompressedIndex, "decompressedIndex");

            Console.WriteLine("Decompressed Successfully");
        }

        private void WriteInFile(object index, string filename)
        {
            var json = JsonConvert.SerializeObject(index);
            File.WriteAllText($@"D:\MIR\Result\{filename}.json", json);
        }

        private void IndexPersianFiles()
        {
            var reader = new FileReaderFactory().GetPersianReader();

            IndexDocuments(reader);
        }

        private void IndexEnglishFiles()
        {
            var reader = new FileReaderFactory().GetEnglishReader();

            IndexDocuments(reader);
        }

        private void IndexDocuments(IFileReader reader)
        {
            var documents = reader.ReadFile();
            var ppc = new PreprocessClient();
            var documentTokensBulk = ppc.GetTokens(documents, reader is EnglishReader);


            foreach (var docTokens in documentTokensBulk.OrderBy(x => x.DocumentId))
            {
                MainIndex.AddDocumentToIndex(docTokens);
            }

            //MainIndex.SortPostings();
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }
    }
}
