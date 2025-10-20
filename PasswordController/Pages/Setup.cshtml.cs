using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;
using System.Linq;  // For LINQ

namespace PasswordControllerApp.Pages
{
    public class SetupModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public SetupModel(IPasswordService passwordService)
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
        public string? NameMessage { get; set; }
        public string? PasswordMessage { get; set; }
        public string? PinMessage { get; set; }

        public void OnGet()
        {
            if (_passwordService.IsAccountSetUp())
            {
                ErrorMessage = "Account already set up. Please login or reset.";
            }
        }

        public IActionResult OnPost()
        {
            if (_passwordService.IsAccountSetUp())
            {
                ErrorMessage = "Account already set up.";
                return Page();
            }

            // Validate first name (unchanged)
            var nameResult = _passwordService.ValidateFirstName(FirstName);
            NameMessage = nameResult.Message;
            if (!nameResult.IsValid)
            {
                return Page();
            }

            // Validate password (updated for new tuple)
            var pwResult = _passwordService.ValidatePassword(Password);
            PasswordMessage = pwResult.IsValid ? "Password is secure." : string.Join("; ", pwResult.Messages);
            if (!pwResult.IsValid)
            {
                return Page();
            }

            // Validate PIN (unchanged)
            var pinResult = _passwordService.ValidatePin(Pin);
            PinMessage = pinResult.Message;
            if (!pinResult.IsValid)
            {
                return Page();
            }

            bool success = _passwordService.SetupAccount(FirstName, Password, Pin);
            if (success)
            {
                // Set session for auto-login after setup
                HttpContext.Session.SetString("LoggedIn", "true");
                HttpContext.Session.SetString("FirstName", FirstName);
                SuccessMessage = "Account set up successfully! Redirecting to profile...";
                ErrorMessage = null;
                // Clear inputs
                FirstName = string.Empty;
                Password = string.Empty;
                Pin = string.Empty;
                return RedirectToPage("/Profile");  // Auto-redirect to Profile
            }
            else
            {
                ErrorMessage = "Setup failed.";
            }

            return Page();
        }
    }
}