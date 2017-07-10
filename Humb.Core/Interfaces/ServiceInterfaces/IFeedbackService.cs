using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    interface IFeedbackService
    {
        void AddFeedback(string email, string feedback);
    }
}
