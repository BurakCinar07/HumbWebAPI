using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class BookInteraction : BaseEntity
    {        
        public int UserID { get; set; }

        public int BookID { get; set; }

        public int InteractionType { get; set; }

        public virtual Book Book { get; set; }

        public virtual User User { get; set; }
    }
}
