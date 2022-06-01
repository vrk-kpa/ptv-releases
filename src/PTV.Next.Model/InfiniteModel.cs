using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class InfiniteModel<T>
    {
        public int Page { get; set; }
        public bool IsMoreAvailable { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}