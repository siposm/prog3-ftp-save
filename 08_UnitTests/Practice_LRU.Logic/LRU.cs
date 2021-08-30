using System;
using System.Collections.Generic;

namespace Practice_LRU.Logic
{
    // NOTE: commented lines are only added 
    // AFTER THE suitable unit test
    public class LRU
    {
        public const int DEFAULT_LIMIT = 10;
        public List<object> Recent { get; private set; }
        public int ListLimit { get; private set; }
        public LRU(int limit = DEFAULT_LIMIT)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Must be positive"); // 2.
            Recent = new List<object>();
            ListLimit = limit;
        }
        public void Add(object subject) // SUT = System Under Test
        {
            if (subject == null) // 3.
            {
                throw new ArgumentNullException(nameof(subject)); // 3.
            }
            if (Recent.Contains(subject)) Recent.Remove(subject); // 5.
            // Recent.Add(subject); // 1 - Initial version
            Recent.Insert(0, subject); // 6.
            if (Recent.Count > ListLimit) // 4.
            {
                // Recent.RemoveAt(0); // 4.
                Recent.RemoveAt(Recent.Count - 1); // 7.
            }
        }
    }

}
