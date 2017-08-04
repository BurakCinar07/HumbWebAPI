using Humb.Core.Constants;

namespace Humb.Core.Interfaces.ServiceInterfaces.InformClient
{
    //TODO : ThreadPool imp.
    public interface IInformClientService
    {
        void InformClient(InformClientEnums val, params object[] parameters);
    }
}
