using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Classification
{
    class NaiveBayesClassifierClient : VectorClassifier
    {
        ClassifierClient client = new ClassifierClient(Mode.naivebayes, 2.0f);

        public void Train(List<(TfIdfVector vector, byte Tag)> data)
        {
            client.Train(data);
        }

        public List<byte> Classify(List<TfIdfVector> vectorList)
        {
            return client.Classify(vectorList);
        }
    }
}
