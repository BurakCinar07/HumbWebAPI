using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class BookRequest : BaseEntity
    {        
        public int RequestingUserId { get; set; }

        public int RespondingUserId { get; set; }

        public int BookId { get; set; }

        public int RequestType { get; set; }

        public virtual Book Book { get; set; }
    }
}
