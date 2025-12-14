using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Data.Interfaces;

namespace Services
{
    public class TrackService
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;

        public TrackService(ITrackRepository trackRepository, IArtistRepository artistRepository, IAlbumRepository albumRepository)
        {
            _trackRepository = trackRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
        }

        public void AddTrack(Track track)
        {
            // Ensure artist exists
            if (track.ArtistId.HasValue)
            {
                var artist = _artistRepository.GetById(track.ArtistId.Value);
                if (artist == null)
                    throw new ArgumentException($"Artist with ID {track.ArtistId} does not exist");
            }

            // Ensure album exists
            if (track.AlbumId.HasValue)
            {
                var album = _albumRepository.GetById(track.AlbumId.Value);
                if (album == null)
                    throw new ArgumentException($"Album with ID {track.AlbumId} does not exist");
            }

            _trackRepository.Add(track);
        }


        public Track? GetTrackById(int id)
        {
            return _trackRepository.GetById(id);
        }

        public List<Track> GetAllTracks()
        {
            return _trackRepository.GetAll();
        }

        public void UpdateTrack(Track track)
        {
            _trackRepository.Update(track);
        }

        public void DeleteTrack(int id)
        {
            _trackRepository.Delete(id);
        }
    }

    public class PlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ITrackRepository _trackRepository;
        private readonly IUserRepository _userRepository;

        public PlaylistService(IPlaylistRepository playlistRepository, ITrackRepository trackRepository, IUserRepository userRepository)
        {
            _playlistRepository = playlistRepository;
            _trackRepository = trackRepository;
            _userRepository = userRepository;
        }

        public void AddPlaylist(Playlist playlist)
        {
            // Ensure user exists
            if (playlist.UserId.HasValue)
            {
                var user = _userRepository.GetById(playlist.UserId.Value);
                if (user == null)
                    throw new ArgumentException($"User with ID {playlist.UserId} does not exist");
            }

            _playlistRepository.Add(playlist);
        }

        public Playlist? GetPlaylistById(int id)
        {
            return _playlistRepository.GetById(id);
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _playlistRepository.GetAll();
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            _playlistRepository.Update(playlist);
        }

        public void DeletePlaylist(int id)
        {
            _playlistRepository.Delete(id);
        }

        public void AddTrackToPlaylist(int playlistId, int trackId, int order = 0)
        {
            var playlist = _playlistRepository.GetById(playlistId);
            var track = _trackRepository.GetById(trackId);

            if (playlist == null || track == null)
                throw new ArgumentException("Playlist or Track not found");

            // Check if track is already in the playlist
            var existing = playlist.PlaylistTracks.FirstOrDefault(pt => pt.TrackId == trackId);
            if (existing != null)
                return; // Track already exists in playlist

            var playlistTrack = new PlaylistTrack
            {
                PlaylistId = playlistId,
                TrackId = trackId,
                TrackOrder = order,
                DateAdded = DateTime.Now
            };

            playlist.PlaylistTracks.Add(playlistTrack);
            _playlistRepository.Update(playlist);
        }

        public void RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            var playlist = _playlistRepository.GetById(playlistId);

            if (playlist == null)
                throw new ArgumentException("Playlist not found");

            var playlistTrack = playlist.PlaylistTracks.FirstOrDefault(pt => pt.TrackId == trackId && pt.PlaylistId == playlistId);

            if (playlistTrack != null)
            {
                playlist.PlaylistTracks.Remove(playlistTrack);
                _playlistRepository.Update(playlist);
            }
        }

    }

    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void AddUser(User user)
        {
            // Check if login is already taken
            var existingUser = _userRepository.GetByLogin(user.Login);
            if (existingUser != null)
                throw new ArgumentException("User with this login already exists");

            _userRepository.Add(user);
        }

        public User? GetUserById(int id)
        {
            return _userRepository.GetById(id);
        }

        public User? GetUserByLogin(string login)
        {
            return _userRepository.GetByLogin(login);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public void UpdateUser(User user)
        {
            _userRepository.Update(user);
        }

        public void DeleteUser(int id)
        {
            _userRepository.Delete(id);
        }

        public bool ValidateUser(string login, string password)
        {
            var user = _userRepository.GetByLogin(login);
            if (user == null)
                return false;

            // Compare password hash with stored hash
            return VerifyPassword(password, user.PasswordHash);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Simplified password verification - in real app, use proper hashing like bcrypt or PBKDF2
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        public void RegisterUser(string login, string username, string? email, string password)
        {
            // Check if login is already taken
            var existingUser = _userRepository.GetByLogin(login);
            if (existingUser != null)
                throw new ArgumentException("User with this login already exists");

            var user = new User
            {
                Login = login,
                Username = username,
                Email = email,
                SubscriptionExpiry = null // No subscription by default
            };

            // Hash the password before storing
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _userRepository.Add(user);
        }

    }

    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;

        public PaymentService(IPaymentRepository paymentRepository, IUserRepository userRepository)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
        }

        public void ProcessPayment(int userId, string cardNumber, string expiryDate, string cvv, decimal amount)
        {
            // Validate user exists
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Validate card details (simplified validation)
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 16 || !long.TryParse(cardNumber, out _))
                throw new ArgumentException("Invalid card number");

            if (string.IsNullOrWhiteSpace(expiryDate) || expiryDate.Length != 5)
                throw new ArgumentException("Invalid expiry date");

            if (string.IsNullOrWhiteSpace(cvv) || (cvv.Length != 3 && cvv.Length != 4))
                throw new ArgumentException("Invalid CVV");

            // Create payment record
            var payment = new Payment
            {
                UserId = userId,
                CardLastFour = cardNumber.Substring(cardNumber.Length - 4),
                PaymentAmount = amount,
                PaymentDate = DateTime.Now,
                Status = "Success"
            };

            _paymentRepository.Add(payment);

            // Update user subscription expiry (typically 1 month from payment)
            user.SubscriptionExpiry = DateTime.Now.AddMonths(1);
            _userRepository.Update(user);
        }

    }
}