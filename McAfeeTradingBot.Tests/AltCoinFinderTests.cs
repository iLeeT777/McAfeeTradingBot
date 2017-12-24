using System;
using System.Configuration;
using System.Linq;
using Bittrex.Net;
using McAfeeTradingBot.Bittrex;
using McAfeeTradingBot.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McAfeeTradingBot.Tests
{
    [TestClass]
    public class AltCoinFinderTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            BittrexDefaults.SetDefaultApiCredentials(
                ConfigurationManager.AppSettings["BittrexApiKey"],
                ConfigurationManager.AppSettings["BittrexApiSecret"]);
        }

        [TestMethod]
        public void CoinOfTheDayTest()
        {
            const string coinOfTheDayTweet = "Coin of the day: BURST -- First truly Green coin and most overlooked coin. Uses 400 times less power than Bitcoin. " +
                                        "Super secure and private. Includes smart contracts, encrypted messaging, decentralized wallet, libertine blockchain. " +
                                        "Most undervalued coin. https://www.burst-coin.org ";

            var currencies = new BittrexDataProvider()
                .GetCurrencies();

            const string expectedCoinOfTheDay = "BURST";

            var success = new AltCoinFinder()
                .TryFindCoinOfTheDay(coinOfTheDayTweet, currencies.Values, out var actualCoinOfTheDay);

            Assert.IsTrue(string.Equals(expectedCoinOfTheDay, actualCoinOfTheDay, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(success);

            //var shortName = currencies
            //    .FirstOrDefault(x => x.Value.Equals(actualCoinOfTheDay, StringComparison.OrdinalIgnoreCase)).Key;

            //new BittrexDataProvider().PlaceBuyOrder(shortName);
        }
    }
}
