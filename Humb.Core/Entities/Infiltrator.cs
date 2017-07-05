using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{
    public partial class Infiltrator : BaseEntity
    {        
        public string IPAdress { get; set; }        

        public int Reason { get; set; }

        public string ExtraInfo { get; set; }
    }
}
