using System.Collections.Generic;
using System.Linq;
using Yaft.InvertedIndex;

namespace Yaft.Storage
{
    public class IndexCompressor
    {
        public CompressedIndex Compress(PositionalIndex uncompressedIndex)
        {
            CompressedIndex compressedIndex = CopyDataToCompressedIndex(uncompressedIndex);

            var mapping = new List<RawDataMapping>();

            foreach (var tokenPosting in compressedIndex.PostingsByToken)
            {
                mapping.AddRange(tokenPosting.Value.RawData.Select(list => new RawDataMapping(tokenPosting.Key, list)));
            }

            new CompressUtility().CompressIntList(mapping);


            foreach (var tokenPosting in compressedIndex.PostingsByToken)
            {
                var postingCompressedStrings = mapping.Where(x => x.Token == tokenPosting.Key)
                    .OrderBy(x => x.Id)
                    .Select(x => x.CompressedData)
                    .ToList();

                tokenPosting.Value.PlaceCompressedData(postingCompressedStrings);
            }

            return compressedIndex;
        }

        private CompressedIndex CopyDataToCompressedIndex(PositionalIndex uncompressedIndex)
        {
            var result = new CompressedIndex();
            foreach (var item in uncompressedIndex.PostingsByToken)
            //Parallel.ForEach(uncompressedIndex.IndexByTokens, new ParallelOptions() { MaxDegreeOfParallelism = 50 }, (item) =>
            {
                result.AddPosting(item.Key, item.Value);
            }

            return result;
        }

        public PositionalIndex Decompress(CompressedIndex compressedIndex)
        {
            //return compressedIndex.Decompress();

            //foreach (var item in compressedIndex.PostingsByTokens)
            //    result.(item.Key, item.Value);

            var mapping = new List<RawDataMapping>();
            foreach (var compressedPosting in compressedIndex.PostingsByToken)
            {
                mapping.Add(new RawDataMapping(compressedPosting.Key, compressedPosting.Value.CompressedListOfDocumentIds));
                mapping.AddRange(compressedPosting.Value.OccurrencesList.Select(x => new RawDataMapping(compressedPosting.Key, x)));
            }

            new CompressUtility().DecompressList(mapping);

            var result = new PositionalIndex();
            foreach (var tokenPosting in compressedIndex.PostingsByToken)
            {
                var postingIntList = mapping.Where(x => x.Token == tokenPosting.Key)
                    .OrderBy(x => x.Id)
                    .Select(x => x.RawData)
                    .ToList();

                var decompressedPosting = tokenPosting.Value.Decompress(postingIntList);

                result.AddDecompressedPosting(decompressedPosting);
            }

            return result;
        }
    }
}
