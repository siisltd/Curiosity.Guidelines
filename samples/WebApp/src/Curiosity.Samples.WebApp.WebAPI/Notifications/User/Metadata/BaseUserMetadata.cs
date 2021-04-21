using System;

namespace Curiosity.Samples.WebApp.API.Notifications.User.Metadata
{
    public class BaseUserMetadata
    {
        public string Email { get; }
        public string Token { get; }

        public BaseUserMetadata(string email, string token)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            if (String.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
            
            Email = email;
            Token = token;
        }
    }
}