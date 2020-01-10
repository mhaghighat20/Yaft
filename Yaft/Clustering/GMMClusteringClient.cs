using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Clustering
{
    class GMMClusteringClient : VectorClustering
    {
        ClusteringClient client = new ClusteringClient(Mode.gmm, 5);

        public List<byte> Classify(List<TfIdfVector> vectorList)
        {
            return client.Classify(vectorList);
        }
    }
}
