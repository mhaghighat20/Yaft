﻿using System;
using System.Collections.Generic;
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
            Console.ReadLine();

            CompressedMainIndex = new IndexCompressor().Compress(MainIndex);

            Console.WriteLine("Compressed Successfully");
            Console.ReadLine();

            var decompressedIndex = new IndexCompressor().Decompress(CompressedMainIndex);

            Console.WriteLine("Decompressed Successfully");
            Console.ReadLine();
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


            foreach (var docTokens in documentTokensBulk)
            {
                MainIndex.AddDocumentToIndex(docTokens);
            }
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }
    }
}
