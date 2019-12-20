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

        public List<SearchResult> ExecuteTfIdfSearch(int? windowSize = null)
        {
            var idfs = Query.Select(x => (token: x, idf: Index.Idf(x))).ToDictionary(x => x.token, x => x.idf);
            var queryVector = new TfIdfVector(idfs, QueryTokenCounts);

            var docIds = new HashSet<int>();
            foreach (var token in Query)
            {
                foreach (var docId in Index.SearchByToken(token))
                    docIds.Add(docId);
            }

            if (windowSize.HasValue)
                docIds = ProximityFilter(docIds, windowSize.Value);

            var scoresByDocId = new Dictionary<int, double>();

            foreach (var docId in docIds)
            {
                var tfByToken = new Dictionary<string, int>();

                foreach (var token in Query)
                {
                    tfByToken.Add(token, Index.GetOccurrence(token, docId).Positions.Count);
                }

                var docVector = new TfIdfVector(idfs, tfByToken);
                scoresByDocId.Add(docId, docVector.Multiply(queryVector));
            }

            return scoresByDocId.OrderByDescending(x => x.Value).Select(x => new SearchResult(x.Key, Index.GetHighlight(x.Key), x.Value)).ToList();
        }

        private HashSet<int> ProximityFilter(HashSet<int> docIds, int windowSize)
        {
            var newDocIds = new HashSet<int>();

            foreach (var docId in docIds)
            {
                var firstPostitions = new List<int>();

                foreach (var token in Query)
                {
                    var positions = Index.GetOccurrence(token, docId).Positions;

                    if (positions.Any())
                        firstPostitions.Add(positions.First());
                }

                if (firstPostitions.Count == Query.Count)
                {
                    if (firstPostitions.SequenceEqual(firstPostitions.OrderBy(x => x).ToList()))
                    {
                        if (firstPostitions.Last() - firstPostitions.First() <= windowSize)
                            newDocIds.Add(docId);
                    }
                }

            }

            return newDocIds;
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
