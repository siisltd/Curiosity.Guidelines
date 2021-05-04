using System.ComponentModel.DataAnnotations;

namespace Curiosity.Samples.WebApp.API.Tools
{
    public class OnlyDigitAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Разрешает только цифры в строке
        /// </summary>
        public OnlyDigitAttribute() : base(@"^[0-9]+$")
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Поле \"{name}\" должно содержать только цифры";
        }
    }
}