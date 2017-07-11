using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class BookTransaction : BaseEntity
    {
        public int GiverUserId { get; set; }

        public int TakerUserId { get; set; }

        public int BookId { get; set; }

        public int TransactionType { get; set; }

        public virtual Book Book { get; set; }
    }
}
