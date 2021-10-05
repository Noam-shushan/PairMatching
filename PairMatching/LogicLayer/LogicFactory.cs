
namespace LogicLayer
{
    /// <summary>
    /// Logic Factory 
    /// </summary>
    public static class LogicFactory
    {
        /// <summary>
        /// Logic Factory function
        /// </summary>
        /// <returns>Logic instance of the implemention of the Logic Layer</returns>
        public static ILogicLayer GetLogicFactory()
        {
            return LogicImplementaion.Instance;
        }
    }
}
