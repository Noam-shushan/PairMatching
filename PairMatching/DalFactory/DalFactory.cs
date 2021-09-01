using System.Configuration;

namespace DataLayer
{
    public static class DalFactory
    {
        public static IDataLayer GetDal()
        {
            var dal = ConfigurationManager.AppSettings["dal"];
            switch (dal)
            {
                case "json":
                    return DalJson.DalJson.Instance;
                case "mongo":
                    return DalMongo.DalMongo.Instance;
                default:
                    return null;
            }
        }
    }
}
