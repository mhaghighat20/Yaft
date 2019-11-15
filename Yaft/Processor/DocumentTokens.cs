using System.Collections.Generic;
using System.Linq;

namespace Yaft.Processor
{
    public class DocumentTokens
    {
        public int DocumentId;
        public List<(int position, string token)> TokensByPosition;

        public DocumentTokens(int documentId, Dictionary<int, string> tokensByPosition)
        {
            DocumentId = documentId;
            TokensByPosition = tokensByPosition.Select(x => (position: x.Key, token: x.Value)).ToList();
        }
    }
}