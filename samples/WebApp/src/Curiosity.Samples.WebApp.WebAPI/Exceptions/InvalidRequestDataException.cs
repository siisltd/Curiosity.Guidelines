using System.Collections.Generic;
using System.Linq;
using Curiosity.Samples.WebApp.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Ошибка валидации модели.
    /// По-хорошему фронт не должен допускать таких ошибок - должен валидировать модель до запроса,
    /// но тем не менее стоит учитывать, что пользователь вполне может видеть такую ошибку - это почти нормально.
    /// </summary>
    public class InvalidRequestDataException : AppException
    {
        public override ErrorCode ErrorCode { get; } = ErrorCode.InvalidRequestData;

        public IReadOnlyList<Error> Errors { get; }

        public InvalidRequestDataException(string message, string? key = null) : base(message)
        {
            Errors = new []
            {
                new Error(IntErrorCode, message, key), 
            };
        }

        public InvalidRequestDataException(IReadOnlyList<InvalidRequestDataError> errors, string? errorMessage = null) : base(errorMessage ?? "Параметры запроса не валидны")
        {
            Errors = errors.Select(e => new Error(IntErrorCode, e.Message, e.Key)).ToArray();
        }
        
        public InvalidRequestDataException(IEnumerable<IdentityError> errors) : base("Параметры запроса не валидны")
        {
            Errors = errors.Select(e => new Error(IntErrorCode, e.Description, e.Code)).ToArray();
        }

        public override Response ToErrorResponse()
        {
            return new Response(Errors);
        }
    }
    
    public class InvalidRequestDataError
    {
        public string Message { get; }
        
        public string Key { get; }

        public InvalidRequestDataError(string message, string key)
        {
            Message = message;
            Key = key;
        }
    }
}