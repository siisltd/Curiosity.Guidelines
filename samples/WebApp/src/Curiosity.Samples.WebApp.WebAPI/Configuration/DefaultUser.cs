namespace Curiosity.Samples.WebApp.API.Configuration
{
    /// <summary>
    /// Юзер, который создаётся первой миграцией
    /// </summary>
    public class DefaultUser
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public override string ToString()
        {
            return $"{nameof(Email)}: {Email}, " +
                   $"{nameof(Password)}: {Password}";
        }
    }
}