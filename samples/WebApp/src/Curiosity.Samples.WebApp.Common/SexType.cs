using System;

namespace Curiosity.Samples.WebApp.Common
{
    public enum SexType
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }
    
    public static class SexTypeExtensions
    {
        /// <summary>
        /// Расшифровка для Swagger UI 
        /// </summary>
        public static string GetDescription(this SexType type)
        {
            return type switch
            {
                SexType.Unknown => "Не известный",
                SexType.Male => "Мужской",
                SexType.Female => "Женский",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Отсутствует описание")
            };
        }
    }
}