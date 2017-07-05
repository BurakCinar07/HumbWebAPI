using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class Message : BaseEntity
    {
        public int FromUserID { get; set; }

        public int ToUserID { get; set; }

        public string MessageText { get; set; }

        public int FromUserMessageState { get; set; }

        public int ToUserMessageState { get; set; }
    }
}
