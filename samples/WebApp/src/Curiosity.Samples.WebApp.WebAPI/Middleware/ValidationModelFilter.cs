using System.Collections.Generic;
using System.Threading.Tasks;
using Curiosity.Samples.WebApp.API.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Curiosity.Samples.WebApp.API.Middleware
{
    /// <summary>
    /// Фильтр, который проверяет валидна ли модель в запросе.
    /// Если не валидна, кидает InvalidRequestDataException.
    /// </summary>
    public class ValidationModelFilter : IAsyncActionFilter
    {
        /// <summary>
        /// When model is invalid return json response with error
        /// </summary>
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelState = context.ModelState;

            // если модель валидна выполняем действие
            if (modelState.IsValid)
            {
                return next();
            }

            // если не валидна - бросаем исключение (обработается в ExceptionFilterMiddleware)
            var errors = new List<InvalidRequestDataError>(modelState.Count);
            foreach (var item in modelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    errors.Add(new InvalidRequestDataError(error.ErrorMessage, item.Key));
                }
            }

            throw new InvalidRequestDataException(errors);
        }
    }
}