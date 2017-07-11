using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class LovedGenre : BaseEntity
    {
        public int UserId { get; set; }

        public int GenreCode { get; set; }

        public virtual User User { get; set; }
    }
}
