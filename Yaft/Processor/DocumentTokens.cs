using System.Collections.Generic;

namespace Yaft.Processor
{
    public class DocumentTokens
    {
        public List<string> TokenList;

        public DocumentTokens(List<string> tokenList)
        {
            TokenList = tokenList;
        }
    }
}