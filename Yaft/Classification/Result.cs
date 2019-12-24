using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaft.Storage;

namespace Yaft.Classification
{
    public class Result
    {
        private Dictionary<byte, ClassStats> StatsByClass = new Dictionary<byte, ClassStats>()
        {
            { 1, new ClassStats() },
            { 2, new ClassStats() },
            { 3, new ClassStats() },
            { 4, new ClassStats() }
        };


        public void AddItem(DocumentWrapper doc)
        {
            var isCorrect = doc.IsClassifiedCorrectly();

            if (isCorrect)
            {
                StatsByClass[doc.ClassifiedTag.Value].TruePositives++;

                for (byte i = 1; i <= 4; i++)
                {
                    if (doc.ClassifiedTag.Value != i)
                        StatsByClass[i].TrueNegatives++;
                }
            }
            else
            {
                StatsByClass[doc.ClassifiedTag.Value].FalsePositives++;
                StatsByClass[doc.Document.Tag].FalseNegatives++;

                for (byte i = 1; i <= 4; i++)
                {
                    if (doc.ClassifiedTag.Value != i && doc.Document.Tag != i)
                        StatsByClass[i].TrueNegatives++;
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(StatsByClass);
        }
    }

    public class ClassStats
    {
        public int TruePositives;
        public int FalsePositives;
        public int TrueNegatives;
        public int FalseNegatives;
    }
}
