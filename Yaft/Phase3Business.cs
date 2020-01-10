using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yaft.Clustering;
using Yaft.FileReader;
using Yaft.Processor;
using Yaft.Storage;

namespace Yaft
{
    class Phase3Business
    {
        Dictionary<int, DocumentWrapper> Documents { get; set; }
        
        TokenMapper tokenMapper = new TokenMapper();

        public void Run()
        {
            PrepareData();

            var clusteringClient = new KMeansClusteringClient();

            var result = clusteringClient.Classify(Documents.Values.Select(x => x.CreateClassificationVector(tokenMapper)).ToList());

            for (int i = 0; i < result.Count; i++)
            {
                Documents.Values.ElementAt(i).ClassifiedTag = result[i];
            }

            new ClusteringResult(clusteringClient.GetType().Name, Documents.Values).Write();
        }

        private void PrepareData()
        {
            var reader = new FileReaderFactory().GetEnglishReaderForPhase3();
            Documents = reader.ReadFile().Select(x => new DocumentWrapper(x)).ToDictionary(x => x.Document.Id);

            var generator = new VectorGenerator(Documents, false);

            generator.Process();
        }
    }

    class ClusteringResult
    {
        string Name;

        List<ResultRow> List;

        public ClusteringResult(string mode, IEnumerable<DocumentWrapper> input)
        {
            var dt = DateTime.Now.ToString("HH-mm");
            Name = mode + "_" + dt;
            List = input.Select(x => new ResultRow()
            {
                ID = x.Document.Id,
                Text = x.Document.Text,
                ClusterId = x.ClassifiedTag.Value
            }).ToList();
        }

        public void Write()
        {
            using (var writer = new StreamWriter(FileReaderFactory.ParentPathPhase3 + Name))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(List);
            }
        }
    }

    class ResultRow
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public byte ClusterId { get; set; }
    }
}
