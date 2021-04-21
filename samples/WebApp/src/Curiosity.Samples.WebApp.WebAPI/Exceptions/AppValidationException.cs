using System.Collections.Generic;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Ошибка валидации, которую не может проверить фронт
    /// </summary>
    public class AppValidationException : InvalidRequestDataException // наследуем чтоб использовать список ошибок
    {
        public override ErrorCode ErrorCode { get; } = ErrorCode.ValidationError;
        
        public AppValidationException(string message, string? key = null) : base(message, key)
        {
        }

        public AppValidationException(IReadOnlyList<InvalidRequestDataError> errors) : base(errors, "Ошибка валидации")
        {
        }
    }
}