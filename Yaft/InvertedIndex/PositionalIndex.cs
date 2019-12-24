using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.FileReader;
using Yaft.Processor;

namespace Yaft.InvertedIndex
{
    public class PositionalIndex
    {
        public Dictionary<string, TokenPosting> PostingsByToken { get; private set; }

        [JsonIgnore]
        public Dictionary<int, List<string>> DocumentsById { get; private set; }

        [JsonIgnore]
        public Dictionary<int, Document> PureDocumentsById { get; set; }

        public PositionalIndex()
        {
            PostingsByToken = new Dictionary<string, TokenPosting>();
            DocumentsById = new Dictionary<int, List<string>>();
        }

        public void AddDocumentToIndex(DocumentTokens documentTokens)
        {
            DocumentsById.Add(documentTokens.DocumentId, documentTokens.TokensByPosition
                .OrderBy(x => x.position)
                .Select(x => x.token)
                .ToList());

            foreach (var (position, token) in documentTokens.TokensByPosition)
            {
                AddToken(token, documentTokens.DocumentId, position);
            }
        }

        private void AddToken(string token, int documentId, int position)
        {
            var postings = GetOrCreateTokenPostings(token);
            var occurrencePerDocument = postings.GetOrCreateDocumentOccurrence(documentId);

            occurrencePerDocument.Positions.Add(position);
        }

        private TokenPosting GetOrCreateTokenPostings(string token)
        {
            if (PostingsByToken.TryGetValue(token, out TokenPosting tokenPostings))
            {
                return tokenPostings;
            }
            else
            {
                var result = new TokenPosting(token);
                PostingsByToken.Add(token, result);

                return result;
            }
        }

        public List<string> GetAllTokens()
        {
            return PostingsByToken.Keys.ToList();
        }

        public List<(string token, int repeatCount)> GetAllTokensRepeats()
        {
            return PostingsByToken.Values.Select(x => (token: x.Token, x.GetTokenOccurrenceCount())).ToList();
        }

        public void AddDecompressedPosting(TokenPosting tokenPosting)
        {
            PostingsByToken.Add(tokenPosting.Token, tokenPosting);
        }

        public void SortPostings()
        {
            foreach (var posting in PostingsByToken.Values)
            {
                posting.Sort();
            }
        }

        public List<int> SearchByToken(string token)
        {
            if (PostingsByToken.TryGetValue(token, out TokenPosting value))
            {
                return value.AllOccurrencesByDocumentId.Keys.ToList();
            }
            else
            {
                return new List<int>();//TokenPosting(token);
            }
        }

        public OccurrencesPerDocument GetOccurrence(string token, int docId)
        {
            if (PostingsByToken.TryGetValue(token, out TokenPosting value))
            {
                if (value.AllOccurrencesByDocumentId.TryGetValue(docId, out OccurrencesPerDocument occurrences))
                    return occurrences;
            }

            return new OccurrencesPerDocument(docId);
        }

        public string GetWordInfo(string token)
        {
            var sb = new StringBuilder();
            if (PostingsByToken.TryGetValue(token, out TokenPosting value))
            {
                foreach (var doc in value.AllOccurrencesByDocumentId.Values)
                {
                    sb.Append("DocId: " + doc.DocumentId);
                    sb.Append(" Positions: " + string.Join(",", doc.Positions));
                    sb.Append(Environment.NewLine);
                }

                return sb.ToString();
            }
            else
            {
                return "This word does not exist";
            }

        }

        public string GetHighlight(int documentId)
        {
            return string.Join(" ", DocumentsById[documentId].Take(20));
        }

        public double Idf(string token)
        {
            var totalDocs = DocumentsById.Keys.Count;
            var documentsWithTokenOccurrence = SearchByToken(token).Count;

            return Math.Log(totalDocs * 1.0 / documentsWithTokenOccurrence, 2);
        }
    }

    public class TokenPosting
    {
        public readonly string Token;

        public Dictionary<int, OccurrencesPerDocument> AllOccurrencesByDocumentId { get; private set; }

        public TokenPosting(string token)
        {
            Token = token;
            AllOccurrencesByDocumentId = new Dictionary<int, OccurrencesPerDocument>();
        }

        /// <summary>
        /// Calculates how many times which this token is repeated.
        /// </summary>
        /// <returns>number of repeats</returns>
        public int GetTokenOccurrenceCount()
        {
            return AllOccurrencesByDocumentId.Values.Sum(x => x.Positions.Count);
        }

        internal OccurrencesPerDocument GetOrCreateDocumentOccurrence(int documentId)
        {
            if (AllOccurrencesByDocumentId.TryGetValue(documentId, out OccurrencesPerDocument occurrencesPerDocument))
            {
                return occurrencesPerDocument;
            }
            else
            {
                var result = new OccurrencesPerDocument(documentId);
                AllOccurrencesByDocumentId.Add(documentId, result);

                return result;
            }

        }

        internal void Sort()
        {
            AllOccurrencesByDocumentId = AllOccurrencesByDocumentId.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        //internal TokenPosting Clone()
        //{
        //    var result = new TokenPosting(Token);
        //    result.AllOccurrencesByDocumentId = new Dictionary<int, OccurrencesPerDocument>(this.AllOccurrencesByDocumentId);

        //    foreach(var occurrence in AllOccurrencesByDocumentId)
        //        result.AllOccurrencesByDocumentId[key]
        //}
    }

    public class OccurrencesPerDocument
    {
        public readonly int DocumentId;

        public List<int> Positions { get; private set; }

        public OccurrencesPerDocument(int documentId)
        {
            DocumentId = documentId;
            Positions = new List<int>();
        }
    }
}
