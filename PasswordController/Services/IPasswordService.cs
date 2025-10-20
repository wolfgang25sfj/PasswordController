using PasswordControllerApp.Models;  // For VaultEntry, StrengthLevel
using System.Collections.Generic;   // For List<T>

namespace PasswordControllerApp.Services
{
    public interface IPasswordService
    {
        bool IsAccountSetUp();
        bool SetupAccount(string firstName, string password, string pin);
        bool Login(string firstName, string password, string pin);
        void ResetAccount();
        (bool IsValid, List<string> Messages, StrengthLevel Level) ValidatePassword(string password);
        (bool IsValid, string Message) ValidatePin(string pin);
        (bool IsValid, string Message) ValidateFirstName(string firstName);
        string HashString(string input);
        bool AddVaultEntry(string email, string password);
        List<VaultEntry> GetVaultEntries();
        string GenerateStrongPassword(int length = 16);
        string GetFirstName();
    }
}