using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface ILovedGenreService
    {
        void AddGenreCodes(string email, int[] genreCodes);
        IEnumerable<int> GetUserLovedGenreCodes(string email);
    }
}
