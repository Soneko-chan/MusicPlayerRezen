using System.Collections.Generic;
using System.Linq;
using Domain;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SqlServer
{
    public class TrackRepository : ITrackRepository
    {
        public readonly MusicPlayerDbContext _context;

        public TrackRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public void Add(Track entity)
        {
            _context.Tracks.Add(entity);
        }

        public Track? GetById(int id)
        {
            return _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .FirstOrDefault(t => t.TrackId == id);
        }

        public List<Track> GetAll()
        {
            return _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .ToList();
        }

        public void Update(Track entity)
        {
            _context.Tracks.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Tracks.Remove(entity);
            }
        }
    }

    public class ArtistRepository : IArtistRepository
    {
        public readonly MusicPlayerDbContext _context;

        public ArtistRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public void Add(Artist entity)
        {
            _context.Artists.Add(entity);
        }

        public Artist? GetById(int id)
        {
            return _context.Artists
                .Include(a => a.Tracks)
                .Include(a => a.Albums)
                .FirstOrDefault(a => a.ArtistId == id);
        }

        public List<Artist> GetAll()
        {
            return _context.Artists
                .Include(a => a.Tracks)
                .Include(a => a.Albums)
                .ToList();
        }

        public void Update(Artist entity)
        {
            _context.Artists.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Artists.Remove(entity);
            }
        }
    }

    public class AlbumRepository : IAlbumRepository
    {
        public readonly MusicPlayerDbContext _context;

        public AlbumRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public void Add(Album entity)
        {
            _context.Albums.Add(entity);
        }

        public Album? GetById(int id)
        {
            return _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .FirstOrDefault(a => a.AlbumId == id);
        }

        public List<Album> GetAll()
        {
            return _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .ToList();
        }

        public void Update(Album entity)
        {
            _context.Albums.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Albums.Remove(entity);
            }
        }
    }

    public class PlaylistRepository : IPlaylistRepository
    {
        public readonly MusicPlayerDbContext _context;

        public PlaylistRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public void Add(Playlist entity)
        {
            _context.Playlists.Add(entity);
        }

        public Playlist? GetById(int id)
        {
            return _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Artist)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Album)
                .FirstOrDefault(p => p.PlaylistId == id);
        }

        public List<Playlist> GetAll()
        {
            return _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Artist)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Album)
                .ToList();
        }

        public void Update(Playlist entity)
        {
            _context.Playlists.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Playlists.Remove(entity);
            }
        }
    }

    public class UserRepository : IUserRepository
    {
        public readonly MusicPlayerDbContext _context;

        public UserRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public void Add(User entity)
        {
            _context.Users.Add(entity);
        }

        public User? GetById(int id)
        {
            return _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .FirstOrDefault(u => u.UserId == id);
        }

        public List<User> GetAll()
        {
            return _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .ToList();
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
            }
        }

        public User? GetByLogin(string login)
        {
            return _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .FirstOrDefault(u => u.Login == login);
        }
    }

    public class PaymentRepository : IPaymentRepository
    {
        public readonly MusicPlayerDbContext _context;

        public PaymentRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public void Add(Payment entity)
        {
            _context.Payments.Add(entity);
        }

        public Payment? GetById(int id)
        {
            return _context.Payments
                .Include(p => p.User)
                .FirstOrDefault(p => p.PaymentId == id);
        }

        public List<Payment> GetAll()
        {
            return _context.Payments
                .Include(p => p.User)
                .ToList();
        }

        public void Update(Payment entity)
        {
            _context.Payments.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Payments.Remove(entity);
            }
        }
    }
}