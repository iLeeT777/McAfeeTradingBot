using McAfeeTradingBot.Unity;
using Unity;

namespace McAfeeTradingBot
{
    internal class Program
    {
        private static void Main()
        {
            using (var container = new UnityContainer())
            {
                container.AddNewExtension<Registration>();

                container.Resolve<Runner>().Run();
            }
        }
    }
}
