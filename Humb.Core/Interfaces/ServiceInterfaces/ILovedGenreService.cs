using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    interface ILovedGenreService
    {
        void AddGenreCodes(string email, int[] genreCodes);
        int[] GetUserLovedGenreCodes(string email);
    }
}
