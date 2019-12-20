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

        void RunPhase2 ()
        {
            new Phase2Business().Run();
        }

        void RunPhase1()
        {
            MainIndex = new PositionalIndex();

            IndexEnglishFiles();
            //IndexPersianFiles();

            var tokenRepeats = MainIndex.GetAllTokensRepeats()
                .OrderByDescending(x => x.repeatCount)
                .ToList();

            Console.WriteLine(string.Join(",", tokenRepeats));
            Console.WriteLine("Unique Token count is: " + tokenRepeats.Count);


            WriteInFile(MainIndex, "MainIndex");
            GenerateBiword();

            while (true)
            {
                Console.WriteLine("Enter your command: [query|proximity(windowSize)|and|compress|decompress|info(word)]");
                var command = Console.ReadLine();
                var commandWords = command.Split(' ');

                if (command == "and" || command == "a" || command == "q" || command == "query" || commandWords.First() == "p" || commandWords.First() == "proximity")
                {
                    Console.WriteLine("Enter your query: ");
                    var query = Console.ReadLine();

                    if (string.IsNullOrEmpty(query))
                    {
                        Console.WriteLine("Your query is empty.");
                        continue;
                    }

                    var queryExecuter = new QueryExecuter(query, MainIndex);

                    List<SearchResult> result = null;
                    if (command == "and" || command == "a")
                        result = queryExecuter.ExecuteAndSearch();
                    else
                    {
                        if (commandWords.First() == "p" || commandWords.First() == "proximity")
                            result = queryExecuter.ExecuteTfIdfSearch(Convert.ToInt32(commandWords[1]));
                        else
                            result = queryExecuter.ExecuteTfIdfSearch();
                    }

                    Console.WriteLine("Enter classification filter: [1|2|3|4|no filter(Enter)]");
                    var classFilterLine = Console.ReadLine();
                    byte? classFilter = null;

                    

                    Console.WriteLine("Query after preprocess: " + JsonConvert.SerializeObject(queryExecuter.Query));
                    Console.WriteLine(string.Join(Environment.NewLine, result));
                    Console.WriteLine("---------------------------------" + Environment.NewLine);
                }
                else if (command.StartsWith("info"))
                {
                    var word = command.Split(' ')[1];

                    Console.WriteLine(MainIndex.GetWordInfo(word));
                }
                else if (command.Contains("compress"))
                {
                    Console.WriteLine("Enter compress mode: [gamma|varbyte]");
                    var modeStr = Console.ReadLine();
                    CompressUtility.Mode = (CompressMode)Enum.Parse(typeof(CompressMode), modeStr);

                    if (command.StartsWith("de"))
                        Decompress();
                    else
                        Compress();
                }
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
            WriteInFile(CompressedMainIndex, "CompressedMainIndex_" + CompressUtility.Mode);

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
            var filePath = $@"D:\Projects\MIR\Result\{filename}.json";
            File.WriteAllText(filePath, json);
            Console.WriteLine("Size is: " +  new FileInfo(filePath).Length / 1024 + " KB");
        }

        private void IndexPersianFiles()
        {
            var reader = new FileReaderFactory().GetPersianReader();

            IndexDocuments(reader);
        }

        private void IndexEnglishFiles()
        {
            var reader = new FileReaderFactory().GetEnglishReaderForPhase1();

            IndexDocuments(reader);
        }

        private void IndexDocuments(IFileReader reader)
        {
            var documents = reader.ReadFile();
            var ppc = new PreprocessClient(false, reader is EnglishReader);
            var documentTokensBulk = ppc.GetTokens(documents);


            foreach (var docTokens in documentTokensBulk.OrderBy(x => x.DocumentId))
            {
                MainIndex.AddDocumentToIndex(docTokens);
            }

            //MainIndex.SortPostings();
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.RunPhase2();
        }
    }
}
