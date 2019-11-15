using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Yaft.InvertedIndex;

namespace Yaft.Storage
{
    public class CompressedIndex
    {
        public Dictionary<string, CompressedPosting> PostingsByToken { get; private set; }

        public CompressedIndex()
        {
            PostingsByToken = new Dictionary<string, CompressedPosting>();
        }

        public void AddPosting(string token, TokenPosting posting)
        {
            var cp = new CompressedPosting(posting);
            PostingsByToken.Add(token, cp);
        }

        public PositionalIndex Decompress()
        {
            var result = new PositionalIndex();

            foreach (var tokenItem in PostingsByToken)
            {
                var token = tokenItem.Key;
                result.IndexByTokens.Add(token, tokenItem.Value.Decompress(token));
            }

            return result;
        }
    }

    public class CompressedPosting
    {
        string CompressedListOfDocumentIds;

        List<string> OccurrencesList;

        [JsonIgnore]
        public List<List<int>> rawData;

        public CompressedPosting(TokenPosting posting)
        {
            rawData = new List<List<int>>();
            var docIds = posting.AllOccurrencesByDocumentId.Keys.OrderBy(x => x).ToList();

            rawData.Add(docIds);

            foreach (var docOccurrence in posting.AllOccurrencesByDocumentId.OrderBy(x => x.Key).Select(x => x.Value))
            {
                rawData.Add(docOccurrence.Positions.ToList());
            }
        }

        public void PlaceCompressedData(List<string> compressedStrings)
        {
            CompressedListOfDocumentIds = compressedStrings.First();
            OccurrencesList = compressedStrings.Skip(1).ToList();
        }

        public TokenPosting Decompress(string token)
        {
            var result = new TokenPosting(token);

            var data = new List<string>() { CompressedListOfDocumentIds };
            data.AddRange(OccurrencesList);

            var decompressedData = new CompressUtility().DecompressList(data);
            var docIds = decompressedData.First();

            var i = 1;
            foreach (var docId in docIds)
            {
                result.GetOrCreateDocumentOccurrence(docId).Positions.AddRange(decompressedData[i]);
            }

            return result;
        }
    }
}