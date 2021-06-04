using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public static class BlFactory
    {
        public static IBL GetBL()
        {
            return LogicImplementaion.Instance;
        }
    }
}
