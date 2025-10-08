using PasswordControllerApp.Models;
using System.Security.Cryptography;
using System.Text;

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

        public (bool IsValid, string Message) ValidatePassword(string password)
        {
            // Using if-else statements as requested
            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, "Password cannot be empty.");
            }

            if (password.Length < 8)
            {
                return (false, "Password must be at least 8 characters long.");
            }

            bool hasNumber = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsDigit(c))
                {
                    hasNumber = true;
                }
                else if (IsSpecialCharacter(c))
                {
                    hasSpecial = true;
                }
                if (hasNumber && hasSpecial)
                {
                    break; // Early exit if both found
                }
            }

            if (!hasNumber)
            {
                return (false, "Password must contain at least one number.");
            }

            if (!hasSpecial)
            {
                return (false, "Password must contain at least one special character (!@#$%^&*(),.?\":{}|<>)");
            }

            return (true, "Password is secure.");
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

        // New: Vault methods
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

        // New: Generate strong password
        public string GenerateStrongPassword(int length = 16)
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specials = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var allChars = lowercase + uppercase + numbers + specials;
            var random = RandomNumberGenerator.Create();
            var password = new char[length];
            var allCharsBytes = Encoding.UTF8.GetBytes(allChars);

            for (int i = 0; i < length; i++)
            {
                var randomBytes = new byte[1];
                random.GetBytes(randomBytes);
                password[i] = allChars[randomBytes[0] % allChars.Length];
            }

            // Ensure at least one of each type (simple shuffle for demo)
            password[0] = uppercase[random.Next(uppercase.Length)];
            password[1] = lowercase[random.Next(lowercase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = specials[random.Next(specials.Length)];

            return new string(password);
        }
    }
}