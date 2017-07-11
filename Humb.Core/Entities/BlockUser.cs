using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class BlockUser : BaseEntity
    {
        public int FromUserId { get; set; }

        public int ToUserId { get; set; }
    }
}
