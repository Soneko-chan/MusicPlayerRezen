using System.Collections.Generic;
using Domain;

namespace Data.Interfaces
{
    public interface ITrackRepository
    {
        void Add(Track entity);
        Track? GetById(int id);
        List<Track> GetAll();
        void Update(Track entity);
        void Delete(int id);
    }

    public interface IArtistRepository
    {
        void Add(Artist entity);
        Artist? GetById(int id);
        List<Artist> GetAll();
        void Update(Artist entity);
        void Delete(int id);
    }

    public interface IAlbumRepository
    {
        void Add(Album entity);
        Album? GetById(int id);
        List<Album> GetAll();
        void Update(Album entity);
        void Delete(int id);
    }

    public interface IPlaylistRepository
    {
        void Add(Playlist entity);
        Playlist? GetById(int id);
        List<Playlist> GetAll();
        void Update(Playlist entity);
        void Delete(int id);
    }

    public interface IUserRepository
    {
        void Add(User entity);
        User? GetById(int id);
        List<User> GetAll();
        void Update(User entity);
        void Delete(int id);
        User? GetByLogin(string login);
    }

    public interface IPaymentRepository
    {
        void Add(Payment entity);
        Payment? GetById(int id);
        List<Payment> GetAll();
        void Update(Payment entity);
        void Delete(int id);
    }
}