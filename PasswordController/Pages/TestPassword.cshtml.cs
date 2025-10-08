using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;

namespace PasswordControllerApp.Pages
{
    public class TestPasswordModel : PageModel
    {
        private readonly IPasswordService _passwordService;

        public TestPasswordModel(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [BindProperty]
        public string TestPassword { get; set; } = string.Empty;

        public bool IsValid { get; set; }
        public string TestResult { get; set; } = string.Empty;
        public string GeneratedPassword { get; set; } = string.Empty;

        public void OnGet()
        {
            GeneratedPassword = _passwordService.GenerateStrongPassword();  // Pre-generate for demo
        }

        public void OnPost()
        {
            var result = _passwordService.ValidatePassword(TestPassword);
            IsValid = result.IsValid;
            TestResult = result.Message;
        }

        public IActionResult OnGetGenerate()
        {
            var generated = _passwordService.GenerateStrongPassword();
            return Content(generated);
        }
    }
}