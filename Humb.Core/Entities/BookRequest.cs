using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class BookRequest : BaseEntity
    {        
        public int RequestingUserID { get; set; }

        public int RespondingUserID { get; set; }

        public int BookID { get; set; }

        public int RequestType { get; set; }

        public virtual Book Book { get; set; }
    }
}
