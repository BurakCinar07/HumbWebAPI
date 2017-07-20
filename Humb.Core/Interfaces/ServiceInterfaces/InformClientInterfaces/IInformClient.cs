using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces.InformClientInterfaces
{
    public interface IInformClient
    {
        void Inform(string senderFcmToken, string recieverFcmToken, object fcmData);
    }
}
