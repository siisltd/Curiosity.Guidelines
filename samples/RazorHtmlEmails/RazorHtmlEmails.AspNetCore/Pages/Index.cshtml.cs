using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IRegisterAccountService = RazorHtmlEmails.AspNetCore.Common.IRegisterAccountService;

namespace RazorHtmlEmails.AspNetCore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRegisterAccountService _registerAccountService;

        public IndexModel(IRegisterAccountService registerAccountService)
        {
            _registerAccountService = registerAccountService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // In reality, you would have this on a POST and pass along user input and not just have the Confirm Account link be the Index page... but #Demoware
            await _registerAccountService.Register("test@example.com", Url.Page("./Index"));
            return Page();
        }
    }
}
