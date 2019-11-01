using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.FileReader
{
    public class FileReaderFactory
    {
        public IFileReader GetEnglishReader()
        {
            return new EnglishReader(@"D:\MIR\Phase1\English.csv");
        }

        public IFileReader GetPersianReader()
        {
            return new PersianReader(@"D:\MIR\Phase1\Persian.xml");
        }
    }
}
