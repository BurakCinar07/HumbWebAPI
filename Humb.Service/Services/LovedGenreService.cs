using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services
{
    public class LovedGenreService : ILovedGenreService
    {
        private readonly IUserService _userService;
        private readonly IRepository<LovedGenre> _lovedGenreRepository;
        public LovedGenreService(IUserService userService, IRepository<LovedGenre> lovedGenreRepo)
        {
            this._userService = userService;
            this._lovedGenreRepository = lovedGenreRepo;
        }
        public void AddGenreCodes(string email, int[] genreCodes)
        {
            User user = _userService.GetUser(email);
            if(user.LovedGenres != null)
            {
                foreach(var lg in _lovedGenreRepository.FindBy(x => x.UserId == user.Id))
                {
                    _lovedGenreRepository.Delete(lg);
                }
            }
            LovedGenre lovedGenre;
            foreach (var genreCode in genreCodes)
            {
                lovedGenre = new LovedGenre()
                {
                    User = user,
                    GenreCode = genreCode,
                    CreatedAt = DateTime.Now
                };
                _lovedGenreRepository.Insert(lovedGenre);
            }
        }
        public IEnumerable<int> GetUserLovedGenreCodes(string email)
        {
            int userId = _userService.GetUserId(email);
            return _lovedGenreRepository.FindBy(x => x.UserId == userId).Select(x => x.GenreCode);
        }
    }
}
