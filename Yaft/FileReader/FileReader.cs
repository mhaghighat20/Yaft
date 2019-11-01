using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.FileReader
{
    public interface IFileReader
    {
        List<Document> ReadFile();
    }
}
