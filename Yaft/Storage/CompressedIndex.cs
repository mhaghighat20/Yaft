using System.Collections.Generic;
using System.Linq;
using Yaft.InvertedIndex;

namespace Yaft.Storage
{
    public class CompressedIndex
    {
        Dictionary<string, CompressedPosting> PostingsByToken;

        public CompressedIndex()
        {
            PostingsByToken = new Dictionary<string, CompressedPosting>();
        }

        public void AddPosting(string token, TokenPosting posting)
        {
            PostingsByToken.Add(token, new CompressedPosting(posting));
        }

        public PositionalIndex Decompress()
        {
            var result = new PositionalIndex();

            foreach(var tokenItem in PostingsByToken)
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

        public CompressedPosting(TokenPosting posting)
        {
            var rawData = new List<List<int>>();
            var docIds = posting.AllOccurrencesByDocumentId.Keys.ToList();

            rawData.Add(docIds);

            foreach (var docOccurrence in posting.AllOccurrencesByDocumentId.OrderBy(x => x.Key).Select(x => x.Value))
            {
                rawData.Add(docOccurrence.Positions.ToList());
            }

            var compressUtil = new CompressUtility();
            var compressedStrings = compressUtil.CompressIntList(rawData);

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