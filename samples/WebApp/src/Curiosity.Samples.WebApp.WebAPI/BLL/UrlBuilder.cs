using System;
using System.Web;
using Curiosity.Samples.WebApp.API.Configuration;

namespace Curiosity.Samples.WebApp.API.BLL
{
    public class UrlBuilder
    {
        private readonly string _baseUrl;

        public UrlBuilder(AppConfiguration configuration)
        {
            _baseUrl = configuration.ExternalUrls.WebSite;
        }

        public string ResetPassword(string email, string token)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            if (String.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
            
            return $"{_baseUrl}/auth/password-confirm?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}";
        }
    }
}