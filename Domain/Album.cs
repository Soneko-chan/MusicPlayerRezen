namespace Domain
{
    public class Album
    {
        public int AlbumId { get; set; }
        public int? ArtistId { get; set; }
        public string AlbumName { get; set; } = string.Empty;
        public int? ReleaseYear { get; set; }

        
        public virtual Artist? Artist { get; set; }
        public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}