using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;

namespace PasswordControllerApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public LoginModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string Pin { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            if (!_passwordService.IsAccountSetUp())
            {
                ErrorMessage = "No account set up. Please set up first.";
            }
        }

        public IActionResult OnPost()
        {
            if (!_passwordService.IsAccountSetUp())
            {
                ErrorMessage = "No account set up.";
                return Page();
            }

            bool success = _passwordService.Login(FirstName, Password, Pin);
            if (success)
            {
                TempData["LoggedIn"] = true;  // Simple session flag
                // Redirect to Profile
                return RedirectToPage("/Profile");
            }
            else
            {
                ErrorMessage = "Login failed. Check your credentials.";
            }

            // Clear inputs (now inside the method)
            FirstName = string.Empty;
            Password = string.Empty;
            Pin = string.Empty;

            return Page();
        }
    }
}