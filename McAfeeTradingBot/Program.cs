using Unity;

namespace McAfeeTradingBot
{
    internal class Program
    {
        private static void Main()
        {
            using (var container = new UnityContainer())
            {
                container.Resolve<Runner>().Run();
            }
        }
    }
}
