using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Curiosity.Archiver;
using Curiosity.Samples.WebApp.API.BLL.Auth;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.Common;
using Curiosity.Samples.WebApp.DAL;
using Curiosity.Samples.WebApp.DAL.Entities;
using Curiosity.TimeZone;
using Curiosity.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeTypes;

namespace Curiosity.Samples.WebApp.API.BLL
{
    /// <summary>
    /// Сервис для работы с файлами.
    /// В данном примере мы работаем с локальным хранилищем (в оригинале используем Curiosity.SftpClient)
    /// Код, который проверяет директории и наличие файлов, лучше вынести в отдельный класс (например написать свою реализацию для Curiosity.SftpClient)
    /// Для примера весь код оставим здесь :)
    /// </summary>
    public class FileService
    {
        private const int SizeLimit = Constants.FileSizeLimit;
        private readonly string _tempPath;
        private const string StoragePath = "/storage"; // вообще для этого надо использовать конфигурацию, но для примера оставим так 
        private const string FormatPattern = @"(\.png|\.jpg|\.jpeg|\.pdf)$";
        private static readonly Regex FormatRegex = new Regex(FormatPattern, RegexOptions.IgnoreCase);
        private static readonly TimeZoneHelper TimeHelper = new TimeZoneHelper();

        private readonly ILogger<FileService> _logger;
        private readonly DataContextFactory _contextFactory;
        private readonly IArchiver _archiver;
        private readonly IDateTimeService _dateTimeService;
        
        public FileService(
            ILogger<FileService> logger,
            DataContextFactory contextFactory,
            IArchiver archiver,
            IDateTimeService dateTimeService,
            AppConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _archiver = archiver ?? throw new ArgumentNullException(nameof(archiver));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));

            _tempPath = configuration?.TempFiles.TempPath ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task UploadAsync(UserInfo currentUser, IFormFileCollection files)
        {
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));
            if (files == null) throw new ArgumentNullException(nameof(files));
            _logger.LogDebug($"{currentUser} начал загрузку {files.Count} файлов");

            Validate(files);

            using (var context = _contextFactory.CreateContext())
            {
                foreach (var file in files)
                {
                    var userFileName = Transliteration.Front(file.FileName);
                    var fileName = GetUniqueFileName();
                    var fileBasePath = Path.Combine(StoragePath, $"u{currentUser.Id}");

                    _logger.LogInformation($"Сохраняем файл в локальное хранилище (userFileName = \"{userFileName}\", fileName = \"{fileName}\", filePath = \"{fileBasePath}\")");
                    
                    // здесь может быть запись файлов на SFTP сервер
                    
                    // проверим локальную директорию
                    if (!Directory.Exists(fileBasePath))
                    {
                        Directory.CreateDirectory(fileBasePath);
                    }
                    
                    // сохранение
                    using (var fileStream = File.Create(Path.Combine(fileBasePath, fileName)))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    
                    _logger.LogInformation("Файл успешно сохранен в хранилище");

                    var entity = new FileEntity
                    {
                        UserFileName = userFileName,
                        StorageFilePath = fileBasePath,
                        StorageFileName = fileName,
                        UserId = currentUser.Id,
                    };

                    context.Files.Add(entity);
                    await context.SaveChangesAsync(); // сохраняем при каждой итерации, потому что файл уже на сервере

                    _logger.LogInformation($"Сохранили сущность в базу (id = {entity.Id})");
                }
            }

            _logger.LogDebug("Все файлы успешно загружены");
        }

        public async Task<IActionResult> DownloadAsync(Controller controller, IReadOnlyList<FileEntity> entities, UserInfo user, string baseName, string suffix)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (String.IsNullOrWhiteSpace(baseName)) throw new ArgumentNullException(nameof(baseName));
            _logger.LogDebug($"{user} начал скачивание {entities.Count} файлов");
            
            // здесь может быть скачивание файлов с SFTP сервера во временную директорию
            var tempFiles = entities
                .Select(x => new FileNames(Path.Combine(x.StorageFilePath, x.StorageFileName), x.UserFileName))
                .Where(x => File.Exists(x.StorageFileName)) // отфильтруем существующие
                .ToArray();

            if (tempFiles.Length < 1)
            {
                throw new EntityNotFoundException("Файлы не были найдены. Возможно они уже удалены");
            }

            // если файл 1, то не надо архивировать
            if (tempFiles.Length == 1)
            {
                return SendFile(controller, tempFiles.First());
            }

            // архивируем
            _logger.LogDebug("Архивируем файлы");
            var archiveStorageName = await _archiver.ZipFilesToFileAsync(tempFiles);

            var archiveUserName = GetArchiveName(user, baseName, suffix);
            var fileData = new FileNames(archiveStorageName, archiveUserName);
            _logger.LogInformation($"Создан временный архив: \"{archiveStorageName}\"");

            // удаляем временные файлы, если скачали их с SFTP сервера
            // foreach (var tempFile in tempFiles)
            // {
            //     File.Delete(tempFile.StorageFileName);
            // }

            _logger.LogInformation($"Удалено {tempFiles.Length} временных файлов");

            return SendFile(controller, fileData);
        }

        /// <summary>
        /// Получает content-type файла.
        /// Добавляет хедер для фрона.
        /// Возвращает PhysicalFileResult.
        /// </summary>
        public static IActionResult SendFile(Controller controller, FileNames file)
        {
            var type = MimeTypeMap.GetMimeType(Path.GetExtension(file.UserFileName));

            // чтоб фронт понимал имя файла
            controller.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return controller.PhysicalFile(file.StorageFileName, type, file.UserFileName);
        }

        /// <summary>
        /// Удаляет файл из хранилища и из базы.
        /// Сохраняет контекст
        /// </summary>
        public async Task DeleteFilesAsync(DataContext context, IReadOnlyList<FileEntity> files)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (files == null) throw new ArgumentNullException(nameof(files));

            _logger.LogDebug($"Начинаем удаление {files.Count} файлов");
            foreach (var file in files)
            {
                // здесь может быть удаление файлов с SFTP сервера
                File.Delete(Path.Combine(file.StorageFilePath, file.StorageFileName));
                context.Remove(file);

                // сохраняем при каждой итерации, потому что файл реально уже удалён
                await context.SaveChangesAsync();
                _logger.LogInformation($"Файл (Path = \"{file.StorageFilePath}\", Name = \"{file.StorageFileName}\") успешно удалён");
            }

            _logger.LogDebug("Все файлы успешно удалены");
        }

        private void Validate(IFormFileCollection files)
        {
            const string key = "file";
            var needFormatMessage = false;

            var names = new HashSet<string>();
            var errors = new List<InvalidRequestDataError>();
            foreach (var file in files)
            {
                // null
                if (file == null)
                    throw new InvalidRequestDataException("Загружаемый файл не может быть null");

                // size
                if (file.Length > SizeLimit)
                    errors.Add(new InvalidRequestDataError($"Превышен размер файла \"{file.FileName}\". Максимально допустимый размер {SizeLimit / 1024 / 1024} Мб.", key));

                // empty
                if (file.Length <= 0)
                    errors.Add(new InvalidRequestDataError($"Файл \"{file.FileName}\" пустой.", key));

                // name
                if (Transliteration.Front(file.FileName).Length > 200)
                    errors.Add(new InvalidRequestDataError($"Превышен максимальный размер имени файла \"{file.FileName}\"", key));

                // format
                if (!FormatRegex.IsMatch(file.FileName))
                {
                    errors.Add(new InvalidRequestDataError($"Неверный формат файла: \"{file.FileName}\"", key));
                    needFormatMessage = true;
                }

                // duplicates
                if (names.Contains(file.FileName))
                {
                    errors.Add(new InvalidRequestDataError($"Невозможно загрузить файлы с одинаковыми именами \"{file.FileName}\"", key));
                }
                else
                {
                    names.Add(file.FileName);
                }
            }

            if (errors.Count > 0)
            {
                // выводим список разрешённых расширений
                if (needFormatMessage)
                {
                    var formats = Regex.Replace(FormatPattern, @"(\\|\(|\)|\$|\|)", " ");
                    errors.Add(new InvalidRequestDataError($"Поддерживаемые форматы файлов: {formats}", key));
                }

                throw new InvalidRequestDataException(errors);
            }
        }

        private string GetArchiveName(UserInfo? user, string baseName, string suffix)
        {
            return $"{Clean(baseName)}.{suffix}.{GetTime(user)}.zip";
        }

        /// <summary>
        /// Вычищаем из строки лишние символы - что бы не попали недопустимые для имени файла
        /// </summary>
        private static string Clean(string str)
        {
            return Regex.Replace(str, @"[^a-zA-Zа-яА-Я0-9().]+", "_");
        }

        /// <summary>
        /// Возвращает текущее время для юзера в спец формате
        /// </summary>
        private string GetTime(UserInfo? user)
        {
            var timeZone = user?.TimeZoneId ?? Constants.DefaultTimeZoneId;
            return TimeHelper.ToClientTime(timeZone, _dateTimeService.GetCurrentTimeUtc()).ToString("dd.MM.yy-HH.mm");
        }

        private string GetTempFileFullName()
        {
            return Path.Combine(_tempPath, GetUniqueFileName());
        }

        private static string GetUniqueFileName()
        {
            return $"{Guid.NewGuid():N}.file";
        }
    }
}