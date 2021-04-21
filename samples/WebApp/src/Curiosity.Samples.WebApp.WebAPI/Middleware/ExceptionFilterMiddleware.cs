using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.API.Models;
using Curiosity.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Curiosity.Samples.WebApp.API.Middleware
{
    public class ExceptionFilterMiddleware
    {
        private readonly ILogger<ExceptionFilterMiddleware> _logger;
        private readonly SensitiveDataProtector _dataProtector;
        private readonly RequestDelegate _next;

        public ExceptionFilterMiddleware(
            ILogger<ExceptionFilterMiddleware> logger, 
            SensitiveDataProtector dataProtector, 
            RequestDelegate next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProtector = dataProtector ?? throw new ArgumentNullException(nameof(dataProtector));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Ловит исключение, пишет ошибку в лог, возвращает респонс с ошибкой
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (AppException ex) // ожидаемое исключение
            {
                // log
                _logger.LogWarning(ex, $"Поймано исключение: \"{ex.GetType().Name}\" " +
                                       $"с кодом ошибки: \"{ex.IntErrorCode}\" " +
                                       $"и сообщением: \"{ex.Message}\". " +
                                       $"Для запроса: \"{Describe(context)}\""); 
                
                // write response
                var response = ex.ToErrorResponse();
                await WriteResponseAsync(context, response);
            }
            catch (Exception ex) // критическая ошибка
            {
                _logger.LogError(ex, $"Неожиданное исключение: \"{ex.Message}\" произошло во время обработки запроса: {Describe(context)}");

                // write response
                var error = new Error((int) ErrorCode.InternalServerError, "Критическая ошибка сервера");
                await WriteResponseAsync(context, new Response(error));
            }
        }

        /// <summary>
        /// Записывает API Response в HttpContext
        /// </summary>
        private async Task WriteResponseAsync(HttpContext context, Response response)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }
        
        /// <summary>
        /// Извлекает интересующие данные из запроса и описывает их
        /// </summary>
        private string Describe(HttpContext context)
        {
            var request = context.Request;
            
            request.Headers.TryGetValue("Authorization", out var token);
            
            var body = "";
            if (request.ContentType == "application/json" && request.Body.CanSeek)
            {
                using (var reader = new StreamReader(request.Body))
                {
                    request.Body.Position = 0;
                    body = reader.ReadToEnd();
                }
            }

            return $"\nPath: {request.Path} " +
                   $"\nMethod: {request.Method} " +
                   $"\nToken: {token} " +
                   $"\nContentType: {request.ContentType} " +
                   $"\nQuery: {request.QueryString} " +
                   $"\nBody: {_dataProtector.HideInJson(body)}"; // пишем запрос, но скрываем пароли
        }
    }
}