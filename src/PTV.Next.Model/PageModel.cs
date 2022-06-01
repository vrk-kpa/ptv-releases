using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class PageModel<T>
    {
        public int Index { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}