using PasswordControllerApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;  // For Any()

namespace PasswordControllerApp.Services
{
    public class PasswordService : IPasswordService
    {
        private static StoredData? _storedData;

        public bool IsAccountSetUp()
        {
            return _storedData != null;
        }

        public bool SetupAccount(string firstName, string password, string pin)
        {
            if (IsAccountSetUp())
            {
                return false; // Already set up
            }

            var pwValid = ValidatePassword(password);
            if (!pwValid.IsValid) return false;

            var pinValid = ValidatePin(pin);
            if (!pinValid.IsValid) return false;

            var nameValid = ValidateFirstName(firstName);
            if (!nameValid.IsValid) return false;

            _storedData = new StoredData
            {
                FirstName = firstName,
                PasswordHash = HashString(password),
                PinHash = HashString(pin)
            };

            return true;
        }

        public bool Login(string firstName, string password, string pin)
        {
            if (!IsAccountSetUp())
            {
                return false;
            }

            if (firstName.ToLower() != _storedData!.FirstName!.ToLower())
            {
                return false;
            }

            if (HashString(pin) != _storedData.PinHash)
            {
                return false;
            }

            if (HashString(password) != _storedData.PasswordHash)
            {
                return false;
            }

            return true;
        }

        public void ResetAccount()
        {
            _storedData = null;
        }

        public (bool IsValid, List<string> Messages, StrengthLevel Level) ValidatePassword(string password)
        {
            var messages = new List<string>();
            var level = StrengthLevel.Weak;
            int criteriaMet = 0;  // For scoring

            if (string.IsNullOrWhiteSpace(password))
            {
                messages.Add("Password cannot be empty.");
            }
            else
            {
                // Length check
                if (password.Length < 8)
                {
                    messages.Add("Password must be at least 8 characters long.");
                }
                else
                {
                    criteriaMet++;
                }

                // Number check
                bool hasNumber = password.Any(char.IsDigit);
                if (!hasNumber)
                {
                    messages.Add("Password must contain at least one number.");
                }
                else
                {
                    criteriaMet++;
                }

                // Special check
                bool hasSpecial = password.Any(IsSpecialCharacter);
                if (!hasSpecial)
                {
                    messages.Add("Password must contain at least one special character (!@#$%^&*(),.?\":{}|<>)");
                }
                else
                {
                    criteriaMet++;
                }

                // Score level with if-else
                if (criteriaMet == 3 && password.Length >= 12)  // Extra for longer
                {
                    level = StrengthLevel.Strong;
                }
                else if (criteriaMet >= 2)
                {
                    level = StrengthLevel.Medium;
                }
                else
                {
                    level = StrengthLevel.Weak;
                }
            }

            bool isValid = messages.Count == 0;
            return (isValid, messages, level);
        }

        public (bool IsValid, string Message) ValidatePin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
            {
                return (false, "PIN cannot be empty.");
            }

            if (!pin.All(char.IsDigit))
            {
                return (false, "PIN must contain only digits.");
            }

            if (pin.Length < 4 || pin.Length > 16)
            {
                return (false, "PIN must be 4-16 digits long.");
            }

            return (true, "PIN is valid.");
        }

        public (bool IsValid, string Message) ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return (false, "First name cannot be empty.");
            }

            if (!firstName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                return (false, "First name must contain only letters and spaces.");
            }

            return (true, "First name is valid.");
        }

        private bool IsSpecialCharacter(char c)
        {
            string specialChars = "!@#$%^&*(),.?\":{}|<>";
            return specialChars.Contains(c);
        }

        public string HashString(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToHexString(bytes);
            }
        }

        // Vault methods
        public bool AddVaultEntry(string email, string password)
        {
            if (!IsAccountSetUp()) return false;

            var pwValid = ValidatePassword(password);
            if (!pwValid.IsValid) return false;

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                return false;  // Simple email check
            }

            _storedData!.VaultEntries.Add(new VaultEntry
            {
                Email = email,
                PasswordHash = HashString(password)
            });

            return true;
        }

        public List<VaultEntry> GetVaultEntries()
        {
            return IsAccountSetUp() ? _storedData!.VaultEntries : new List<VaultEntry>();
        }

        // Generate strong password
        public string GenerateStrongPassword(int length = 16)
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specials = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var allChars = lowercase + uppercase + numbers + specials;
            var password = new char[length];
            var random = RandomNumberGenerator.Create();

            // Fill randomly
            for (int i = 0; i < length; i++)
            {
                var randomBytes = new byte[1];
                random.GetBytes(randomBytes);
                password[i] = allChars[randomBytes[0] % allChars.Length];
            }

            // Force one of each type at start positions
            var buffer = new byte[1];
            random.GetBytes(buffer);
            password[0] = uppercase[buffer[0] % uppercase.Length];
            random.GetBytes(buffer);
            password[1] = lowercase[buffer[0] % lowercase.Length];
            random.GetBytes(buffer);
            password[2] = numbers[buffer[0] % numbers.Length];
            random.GetBytes(buffer);
            password[3] = specials[buffer[0] % specials.Length];

            return new string(password);
        }

        public string GetFirstName()
        {
            return IsAccountSetUp() ? _storedData!.FirstName ?? "User" : "User";
        }
    }
}