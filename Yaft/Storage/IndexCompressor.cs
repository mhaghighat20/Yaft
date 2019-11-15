using System.Collections.Generic;
using System.Linq;
using Yaft.InvertedIndex;

namespace Yaft.Storage
{
    public class IndexCompressor
    {
        public CompressedIndex Compress(PositionalIndex uncompressedIndex)
        {
            var result = new CompressedIndex();

            foreach (var item in uncompressedIndex.IndexByTokens)
            //Parallel.ForEach(uncompressedIndex.IndexByTokens, new ParallelOptions() { MaxDegreeOfParallelism = 50 }, (item) =>
            {
                result.AddPosting(item.Key, item.Value);
            }

            var mapping = new List<RawDataMapping>();

            foreach (var tokenPosting in result.PostingsByToken)
            {
                mapping.AddRange(tokenPosting.Value.rawData.Select(list => new RawDataMapping(tokenPosting.Key, list)));
            }

            new CompressUtility().CompressIntList(mapping);

            foreach (var tokenPosting in result.PostingsByToken)
            {
                var postingCompressedStrings = mapping.Where(x => x.Token == tokenPosting.Key)
                    .OrderBy(x => x.Id)
                    .Select(x => x.CompressedData)
                    .ToList();

                tokenPosting.Value.PlaceCompressedData(postingCompressedStrings);
            }

            return result;
        }

        public PositionalIndex Decompress(CompressedIndex compressedIndex)
        {
            return compressedIndex.Decompress();
        }
    }
}
