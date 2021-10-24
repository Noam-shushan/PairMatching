using System.Configuration;

namespace DataLayer
{
    /// <summary>
    /// A Factory class to get the Singleton object of the implamentation of the data layer. <br/>
    /// Can be implament by MongoDB in the cloud or in the hard drive in Json format.
    /// </summary>
    public static class DalFactory
    {
        /// <summary>
        /// Get the data layer implamentation.<br/>
        /// Can be implament by MongoDB in the cloud or in the hard drive in Json format.
        /// </summary>
        /// <returns>The Instance object of the dal implamentation</returns>
        public static IDataLayer GetDal()
        {
            // config the data layer implamention from the app.config
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
