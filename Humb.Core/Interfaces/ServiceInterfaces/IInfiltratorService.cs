using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    interface IInfiltratorService
    {
        void AddInfiltrator(string ipAdress, int reason, string extraInfo);
    }
}
