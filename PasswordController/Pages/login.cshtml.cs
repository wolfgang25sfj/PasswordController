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

        public IActionResult OnGet()
        {
            if (!_passwordService.IsAccountSetUp())
            {
                ErrorMessage = "No account set up. Please <a asp-page='/Setup'>set up first</a>.";
                return Page();
            }

            if (HttpContext.Session.GetString("LoggedIn") == "true")
            {
                return RedirectToPage("/Profile");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!_passwordService.IsAccountSetUp())
            {
                ErrorMessage = "No account set up.";
                return Page();
            }

            // Clear stale session
            if (HttpContext.Session.GetString("LoggedIn") == "true" && !_passwordService.Login(FirstName, Password, Pin))
            {
                HttpContext.Session.Clear();
            }

            bool success = _passwordService.Login(FirstName, Password, Pin);
            if (success)
            {
                HttpContext.Session.SetString("LoggedIn", "true");
                HttpContext.Session.SetString("FirstName", FirstName);
                TempData["Success"] = "Login successful!";
                return RedirectToPage("/Profile");
            }
            else
            {
                ErrorMessage = "Login failed. Verify: First name (case-insensitive), password & PIN (case-sensitive).";
                HttpContext.Session.Clear();
            }

            // Clear inputs
            FirstName = string.Empty;
            Password = string.Empty;
            Pin = string.Empty;

            return Page();
        }
    }
}