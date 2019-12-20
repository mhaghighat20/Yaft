using System.Collections.Generic;

namespace Yaft.Processor
{
    public class TokenMapper
    {
        Dictionary<string, int> mapping = new Dictionary<string, int>();

        public int GetOrCreateId(string token)
        {
            if (mapping.TryGetValue(token, out int result))
            {
                return result;
            }
            else
            {
                var addedId = mapping.Count;
                mapping.Add(token, addedId);

                return addedId;
            }
        }
    }
}
