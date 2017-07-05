using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class BookTransaction : BaseEntity
    {
        public int GiverUserID { get; set; }

        public int TakerUserID { get; set; }

        public int BookID { get; set; }

        public int TransactionType { get; set; }

        public virtual Book Book { get; set; }
    }
}
