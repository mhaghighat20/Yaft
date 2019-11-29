using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.InvertedIndex;

namespace Yaft.Processor
{
    public class QueryExecuter
    {
        private string RawQuery { get; set; }
        public List<string> Query { get; private set; }
        public Dictionary<string, int> QueryTokenCounts { get; private set; }

        private PositionalIndex Index { get; set; }

        public QueryExecuter(string rawQuery, PositionalIndex index)
        {
            RawQuery = rawQuery;
            Index = index;

            var preprocessor = new QueryProcessor(RawQuery);
            preprocessor.Preprocess();
            Query = preprocessor.Tokens.Distinct().ToList();
            QueryTokenCounts = Query.Select(x => (token: x, count: preprocessor.Tokens.Count(y => y == x)))
                .ToDictionary(x => x.token, x => x.count);
        }

        public List<SearchResult> ExecuteAndSearch()
        {
            if (Query.Count == 0)
                throw new InvalidOperationException("Query is null");


            var matching = new Matching(Index.SearchByToken(Query.First()));

            foreach (var token in Query.Skip(1))
                matching.And(Index.SearchByToken(token));


            return matching.DocumentIds.Select(x => new SearchResult(x, Index.GetHighlight(x))).ToList();
        }

        public List<SearchResult> ExecuteTfIdfSearch()
        {
            var idfs = Query.Select(x => (token: x, idf: Idf(x))).ToDictionary(x => x.token, x => x.idf);
            var queryVector = new TfIdfVector(idfs, QueryTokenCounts);

            var docIds = new HashSet<int>();
            foreach (var token in Query)
            {
                foreach (var docId in Index.SearchByToken(token))
                    docIds.Add(docId);
            }

            var scoresByDocId = new Dictionary<int, double>();

            foreach (var docId in docIds)
            {
                var tfByToken = new Dictionary<string, int>();

                foreach (var token in Query)
                {
                    tfByToken.Add(token, Index.TermFrequency(token, docId));
                }

                var docVector = new TfIdfVector(idfs, tfByToken);
                scoresByDocId.Add(docId, docVector.Multiply(queryVector));
            }

            return scoresByDocId.OrderByDescending(x => x.Value).Select(x => new SearchResult(x.Key, Index.GetHighlight(x.Key), x.Value)).ToList();
        }


        private double Idf(string token)
        {
            var totalDocs = Index.DocumentsById.Keys.Count;
            var documentsWithTokenOccurrence = Index.SearchByToken(token).Count;

            return Math.Log(totalDocs * 1.0 / documentsWithTokenOccurrence, 2);
        }
    }

    internal class TfIdfVector
    {
        Dictionary<string, double> IdfByToken;
        Dictionary<string, int> TfByToken;
        Dictionary<string, double> scoresByToken;

        public TfIdfVector(Dictionary<string, double> idfByToken, Dictionary<string, int> tfByToken)
        {
            IdfByToken = idfByToken;
            TfByToken = tfByToken;

            scoresByToken = tfByToken.Select(x => (token: x.Key, score: x.Value * idfByToken[x.Key])).ToDictionary(x => x.token, x => x.score);
        }

        internal double Multiply(TfIdfVector queryVector)
        {
            double sum = 0;
            foreach (var token in queryVector.scoresByToken.Keys)
            {
                sum += queryVector.scoresByToken[token] * this.scoresByToken[token];
            }

            return sum;
        }
    }

    internal class Matching
    {
        internal List<int> DocumentIds { get; private set; }

        public Matching(List<int> documentIds)
        {
            DocumentIds = documentIds;
        }

        internal void And(List<int> newDocumentIds)
        {
            DocumentIds = DocumentIds.Intersect(newDocumentIds).ToList();
        }
    }

    public class SearchResult
    {
        public int DocumentId;
        public string Highlight;
        public double Score;

        public SearchResult(int documentId, string highlight) : this(documentId, highlight, 1)
        {
            
        }

        public SearchResult(int documentId, string highlight, double score) 
        {
            DocumentId = documentId;
            Highlight = highlight;
            Score = score;
        }

        public override string ToString()
        {
            return DocumentId + $" (score={Score}): " + Highlight;
        }
    }
}
