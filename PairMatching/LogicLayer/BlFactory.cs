
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
