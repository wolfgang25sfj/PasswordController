using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;

namespace PasswordControllerApp.Pages
{
    public class ResetModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public ResetModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public IActionResult OnGet()
        {
            if (!_passwordService.IsAccountSetUp())
            {
                TempData["Error"] = "No account to reset.";
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            _passwordService.ResetAccount();
            TempData["Success"] = "Account reset successfully.";
            return RedirectToPage("/Index");
        }
    }
}