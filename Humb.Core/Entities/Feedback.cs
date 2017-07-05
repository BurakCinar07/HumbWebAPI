using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class Feedback : BaseEntity
    {
        public string Text { get; set; }

        public bool IsChecked { get; set; }

        public int UserID { get; set; }
    }
}
