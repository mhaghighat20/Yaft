using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yaft.FileReader;
using Yaft.Processor;
using Yaft.Storage;

namespace Yaft
{
    class Phase3Business
    {
        Dictionary<int, DocumentWrapper> Documents { get; set; }

        public void Run()
        {
            PrepareData();

            var mlContext = new MLContext(seed: 0);
            var dataView = mlContext.Data.LoadFromEnumerable(Documents.Values.ToList());

            string featuresColumnName = "Features";

            var pipeline = mlContext.Transforms
                .Concatenate(featuresColumnName, "ContentVector") 
                .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, numberOfClusters: 5));

            var model = pipeline.Fit(dataView);
            
        }

        private void PrepareData()
        {
            var reader = new FileReaderFactory().GetEnglishReaderForPhase3();
            Documents = reader.ReadFile().Select(x => new DocumentWrapper(x)).ToDictionary(x => x.Document.Id);

            var generator = new VectorGenerator(Documents, false);

            generator.Process();
        }
    }
}
