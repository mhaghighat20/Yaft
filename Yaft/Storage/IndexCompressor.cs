using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.InvertedIndex;

namespace Yaft.Storage
{
    public class IndexCompressor
    {
        PositionalIndex UncompressedIndex;
        CompressedIndex CompressedIndex;

        public IndexCompressor(PositionalIndex uncompressedIndex)
        {
            UncompressedIndex = uncompressedIndex;
        }

        public IndexCompressor(CompressedIndex compressedIndex)
        {
            CompressedIndex = compressedIndex;
        }

        public void Compress()
        {
            var result = new CompressedIndex();

            foreach (var item in UncompressedIndex.IndexByTokens)
            {
                result.AddPosting(item.Key, item.Value);
            }
        }

        public void Uncompress()
        {

        }
    }
}
