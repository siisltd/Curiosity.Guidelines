using Curiosity.Samples.WebApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Curiosity.Samples.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TimeZonesController : Controller
    {
        private static readonly TimeZoneModel[] TimeZones = new []
        {
            new TimeZoneModel("Europe/Kaliningrad", "Калининград (МСК-01)"),
            new TimeZoneModel("Europe/Moscow", "Москва (МСК)"),
            new TimeZoneModel("Europe/Astrakhan", "Астрахань (МСК+01)"),
            new TimeZoneModel("Europe/Samara", "Самара (МСК+01)"),
            new TimeZoneModel("Europe/Saratov", "Саратов (МСК+01)"),
            new TimeZoneModel("Europe/Ulyanovsk", "Ульяновск (МСК+01)"),
            new TimeZoneModel("Asia/Yekaterinburg", "Екатеринбург (МСК+02)"),
            new TimeZoneModel("Asia/Omsk", "Омск (МСК+03)"),
            new TimeZoneModel("Asia/Barnaul", "Барнаул (МСК+04)"),
            new TimeZoneModel("Asia/Krasnoyarsk", "Красноярск (МСК+04)"),
            new TimeZoneModel("Asia/Novokuznetsk", "Новокузнецк (МСК+04)"),
            new TimeZoneModel("Asia/Novosibirsk", "Новосибирск (МСК+04)"),
            new TimeZoneModel("Asia/Tomsk", "Томск (МСК+04)"),
            new TimeZoneModel("Asia/Irkutsk", "Иркутск (МСК+05)"),
            new TimeZoneModel("Asia/Chita", "Чита (МСК+06)"),
            new TimeZoneModel("Asia/Yakutsk", "Якутск (МСК+06)"),
            new TimeZoneModel("Asia/Vladivostok", "Владивосток (МСК+07)"),
            new TimeZoneModel("Asia/Magadan", "Магадан (МСК+08)"),
            new TimeZoneModel("Asia/Sakhalin", "Сахалин (МСК+08)"),
            new TimeZoneModel("Asia/Anadyr", "Анадырь (МСК+09)"),
            new TimeZoneModel("Asia/Kamchatka", "Петропавловск-Камчатский (МСК+09)"),
        };
        
        /// <summary>
        /// Возвращает список российских часовых поясов
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Allow anonymous
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public Response<TimeZoneModel[]> GetTimeZones()
        {
            return new Response<TimeZoneModel[]>(TimeZones);
        }
    }
}