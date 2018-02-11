using System;
using System.Collections.Generic;

namespace Framework.Collections.Generic
{
    public class PaginatedList<T> : List<T>
    {

        #region read Only Properties

        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        #endregion

        #region Constructor

        public PaginatedList(IEnumerable<T> source, int pageNumber, int pageSize, int totalCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int) Math.Ceiling(totalCount / (double) pageSize);

            AddRange(source);

        }

        #endregion
    }
}