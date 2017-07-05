using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Controllers.Utils
{
    public static class LovedGenreUtils
    {
        public static void AddGenreCodes(Context context, string email, int[] genreCodes)
        {
            User user = UserUtils.GetUser(context, email);
            LovedGenre lovedGenre;
            if (context.LovedGenres.Any(x => x.userID == user.userID))
            {
                var lovedGenres = context.LovedGenres.Where(x => x.userID == user.userID);
                foreach (var l in lovedGenres)
                {
                    context.LovedGenres.Remove(l);
                }
            }
            foreach (int genreCode in genreCodes)
            {
                lovedGenre = new LovedGenre();
                lovedGenre.User = user;
                lovedGenre.genreCode = genreCode;
                lovedGenre.userID = user.userID;
                context.LovedGenres.Add(lovedGenre);
            }
            context.SaveChanges();
        }

        public static int[] GetGenreCodes(Context context, string email)
        {
            User user = UserUtils.GetUser(context, email);
            return context.LovedGenres.Where(x => x.userID == user.userID).Select(x => x.genreCode).ToArray();
        }
    }
}