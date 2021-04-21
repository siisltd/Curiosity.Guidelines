using System;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Сущность не найдена.
    /// Сущность по запрашиваемому Id не найдена.
    /// Может быть если с фронта пришёл не правильный Id или сущность успели удалить
    /// </summary>
    public class EntityNotFoundException : AppException
    {
        public override ErrorCode ErrorCode { get; } = ErrorCode.EntityNotFound;

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}