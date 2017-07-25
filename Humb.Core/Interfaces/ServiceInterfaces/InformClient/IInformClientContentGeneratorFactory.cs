using Humb.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces.InformClient
{
    public interface IInformClientContentGeneratorFactory
    {
        IInformClientContentGenerator GenerateWebRequest(InformClientEnums val, params object[] parameters);
    }
}
