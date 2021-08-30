using System;
using System.Collections.Generic;

namespace Lru.Logic
{
    public class LRU
    {
        public const int DEFAULT_LIMIT = 10;
        public List<object> Recent { get; private set; }
        public int ListLimit { get; private set; }
        public LRU(int limit = DEFAULT_LIMIT)
        {
            Recent = new List<object>();
            ListLimit = limit;
        }
        public void Add(object subject) // SUT = System Under Test
        {
            Recent.Add(subject); // 1 - Initial version
        }
    }
}