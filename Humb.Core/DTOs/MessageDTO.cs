using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public UserDTO FromUser { get; set; }
        public UserDTO ToUser { get; set; }
        public string MessageText { get; set; }
        public string CreatedAt { get; set; }
        public int MessageState { get; set; }
    }
}
