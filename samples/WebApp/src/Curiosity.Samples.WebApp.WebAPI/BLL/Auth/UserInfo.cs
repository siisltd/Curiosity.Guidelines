using Curiosity.Samples.WebApp.Common;

namespace Curiosity.Samples.WebApp.API.BLL.Auth
{
    public class UserInfo
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public SexType Sex { get; set; }
        public string TimeZoneId { get; set; } = null!;

        public override string ToString()
        {
            return $"Пользователь ({nameof(Id)}: {Id}, {nameof(Email)}: \"{Email}\")";
        }
    }
}