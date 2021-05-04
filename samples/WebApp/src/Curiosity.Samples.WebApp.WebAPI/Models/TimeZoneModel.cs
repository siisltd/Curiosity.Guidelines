using System;

namespace Curiosity.Samples.WebApp.API.Models
{
    public class TimeZoneModel
    {
        /// <summary>
        /// Id часового пояса в NodaTime
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; }

        public TimeZoneModel(string id, string name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}