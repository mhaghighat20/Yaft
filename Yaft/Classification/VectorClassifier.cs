using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Classification
{
    interface VectorClassifier
    {
        void Train(List<(TfIdfVector vector, byte Tag)> data);

        List<byte> Classify(List<TfIdfVector> vectorList);
    }
}
