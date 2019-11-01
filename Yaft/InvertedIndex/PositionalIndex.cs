using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.InvertedIndex
{
    public class PositionalIndex
    {
        private Dictionary<string, TokenPostings> IndexByTokens { get; set; }

        public PositionalIndex()
        {
            IndexByTokens = new Dictionary<string, TokenPostings>();
        }

        public void AddDocumentToIndex(DocumentTokens documentTokens)
        {
            foreach(var tokenPositionTuple in documentTokens.TokensByPosition)
            {
                AddToken(tokenPositionTuple.token, documentTokens.DocumentId, tokenPositionTuple.position);
            }
        }

        private void AddToken(string token, string documentId, int position)
        {
            var postings = GetOrCreateTokenPostings(token);
            var occurrencePerDocument = postings.GetOrCreateDocumentOccurrence(documentId);

            occurrencePerDocument.Positions.Add(position);
        }

        private TokenPostings GetOrCreateTokenPostings(string token)
        {
            if (IndexByTokens.TryGetValue(token, out TokenPostings tokenPostings))
            {
                return tokenPostings;
            }
            else
            {
                var result = new TokenPostings(token);
                IndexByTokens.Add(token, result);

                return result;
            }
        }

        public List<(string token, int repeatCount)> GetAllTokensRepeats()
        {
            return IndexByTokens.Values.Select(x => (token: x.Token, x.GetTokenOccurrenceCount())).ToList();
        }
    }

    internal class TokenPostings
    {
        public readonly string Token;

        public Dictionary<string, OccurrencesPerDocument> AllOccurrencesByDocumentId { get; private set; }

        public TokenPostings(string token)
        {
            Token = token;
            AllOccurrencesByDocumentId = new Dictionary<string, OccurrencesPerDocument>();
        }

        /// <summary>
        /// Calculates how many times which this token is repeated.
        /// </summary>
        /// <returns>number of repeats</returns>
        public int GetTokenOccurrenceCount()
        {
            return AllOccurrencesByDocumentId.Values.Sum(x => x.Positions.Count);
        }

        internal OccurrencesPerDocument GetOrCreateDocumentOccurrence(string documentId)
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
    }

    internal class OccurrencesPerDocument
    {
        readonly string DocumentId;

        public List<int> Positions { get; private set; }

        public OccurrencesPerDocument(string documentId)
        {
            DocumentId = documentId;
            Positions = new List<int>();
        }
    }
}
