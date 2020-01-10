using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Clustering
{
    class HierarchicalClusteringClient : VectorClustering
    {
        ClusteringClient client = new ClusteringClient(Mode.hierarchical, 5);

        public List<byte> Classify(List<TfIdfVector> vectorList)
        {
            return client.Classify(vectorList);
        }
    }
}
