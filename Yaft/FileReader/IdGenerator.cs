using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaft.FileReader
{
    public sealed class IdGenerator
    {
        private static readonly IdGenerator instance = new IdGenerator();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static IdGenerator() { }

        private int Counter;
        private IdGenerator()
        {
            Counter = 1;
        }

        /// <summary>
        /// Thread-safe range Id Generator
        /// </summary>
        /// <param name="rangeSize"></param>
        /// <returns>First available id of range</returns>
        public int ReserveRange(int rangeSize)
        {
            var result = Counter;

            lock (instance)
            {
                Counter += rangeSize;
            }

            return result;
        }

        public int GetNext()
        {
            return ReserveRange(1);
        }

        public static IdGenerator Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

