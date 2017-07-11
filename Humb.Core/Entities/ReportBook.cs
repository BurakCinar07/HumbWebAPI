using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class ReportBook : BaseEntity
    {        
        public int UserId { get; set; }

        public int BookId { get; set; }

        public string ReportInfo { get; set; }

        public int ReportCode { get; set; }
    }
}
