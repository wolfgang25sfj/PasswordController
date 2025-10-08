using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;

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

            // Validate using if-else in service, but display messages
            var nameResult = _passwordService.ValidateFirstName(FirstName);
            NameMessage = nameResult.Message;
            if (!nameResult.IsValid)
            {
                return Page();
            }

            var pwResult = _passwordService.ValidatePassword(Password);
            PasswordMessage = pwResult.Message;
            if (!pwResult.IsValid)
            {
                return Page();
            }

            var pinResult = _passwordService.ValidatePin(Pin);
            PinMessage = pinResult.Message;
            if (!pinResult.IsValid)
            {
                return Page();
            }

            bool success = _passwordService.SetupAccount(FirstName, Password, Pin);
            if (success)
            {
                SuccessMessage = "Account set up successfully!";
                ErrorMessage = null;
                // Clear inputs for security
                FirstName = string.Empty;
                Password = string.Empty;
                Pin = string.Empty;
            }
            else
            {
                ErrorMessage = "Setup failed.";
            }

            return Page();
        }
    }
}