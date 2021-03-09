using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalisedLib;
using LocalizedSite.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ResourceKeys = LocalizedSite.Resources.ResourceKeys;

namespace LocalizedSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly ILocalisedService _localisedService;

        public HomeController(
            IStringLocalizer<HomeController> localizer, 
            ILocalisedService localisedService)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _localisedService = localisedService ?? throw new ArgumentNullException(nameof(localisedService));
        }

        public IActionResult Index()
        {
            return View(new HomeViewModel());
        }

        [HttpPost]
        public IActionResult Index(HomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ViewData["Result"] = _localizer[ResourceKeys.HomeController_SuccessMessage];
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = _localizer[ResourceKeys.HomeController_AboutContent];
            ViewData["Shared"] = _localisedService.GetSharedLocalizedString();
            ViewData["Private"] = _localisedService.GetPrivateLocalizedString();
                
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = _localizer[ResourceKeys.HomeController_ContactContent];

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
