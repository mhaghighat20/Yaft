using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Clustering
{
    public interface VectorClustering
    {
        void Train(List<TfIdfVector> data);

        List<byte> Classify(List<TfIdfVector> vectorList);
    }
}
