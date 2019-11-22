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
        private PositionalIndex Index { get; set; }

        public QueryExecuter(string rawQuery, PositionalIndex index)
        {
            RawQuery = rawQuery;
            Index = index;
        }

        public List<SearchResult> Execute()
        {
            var preprocessor = new QueryProcessor(RawQuery);
            preprocessor.Preprocess();
            Query = preprocessor.Tokens;

            if (Query.Count == 0)
                throw new InvalidOperationException("Query is null");


            var matching = new Matching(Index.SearchByToken(Query.First()));

            foreach (var token in Query.Skip(1))
                matching.And(Index.SearchByToken(token));


            return matching.DocumentIds.Select(x => new SearchResult(x, Index.GetHighlight(x))).ToList();
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

        public SearchResult(int documentId, string highlight)
        {
            DocumentId = documentId;
            Highlight = highlight;
        }

        public override string ToString()
        {
            return DocumentId + ": " + Highlight;
        }
    }
}
