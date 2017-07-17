using Humb.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces.EmailInterfaces
{
    public interface IEmailFactory
    {
        void Initialize(EmailEnums val, object[] objs);
    }
}
