using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordControllerApp.Services;
using PasswordControllerApp.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace PasswordControllerApp.Pages
{
    public class TestPasswordModel : PageModel
    {
        private readonly IPasswordService _passwordService;
        private readonly ILogger<TestPasswordModel> _logger;

        public TestPasswordModel(IPasswordService passwordService, ILogger<TestPasswordModel> logger)
        {
            _passwordService = passwordService;
            _logger = logger;
        }

        [BindProperty]
        public string TestPassword { get; set; } = string.Empty;

        public bool IsValid { get; set; }
        public List<string> TestMessages { get; set; } = new List<string>();
        public StrengthLevel Strength { get; set; }
        public string GeneratedPassword { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public void OnPost()
        {
            _logger.LogInformation("OnPost called with TestPassword length: {Length}", TestPassword?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(TestPassword))
            {
                TestMessages.Add("Input appears empty—check form and try again.");
                IsValid = false;
                Strength = StrengthLevel.Weak;
                return;
            }

            var result = _passwordService.ValidatePassword(TestPassword);
            IsValid = result.IsValid;
            TestMessages = result.Messages;
            Strength = result.Level;
        }

        public IActionResult OnPostGenerate()  // Changed to POST
        {
            _logger.LogInformation("Generate called");
            var generated = _passwordService.GenerateStrongPassword();
            return Content(generated);
        }
    }
}