namespace PasswordControllerApp.Models
{
    public class StoredData
    {
        public string? FirstName { get; set; }
        public string? PasswordHash { get; set; }
        public string? PinHash { get; set; }
        public List<VaultEntry> VaultEntries { get; set; } = new List<VaultEntry>();
    }

    public class VaultEntry
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  // Hashed
        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}