using System;
using System.Collections.Generic;
using System.Linq;
using Yaft.InvertedIndex;

namespace Yaft.Processor
{
    public class TfIdfVector
    {
        Dictionary<string, double> IdfByToken;
        Dictionary<string, int> TfByToken;
        public Dictionary<string, double> FinalVector { get; private set; }
        public Dictionary<int, double> IntVector { get; private set; }

        public TfIdfVector(Dictionary<string, double> idfByToken, Dictionary<string, int> tfByToken)
        {
            IdfByToken = idfByToken;
            TfByToken = tfByToken;
            SetVector();
        }

        public TfIdfVector(Dictionary<string, double> finalVector)
        {
            FinalVector = finalVector;
        }

        private void SetVector()
        {
            FinalVector = TfByToken.Select(x => (token: x.Key, score: x.Value * IdfByToken[x.Key])).ToDictionary(x => x.token, x => x.score);
        }

        public TfIdfVector(PositionalIndex index, List<string> tokens, int docId)
        {
            IdfByToken = tokens.Select(x => (token: x, idf: index.Idf(x)))
                .ToDictionary(x => x.token, x => x.idf);

            TfByToken = tokens.Select(x => (token: x, tf: index.GetOccurrence(x, docId).Positions.Count))
                .ToDictionary(x => x.token, x => x.tf);

            SetVector();
        }

        internal double Multiply(TfIdfVector queryVector)
        {
            double sum = 0;
            foreach (var token in queryVector.FinalVector.Keys)
            {
                sum += queryVector.FinalVector[token] * this.FinalVector[token];
            }

            return sum;
        }

        internal TfIdfVector Add(TfIdfVector other, int sourceWeight)
        {
            var result = new Dictionary<string, double>();

            foreach (var key in this.FinalVector.Keys.Union(other.FinalVector.Keys))
            {
                double finalValue = 0.0;

                if (this.FinalVector.TryGetValue(key, out double currentValue))
                    finalValue += sourceWeight * currentValue;

                if (other.FinalVector.TryGetValue(key, out double otherValue))
                    finalValue += otherValue;

                result.Add(key, finalValue);
            }

            return new TfIdfVector(result);
        }

        internal void ConvertTokenToId(TokenMapper mapper)
        {
            IntVector = new Dictionary<int, double>();

            foreach (var item in FinalVector)
                IntVector.Add(mapper.GetOrCreateId(item.Key), item.Value);
        }
    }
}