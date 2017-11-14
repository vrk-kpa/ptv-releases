using System.Collections.Generic;
using PTV.Domain.Model.Models;

namespace PTV.Domain.Model
{
    /// <summary>
    /// Base model for sending paged data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedData<T> : VmSearchResultBase
    {
        /// <summary>
        /// List of main data
        /// </summary>
        public List<T> Data { get; set; }
    }
}