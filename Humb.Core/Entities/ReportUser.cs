using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class ReportUser : BaseEntity
    {
        public int FromUserID { get; set; }

        public int ToUserID { get; set; }

        public string ReportInfo { get; set; }

        public int ReportCode { get; set; }
    }
}
