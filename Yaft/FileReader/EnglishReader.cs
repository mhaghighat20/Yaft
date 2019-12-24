using CsvHelper;
using CsvHelper.Configuration;
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

        internal EnglishReader(string filePath)
        {
            FilePath = filePath;
        }

        public List<Document> ReadFile()
        {
            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, new Configuration()
            {
                HeaderValidated = null,
                MissingFieldFound = null
            }))
            {
                csv.Configuration.HasHeaderRecord = true;
                var rows = csv.GetRecords<EnglishRow>().ToList();

                var result = new List<Document>();
                var id = IdGenerator.Instance.ReserveRange(rows.Count);

                foreach (var row in rows)
                {
                    result.Add(row.ToDocument(id));

                    id++;
                }

                return result;
            }
        }
    }
}
