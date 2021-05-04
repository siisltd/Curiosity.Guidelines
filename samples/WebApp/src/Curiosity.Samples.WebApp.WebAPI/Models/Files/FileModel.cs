using System;

namespace Curiosity.Samples.WebApp.API.Models.Files
{
    public class FileModel
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public string FileName { get; set; } = null!;
    }
}