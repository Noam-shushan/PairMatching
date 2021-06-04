using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public static class DalFactory
    {
        public static IDataLayer GetDal(string dalImpName)
        {
            switch (dalImpName)
            {
                case "json":
                    return DalJson.DalJson.Instance;
                default:
                    return null;
            }
        }
    }
}
