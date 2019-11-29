using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.FileReader
{
    public class FileReaderFactory
    {
        //private const string ParentPath = @"D:\MIR\Phase1\";
        private const string ParentPath = @"D:\Projects\MIR\Phase1\";
        public IFileReader GetEnglishReader()
        {
            return new EnglishReader(ParentPath +  @"English.csv");
        }

        public IFileReader GetPersianReader()
        {
            return new PersianReader(ParentPath + @"Persian.xml");
        }
    }
}
