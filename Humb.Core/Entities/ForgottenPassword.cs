using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public class ForgottenPassword : BaseEntity
    {
        public string NewPassword { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }

    }
}
