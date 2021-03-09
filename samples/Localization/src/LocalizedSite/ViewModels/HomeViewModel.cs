using System.ComponentModel.DataAnnotations;
using LocalizedSite.Resources;

namespace LocalizedSite.ViewModels
{
    public class HomeViewModel
    {
        [Required(ErrorMessage = ResourceKeys.Required)]
        [EmailAddress(ErrorMessage = ResourceKeys.NotAValidEmail)]
        [Display(Name = ResourceKeys.YourEmail)]
        public string Email { get; set; }
    }
}
