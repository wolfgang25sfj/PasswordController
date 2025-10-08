using Microsoft.AspNetCore.Mvc;  // For IActionResult, BindProperty
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;
using PasswordControllerApp.Models;  // For VaultEntry
using System.ComponentModel.DataAnnotations;  // For BindProperty if needed

namespace PasswordControllerApp.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public ProfileModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public bool IsLoggedIn => TempData["LoggedIn"]?.ToString() == "true" && _passwordService.IsAccountSetUp();

        // Fixed: Access FirstName properly (add a service method if needed; placeholder here)
        public string? FirstName => "User";  // TODO: Expose from service for real use

        public List<VaultEntry> VaultEntries => _passwordService.GetVaultEntries();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
            if (!IsLoggedIn)
            {
                return;
            }
        }

        public IActionResult OnPost()
        {
            if (!IsLoggedIn)
            {
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

            return Page();
        }
    }
}