using System;
using Microsoft.Extensions.Localization;

namespace LocalisedLib
{
    public class LocalizedService : ILocalisedService
    {
        private readonly IStringLocalizer<LocalizedService> _localizer;
        private readonly IStringLocalizer<SharedResources> _shared;

        public LocalizedService(
            IStringLocalizer<LocalizedService> localizer, 
            IStringLocalizer<SharedResources> shared)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _shared = shared ?? throw new ArgumentNullException(nameof(shared));
        }

        public string GetSharedLocalizedString()
        {
            return _shared[ResourceKeys.SharedResourceKey];
        }

        public string GetPrivateLocalizedString()
        {
            return _localizer[ResourceKeys.PrivateResourceKey];
        }
    }
}