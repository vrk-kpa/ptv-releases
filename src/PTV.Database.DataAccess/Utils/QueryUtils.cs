using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Framework;

namespace PTV.Database.DataAccess.Utils
{
    public static class QueryUtils
    {
        public static PagedData<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber)
        {
            var data = query.Skip(pageNumber.PositiveOrZero() * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems + 1).ToList();
            return new PagedData<T>()
            {
                Data = data.Take(CoreConstants.MaximumNumberOfAllItems).ToList(),
                MoreAvailable = data.Count > CoreConstants.MaximumNumberOfAllItems
            };
        }
    }

    public class PagedData<T>
    {
        public List<T> Data { get; set; }
        public bool MoreAvailable { get; set; }
    }
}
