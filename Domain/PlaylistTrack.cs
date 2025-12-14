namespace Domain
{
    public class PlaylistTrack
    {
        public int PlaylistId { get; set; }
        public int TrackId { get; set; }
        public int TrackOrder { get; set; }
        public DateTime DateAdded { get; set; }

        // Navigation properties
        public virtual Playlist? Playlist { get; set; }
        public virtual Track? Track { get; set; }
    }
}