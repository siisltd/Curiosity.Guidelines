namespace Curiosity.Samples.WebApp.Common
{
    public static class Constants
    {
        /// <summary>
        /// Часовой пользователя пояс по умолчанию
        /// </summary>
        public const string DefaultTimeZoneId = "Europe/Moscow";

        /// <summary>
        /// Максимальный размер реквеста в байтах 
        /// </summary>
        public const int PackageSizeLimit = 1 * 1024 * 1024 * 1024; // 1 Gb

        /// <summary>
        /// Максимальный размер загружаемого файла в байтах.
        /// </summary>
        public const int FileSizeLimit = 20 * 1024 * 1024; // 20 Mb
    }
}