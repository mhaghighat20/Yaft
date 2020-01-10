﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Clustering
{
    class GMMClusteringClient : VectorClustering
    {
        ClusteringClient client = new ClusteringClient(Mode.gmm, 1.0f);

        public void Train(List<TfIdfVector> data)
        {
            client.Train(data);
        }

        public List<byte> Classify(List<TfIdfVector> vectorList)
        {
            return client.Classify(vectorList);
        }
    }
}
