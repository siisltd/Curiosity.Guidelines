using System;
using Curiosity.Samples.WebApp.API.Models;

namespace Curiosity.Samples.WebApp.API.Exceptions
{
    /// <summary>
    /// Базовое исключение для ошибок бизнес логики
    /// </summary>
    public abstract class AppException : Exception
    {
        public abstract ErrorCode ErrorCode { get; }
        
        public int IntErrorCode => (int) ErrorCode;

        public AppException(string message) : base(message) {}
        
        public AppException(string message, Exception inner) : base(message, inner) {}
        
        public virtual Response ToErrorResponse()
        {
            return new Response(new Error(IntErrorCode, Message));
        } 
    }
}