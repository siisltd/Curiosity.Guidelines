using System;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Код ошибки, которое может вернуть WebAPI
    /// </summary>
    public enum ErrorCode
    {
        NoErrors = 0,
        InternalServerError = 1,
        InvalidRequestData = 2,
        EntityNotFound = 3,
        AuthException = 4,
        PermissionDenied = 5,
        ValidationError = 6,
    }

    public static class ErrorCodesExtensions
    {
        /// <summary>
        /// Расшифровка для Swagger UI 
        /// </summary>
        public static string GetDescription(this ErrorCode code)
        {
            return code switch
            {
                ErrorCode.NoErrors => "Нет ошибок",
                ErrorCode.InternalServerError => "Внутренняя ошибка сервера",
                ErrorCode.InvalidRequestData => "Ошибка валидации запроса",
                ErrorCode.EntityNotFound => "Объект не найден",
                ErrorCode.AuthException => "Ошибка авторизации (пользователь удалён или заблокирован)",
                ErrorCode.PermissionDenied => "Ошибка доступа (недостаточно прав)",
                ErrorCode.ValidationError => "Ошибка валидации (бизнес логика)",
                _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Отсутствует описание")
            };
        }
    }
}