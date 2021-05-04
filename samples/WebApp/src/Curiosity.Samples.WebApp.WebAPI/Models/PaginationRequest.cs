using System.ComponentModel.DataAnnotations;

namespace Curiosity.Samples.WebApp.API.Models
{
    /// <summary>
    /// Pagination request.
    /// </summary>
    public class PaginationRequest
    {
        /// <summary>
        /// Индекс страницы - начинается с 0.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int PageIndex { get; set; }

        /// <summary>
        /// Размер страницы - по умолчанию = 25.
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 25;
    }
}