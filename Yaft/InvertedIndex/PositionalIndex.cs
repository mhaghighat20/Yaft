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
        public Dictionary<string, TokenPosting> PostingsByToken { get; private set; }

        public PositionalIndex()
        {
            PostingsByToken = new Dictionary<string, TokenPosting>();
        }

        public void AddDocumentToIndex(DocumentTokens documentTokens)
        {
            foreach(var (position, token) in documentTokens.TokensByPosition)
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

        public List<(string token, int repeatCount)> GetAllTokensRepeats()
        {
            return PostingsByToken.Values.Select(x => (token: x.Token, x.GetTokenOccurrenceCount())).ToList();
        }

        public void AddDecompressedPosting(TokenPosting tokenPosting)
        {
            PostingsByToken.Add(tokenPosting.Token, tokenPosting);
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
    }

    public class OccurrencesPerDocument
    {
        readonly int DocumentId;

        public List<int> Positions { get; private set; }

        public OccurrencesPerDocument(int documentId)
        {
            DocumentId = documentId;
            Positions = new List<int>();
        }
    }
}
