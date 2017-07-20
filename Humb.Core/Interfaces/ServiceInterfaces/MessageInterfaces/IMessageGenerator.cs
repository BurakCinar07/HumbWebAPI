using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces.MessageInterfaces
{
    public interface IMessageGenerator
    {
        Message GenerateMessage(int fromUserId, int toUserId, string message);
    }
}
