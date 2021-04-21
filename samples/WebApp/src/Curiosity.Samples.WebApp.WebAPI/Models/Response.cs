using System;
using System.Collections.Generic;
using Curiosity.Samples.WebApp.API.Tools;

namespace Curiosity.Samples.WebApp.API.Models
{
    /// <summary>
    /// Response without body.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// True if query executed without errors.
        /// </summary>
        public bool IsSuccess { get; }
        
        /// <summary>
        /// List of errors. Empty if success.
        /// </summary>
        public IReadOnlyList<Error> Errors { get; }

        /// <summary>
        /// Use when success
        /// </summary>
        public Response()
        {
            IsSuccess = true;
            Errors = Array.Empty<Error>();
        }

        /// <summary>
        /// Use when fall
        /// </summary>
        /// <param name="errors">Errors</param>
        public Response(IReadOnlyList<Error> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        /// <summary>
        /// Use when fall
        /// </summary>
        /// <param name="error">Error description</param>
        public Response(Error error)
        {
            IsSuccess = false;
            Errors = new List<Error>(1)
            {
                error
            };
        }
    }
    
    /// <summary>
    /// Response with body.
    /// </summary>
    public class Response<T> : Response
    {
        /// <summary>
        /// Body of response
        /// </summary>
        public T Body { get; }

        /// <summary>
        /// Use when success
        /// </summary>
        /// <param name="body">Body</param>
        public Response(T body) 
        {
            Body = body;
        }

        /// <summary>
        /// Use when fall
        /// </summary>
        /// <param name="errors">Errors</param>
        public Response(IReadOnlyList<Error> errors) : base(errors)
        {
            Body = default!;
        }
    }

    /// <summary>
    /// Response contains page body and total count of items
    /// </summary>
    public class PaginationResponse<T> : Response<IReadOnlyList<T>> where T: class
    {
        /// <summary>
        /// Индекс полученной страницы
        /// </summary>
        public int PageIndex { get; }
        
        /// <summary>
        /// Максимально возможное количество элементов в ответе
        /// </summary>
        public int PageSize { get; }
        
        /// <summary>
        /// Общее количество элементов в выборке
        /// </summary>
        public int TotalCount { get; }
        
        public PaginationResponse(Page<T> page) : base(page.Data)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            PageIndex = page.PageIndex;
            PageSize = page.PageSize;
            TotalCount = page.TotalCount;
        }
        
        public PaginationResponse(IReadOnlyList<T> body, int pageIndex,  int pageSize, int totalCount) : base(body)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}