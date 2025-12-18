namespace Domain
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;

        
        public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
        public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}