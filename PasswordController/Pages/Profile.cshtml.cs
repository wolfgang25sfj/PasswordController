using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;
using PasswordControllerApp.Models;

namespace PasswordControllerApp.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public ProfileModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public bool IsLoggedIn => HttpContext.Session.GetString("LoggedIn") == "true" && _passwordService.IsAccountSetUp();
        public string FirstName => HttpContext.Session.GetString("FirstName") ?? _passwordService.GetFirstName();
        public List<VaultEntry> VaultEntries => _passwordService.GetVaultEntries();  // Refreshes on call
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (!IsLoggedIn || !_passwordService.IsAccountSetUp())
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!IsLoggedIn || !_passwordService.IsAccountSetUp())
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }

            bool success = _passwordService.AddVaultEntry(Email, Password);
            if (success)
            {
                SuccessMessage = "Entry added securely!";
                ErrorMessage = null;
                Email = string.Empty;
                Password = string.Empty;
            }
            else
            {
                ErrorMessage = "Failed to add entry. Check password strength and email format.";
            }

            // Reload entries for display (since in-memory refreshed)
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}