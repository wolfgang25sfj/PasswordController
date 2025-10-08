using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;

namespace PasswordControllerApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public IndexModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public bool IsAccountSetUp => _passwordService.IsAccountSetUp();
    }
}