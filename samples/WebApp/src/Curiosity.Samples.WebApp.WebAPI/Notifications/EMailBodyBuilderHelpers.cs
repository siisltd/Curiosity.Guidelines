using System;
using System.Collections.Generic;
using System.Text;

namespace Curiosity.Samples.WebApp.API.Notifications
{
    /// <summary>
    /// Помогает собирать текст письма в HTML формате
    /// </summary>
    public static class EMailBodyBuilderHelpers
    {
        public const string SenderName = "Curiosity sample app";

        public static StringBuilder AppendEMailSignature(this StringBuilder stringBuilder)
        {
            if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));

            stringBuilder.AppendLine("<p>--</p>");
            stringBuilder.AppendLine($"<p>С уважением, {SenderName}</p>");
            stringBuilder.AppendNewLine();
            stringBuilder.AppendEMailAutomaticSendSignature();

            return stringBuilder;
        }

        public static StringBuilder AppendNewLine(this StringBuilder stringBuilder)
        {
            return stringBuilder.AppendLine("<p></p>");
        }

        public static StringBuilder AppendEMailAutomaticSendSignature(this StringBuilder stringBuilder)
        {
            if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));

            stringBuilder.AppendLine("<p>Письмо отправлено автоматически. Пожалуйста, не отвечайте на него</p>");

            return stringBuilder;
        }

        public static StringBuilder AppendList<T>(this StringBuilder stringBuilder, IEnumerable<T> items)
        {
            stringBuilder.AppendLine("<ul>");
            foreach (var item in items)
            {
                stringBuilder.AppendLine($"<li>{item}</li>");
            }

            stringBuilder.AppendLine("</ul>");

            return stringBuilder;
        }
    }
}