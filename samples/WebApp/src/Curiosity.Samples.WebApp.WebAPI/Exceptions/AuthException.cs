using System;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Ошибка авторизации, например пользователь заблокирован
    /// </summary>
    public class AuthException : AppException
    {
        public override ErrorCode ErrorCode { get; } = ErrorCode.AuthException;

        public AuthException(string message) : base(message)
        {
        }

        public AuthException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}