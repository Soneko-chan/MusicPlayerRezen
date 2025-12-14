namespace Domain
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime? SubscriptionExpiry { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}