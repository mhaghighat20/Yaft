using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.FileReader
{
    public class EnglishReader : IFileReader
    {
        private readonly string FilePath;

        public EnglishReader(string filePath)
        {
            FilePath = filePath;
        }

        public List<Document> ReadFile()
        {
            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.HasHeaderRecord = true;
                var records = csv.GetRecords<EnglishRow>();

                var result = new List<Document>();

                foreach (var record in records)
                {
                    result.Add(record.ToDocument());
                }

                return result;
            }
        }
    }
}
