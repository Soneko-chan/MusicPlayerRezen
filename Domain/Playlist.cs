namespace Domain
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public int? UserId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public string? PlaylistCoverPath { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        
        public virtual User? User { get; set; }
        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}