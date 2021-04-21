using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Curiosity.Samples.WebApp.API.BLL;
using Curiosity.Samples.WebApp.API.BLL.Auth;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.API.Models;
using Curiosity.Samples.WebApp.API.Models.Files;
using Curiosity.Samples.WebApp.Common;
using Curiosity.Samples.WebApp.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FilesController : Controller
    {
        private readonly ILogger<FilesController> _logger;
        private readonly DataContextFactory _contextFactory;
        private readonly AuthService _authService;

        public FilesController(ILogger<FilesController> logger, DataContextFactory contextFactory, AuthService authService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Возвращает список всех загруженных файлов
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Required
        /// </remarks>
        [HttpGet]
        [Authorize]
        public async Task<Response<FileModel[]>> GetLicenses()
        {
            using (var context = _contextFactory.CreateContext())
            {
                var files = await context.Files
                    .OrderBy(x => x.Created)
                    .Select(x => new FileModel
                    {
                        Id = x.Id,
                        Created = x.Created,
                        FileName = x.UserFileName,
                    })
                    .ToArrayAsync();

                return new Response<FileModel[]>(files);
            }
        }

        /// <summary>
        /// Загружает файлы на сервер. Максимальный размер пакета 1 Гб, файла 20 Мб
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Required
        /// </remarks>
        [HttpPost]
        [RequestSizeLimit(Constants.PackageSizeLimit)]
        [Authorize]
        public async Task<Response> UploadLicenses(
            [Required] [FromForm] [MinLength(1)] IFormFileCollection files,
            [FromServices] FileService fileService)
        {
            var currentUser = await _authService.GetCachedUserAsync(HttpContext);

            using (var context = _contextFactory.CreateContext())
            {
                // проверим, что такие файлы ещё не загружены
                var errors = new List<InvalidRequestDataError>();
                foreach (var file in files)
                {
                    var isUploaded = await context.Files
                        .Where(x => x.UserFileName == file.FileName)
                        .AnyAsync();
                    if (isUploaded)
                        errors.Add(new InvalidRequestDataError($"Файл с именем \"{file.FileName}\" уже загружен", "file"));
                }

                if (errors.Count > 0)
                    throw new InvalidRequestDataException(errors);
            }

            // загрузка
            await fileService.UploadAsync(currentUser, files);
            return new Response();
        }

        /// <summary>
        /// Скачивает файлы
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Required
        /// </remarks>
        /// <param name="ids">Id файлов</param>
        [HttpGet("download")]
        [Authorize]
        public async Task<IActionResult> DownloadLicenses(
            [FromQuery] IReadOnlyList<long> ids,
            [FromServices] FileService fileService)
        {
            var currentUser = await _authService.GetCachedUserAsync(HttpContext);

            using (var context = _contextFactory.CreateContext())
            {
                var files = await context.Files
                    .Where(x => ids.Count == 0 || ids.Contains(x.Id))
                    .AsNoTracking()
                    .ToArrayAsync();

                // скачаем и запакуем и отправим
                return await fileService.DownloadAsync(this, files, currentUser, currentUser.Email, "files");
            }
        }

        /// <summary>
        /// Удаляет файлы
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Required
        /// </remarks>
        /// <param name="ids">Список id файлов</param>
        [HttpDelete]
        [RequestSizeLimit(Constants.PackageSizeLimit)]
        [Authorize]
        public async Task<Response> DeleteLicenses(
            [FromBody] [MinLength(1)] long[] ids,
            [FromServices] FileService fileService)
        {
            var currentUser = await _authService.GetCachedUserAsync(HttpContext);

            _logger.LogDebug($"{currentUser} начал удаление {ids.Length} файлов");

            using (var context = _contextFactory.CreateContext())
            {
                var files = await context.Files
                    .Where(x => ids.Contains(x.Id))
                    .ToArrayAsync();

                await fileService.DeleteFilesAsync(context, files);

                return new Response();
            }
        }
    }
}