using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;

namespace Model
{
    public partial class Controller:IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
}
