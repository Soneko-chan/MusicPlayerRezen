namespace Domain
{
    public class Track
    {
        public int TrackId { get; set; }
        public int? ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string TrackFilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public float TrackDuration { get; set; }
        public string? CoverPath { get; set; }

        // Navigation properties
        public virtual Artist? Artist { get; set; }
        public virtual Album? Album { get; set; }
        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}