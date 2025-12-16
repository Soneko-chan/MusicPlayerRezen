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
            _context.SaveChanges();
        }

        public Track? GetById(int id)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .AsNoTracking()
                .FirstOrDefault(t => t.TrackId == id);
        }

        public List<Track> GetAll()
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .AsNoTracking()
                .ToList();
        }

        public void Update(Track entity)
        {
            var existingTrack = _context.Tracks.FirstOrDefault(t => t.TrackId == entity.TrackId);
            if (existingTrack != null)
            {
                // Обновляем поля сущности
                existingTrack.ArtistId = entity.ArtistId;
                existingTrack.AlbumId = entity.AlbumId;
                existingTrack.DateCreated = entity.DateCreated;
                existingTrack.TrackFilePath = entity.TrackFilePath;
                existingTrack.Title = entity.Title;
                existingTrack.TrackDuration = entity.TrackDuration;
                existingTrack.CoverPath = entity.CoverPath;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Tracks.Remove(entity);
                _context.SaveChanges();
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
            _context.SaveChanges();
        }

        public Artist? GetById(int id)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Artists
                .Include(a => a.Tracks)
                .Include(a => a.Albums)
                .AsNoTracking()
                .FirstOrDefault(a => a.ArtistId == id);
        }

        public List<Artist> GetAll()
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Artists
                .Include(a => a.Tracks)
                .Include(a => a.Albums)
                .AsNoTracking()
                .ToList();
        }

        public void Update(Artist entity)
        {
            var existingArtist = _context.Artists.FirstOrDefault(a => a.ArtistId == entity.ArtistId);
            if (existingArtist != null)
            {
                // Обновляем поля сущности
                existingArtist.ArtistName = entity.ArtistName;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Artists.Remove(entity);
                _context.SaveChanges();
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
            _context.SaveChanges();
        }

        public Album? GetById(int id)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .AsNoTracking()
                .FirstOrDefault(a => a.AlbumId == id);
        }

        public List<Album> GetAll()
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .AsNoTracking()
                .ToList();
        }

        public void Update(Album entity)
        {
            var existingAlbum = _context.Albums.FirstOrDefault(a => a.AlbumId == entity.AlbumId);
            if (existingAlbum != null)
            {
                // Обновляем поля сущности
                existingAlbum.ArtistId = entity.ArtistId;
                existingAlbum.AlbumName = entity.AlbumName;
                existingAlbum.ReleaseYear = entity.ReleaseYear;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Albums.Remove(entity);
                _context.SaveChanges();
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
            _context.SaveChanges();
        }

        public Playlist? GetById(int id)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Artist)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Album)
                .AsNoTracking()
                .FirstOrDefault(p => p.PlaylistId == id);
        }

        public List<Playlist> GetAll()
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Artist)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track!)
                        .ThenInclude(t => t!.Album)
                .AsNoTracking()
                .ToList();
        }

        public void Update(Playlist entity)
        {
            // Сначала получаем существующую сущность из БД
            var existingPlaylist = _context.Playlists
                .FirstOrDefault(p => p.PlaylistId == entity.PlaylistId);

            if (existingPlaylist != null)
            {
                // Обновляем только основные поля
                existingPlaylist.PlaylistName = entity.PlaylistName;
                existingPlaylist.UserId = entity.UserId;
                existingPlaylist.PlaylistCoverPath = entity.PlaylistCoverPath;
                existingPlaylist.DateCreated = entity.DateCreated;

                _context.SaveChanges();
            }

            // Обновляем связи отдельно, чтобы избежать конфликта отслеживания
            // Удаляем старые связи
            var existingTracks = _context.PlaylistTracks.Where(pt => pt.PlaylistId == entity.PlaylistId).ToList();
            _context.PlaylistTracks.RemoveRange(existingTracks);

            // Добавляем новые связи
            foreach (var track in entity.PlaylistTracks)
            {
                // Проверяем, что трек существует и не отслеживается где-то еще
                var playlistTrack = new PlaylistTrack
                {
                    PlaylistId = entity.PlaylistId, // Используем ID плейлиста, а не из track
                    TrackId = track.TrackId,
                    TrackOrder = track.TrackOrder,
                    DateAdded = track.DateAdded
                };

                _context.PlaylistTracks.Add(playlistTrack);
            }

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            // Создаем отдельный запрос без загрузки всех связанных данных, чтобы избежать проблем отслеживания
            var playlist = _context.Playlists
                .Include(p => p.PlaylistTracks)  // Загружаем только связи, которые нужно удалить
                .FirstOrDefault(p => p.PlaylistId == id);

            if (playlist != null)
            {
                // Удаляем связи с треками
                _context.PlaylistTracks.RemoveRange(playlist.PlaylistTracks);

                // Удаляем сам плейлист
                _context.Playlists.Remove(playlist);

                _context.SaveChanges();
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
            _context.SaveChanges();
        }

        public User? GetById(int id)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .AsNoTracking()
                .FirstOrDefault(u => u.UserId == id);
        }

        public List<User> GetAll()
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .AsNoTracking()
                .ToList();
        }

        public void Update(User entity)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == entity.UserId);
            if (existingUser != null)
            {
                // Обновляем поля сущности
                existingUser.Login = entity.Login;
                existingUser.PasswordHash = entity.PasswordHash;
                existingUser.Username = entity.Username;
                existingUser.Email = entity.Email;
                existingUser.SubscriptionExpiry = entity.SubscriptionExpiry;
                existingUser.DateCreated = entity.DateCreated;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            // Для удаления получаем сущность напрямую из базы данных без использования кэшированных данных
            var entity = _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .AsNoTracking()
                .FirstOrDefault(u => u.UserId == id);

            if (entity != null)
            {
                _context.Users.Remove(entity);
                _context.SaveChanges();
            }
        }

        public User? GetByLogin(string login)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .AsNoTracking()
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
            _context.SaveChanges();
        }

        public Payment? GetById(int id)
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Payments
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefault(p => p.PaymentId == id);
        }

        public List<Payment> GetAll()
        {
            // Принудительно очищаем отслеживание и получаем свежие данные из базы
            _context.ChangeTracker.Clear();

            return _context.Payments
                .Include(p => p.User)
                .AsNoTracking()
                .ToList();
        }

        public void Update(Payment entity)
        {
            var existingPayment = _context.Payments.FirstOrDefault(p => p.PaymentId == entity.PaymentId);
            if (existingPayment != null)
            {
                // Обновляем поля сущности
                existingPayment.UserId = entity.UserId;
                existingPayment.CardLastFour = entity.CardLastFour;
                existingPayment.PaymentAmount = entity.PaymentAmount;
                existingPayment.PaymentDate = entity.PaymentDate;
                existingPayment.Status = entity.Status;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Payments.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}