using System;
using System.Linq;
using System.Text;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.Common;

namespace Curiosity.Samples.WebApp.API.Startup
{
    /// <summary>
    /// Create a description for API
    /// </summary>
    public static class ApiDescriptionBuilder
    {
        /// <summary>
        /// Builds a description
        /// </summary>
        public static string BuildDescription()
        {
            var sb = new StringBuilder();

            // write error list 
            sb.AppendLine("Коды ошибок:");
            foreach (var code in Enum.GetValues(typeof(ErrorCode)).Cast<ErrorCode>())
            {
                sb.AppendLine($"- [{(int) code}] - {code.GetDescription()}");
            }
            
            sb.AppendLine();

            // write enums
            sb.AppendLine("Расшифровка значений enums:");
            sb.WriteEnum<SexType>(x => x.GetDescription());
            
            return sb.ToString();
        }
        
        private static void WriteEnum<T>(this StringBuilder sb, Func<T, string> describer) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T)} must be enum");

            sb.AppendLine();
            sb.AppendLine($"{typeof(T).Name}:");
            
            foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
            {
                sb.AppendLine($"- [{Convert.ToInt32(value)}] - {describer.Invoke(value)}");
            }
        }
    }
}