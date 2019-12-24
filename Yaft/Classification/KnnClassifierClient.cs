using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Processor;

namespace Yaft.Classification
{
    public class KnnClassifierClient
    {
        ClassifierClient client = new ClassifierClient(Mode.knn, 1.0f);

        public void Train(List<(TfIdfVector vector, byte Tag)> data)
        {
            client.Train(data);
        }

        public List<byte> Classify(List<TfIdfVector> vectorList)
        {
            var result = new List<byte>();

            var i = 0;
            foreach (var batch in vectorList.Chunk(50))
            {
                Console.WriteLine("Page " + i++);
                result.AddRange(client.Classify(batch.ToList()));
            }

            return result;
        }
    }

    public static class ChunkExtension
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }

            while (list.Any())
            {
                yield return list.Take(chunkSize);
                list = list.Skip(chunkSize);
            }
        }
    }
}
