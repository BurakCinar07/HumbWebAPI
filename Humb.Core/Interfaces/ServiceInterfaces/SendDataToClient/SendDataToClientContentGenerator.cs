﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces.SendDataToClient
{
    public interface SendDataToClientContentGenerator
    {
        void GenerateContent(params string[] parameters);
    }
}
