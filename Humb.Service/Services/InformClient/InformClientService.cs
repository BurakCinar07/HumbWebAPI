using Humb.Core.Constants;
using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services.InformClient
{
    public class InformClientService : IInformClientService
    {
        private readonly IInformClientContentGeneratorFactory _contentGeneratorFactory;
        private readonly IInformClientDataSender _dataSender;
        public InformClientService(IInformClientContentGeneratorFactory contentGeneratorFactory, IInformClientDataSender dataSender)
        {
            _contentGeneratorFactory = contentGeneratorFactory;
            _dataSender = dataSender;
        }
        public void InformClient(InformClientEnums val,params object[] parameters)
        {
            _dataSender.SendData(_contentGeneratorFactory.GenerateWebRequest(val, parameters).GenerateContent());
        }
    }
}
