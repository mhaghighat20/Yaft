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
        private const string ParentPathPhase1 = @"D:\MIR\Phase1\";

        private const string ParentPathPhase2 = @"D:\MIR\Phase2\";

        public const string ParentPathPhase3 = @"D:\MIR\Phase3\";

        public IFileReader GetEnglishReaderForPhase1()
        {
            return new EnglishReader(ParentPathPhase1 +  @"English.csv");
        }

        public IFileReader GetEnglishReaderForPhase2(bool isTrain)
        {
            var suffix = "_test.csv";
            if (isTrain)
                suffix = "_train.csv";

            return new EnglishReader(ParentPathPhase2 + @"phase2" + suffix);
        }

        public IFileReader GetEnglishReaderForPhase3()
        {
            return new EnglishReader(ParentPathPhase3 + "Data.csv");
        }

        public IFileReader GetPersianReader()
        {
            return new PersianReader(ParentPathPhase1 + @"Persian.xml");
        }
    }
}
