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
        IInformClientContentGenerator _contentGenerator;
        IInformClientDataSender _dataSender;
        public InformClientService(IInformClientContentGenerator contentGenerator, IInformClientDataSender dataSender)
        {
            _contentGenerator = contentGenerator;
            _dataSender = dataSender;
        }
        public void InformClient(params string[] parameters)
        {
            _dataSender.SendData(_contentGenerator.GenerateContent(parameters));
        }
    }
}
