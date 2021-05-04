using System;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Доступ к данному действию запрещен.
    /// По-хорошему фронт не должен допускать таких ошибок,
    /// но пользователь может её увидеть, если права изменили, а фронт не успел обновиться.
    /// </summary>
    public class PermissionException : AppException
    {
        public override ErrorCode ErrorCode { get; } = ErrorCode.PermissionDenied;

        public PermissionException(string message) : base(message)
        {
        }

        public PermissionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}