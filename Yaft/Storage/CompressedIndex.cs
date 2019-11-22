using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Yaft.InvertedIndex;

namespace Yaft.Storage
{
    public class CompressedIndex
    {
        public Dictionary<string, CompressedPosting> PostingsByToken { get; set; }

        public CompressedIndex()
        {
            PostingsByToken = new Dictionary<string, CompressedPosting>();
        }

        public void AddPosting(string token, TokenPosting posting)
        {
            var cp = new CompressedPosting(posting);
            PostingsByToken.Add(token, cp);
        }

        //public PositionalIndex Decompress()
        //{
        //    var result = new PositionalIndex();

        //    foreach (var tokenItem in PostingsByToken)
        //    {
        //        var token = tokenItem.Key;
        //        result.PostingsByToken.Add(token, tokenItem.Value.Decompress(token));
        //    }

        //    return result;
        //}
    }

    public class CompressedPosting
    {
        public string CompressedListOfDocumentIds { get; set; }

        public List<string> OccurrencesList { get; set; }

        [JsonIgnore]
        public List<List<int>> RawData { get; set; }
        
        // TODO re-init here after reading from disk
        [JsonIgnore]
        public string Token { get; set; }

        public CompressedPosting(TokenPosting posting)
        {
            RawData = new List<List<int>>();
            Token = posting.Token;

            var docIds = posting.AllOccurrencesByDocumentId.Keys.OrderBy(x => x).ToList();

            RawData.Add(docIds);

            foreach (var docOccurrence in posting.AllOccurrencesByDocumentId.OrderBy(x => x.Key).Select(x => x.Value))
            {
                RawData.Add(docOccurrence.Positions.ToList());
            }
        }

        public void PlaceCompressedData(List<string> compressedStrings)
        {
            CompressedListOfDocumentIds = compressedStrings.First();
            OccurrencesList = compressedStrings.Skip(1).ToList();
        }

        public TokenPosting Decompress(List<List<int>> decompressedData)
        {
            var result = new TokenPosting(Token);

            var docIds = decompressedData.First();

            var i = 1;
            foreach (var docId in docIds)
            {
                result.GetOrCreateDocumentOccurrence(docId).Positions.AddRange(decompressedData[i]);
                i++;
            }

            return result;
        }
    }
}