using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Curiosity.Samples.WebApp.API.Tools
{
    /// <summary>
    /// Помогает получить удобную модель как результат запроса с пагинацией
    /// </summary>
    public static class PaginationHelper
    {
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize, int totalCount) where T : class
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var result = await query
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync();

            return new Page<T>(pageIndex, pageSize, totalCount, result);
        }
    }

    public class Page<T> where T : class
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public IReadOnlyList<T> Data { get; }

        public Page(
            int pageIndex,
            int pageSize,
            int totalCount,
            IReadOnlyList<T>? data)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (totalCount < 0) throw new ArgumentOutOfRangeException(nameof(totalCount));

            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data ?? Array.Empty<T>();
        }
    }
}