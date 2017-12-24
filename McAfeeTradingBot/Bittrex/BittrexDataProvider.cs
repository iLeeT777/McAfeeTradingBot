using System.Collections.Generic;
using System.Linq;
using Bittrex.Net;
using Bittrex.Net.Objects;
using Unity.Interception.Utilities;

namespace McAfeeTradingBot.Bittrex
{
    internal class BittrexDataProvider
    {
        // Exclude the top 5 as these have a massive volume and it could be difficult to manipulate anyway. Exclude 'Decent' from the list as well.
        private readonly HashSet<string> _excludedCoins = new HashSet<string>
        {
            "Bitcoin", "Ethereum", "Bitcoin Cash", "Ripple", "Litecoin", "DECENT"
        };

        /// <summary>
        /// Gets the currencies listed on Bittrex, keyed by the currency short name, value is the long name e.g. BTC key, Bitcoin value.
        /// </summary>
        /// <remarks>All inactive currencies are filtered out.</remarks>
        internal IDictionary<string, string> GetCurrencies()
        {
            var currenciesDictionary = new Dictionary<string, string>();

            using (var client = new BittrexClient())
            {
                var currencies = client.GetCurrencies();

                if (currencies.Success)
                {
                    currencies.Result.ForEach(c =>
                    {
                        if (c.IsActive && !_excludedCoins.Contains(c.CurrencyLong))
                        {
                            currenciesDictionary[c.Currency] = c.CurrencyLong;
                        }
                    });
                }
            }

            return currenciesDictionary;
        }

        internal void PlaceBuyOrder(string coinOfTheDay)
        {
            using (var client = new BittrexClient())
            {
                var coinOfTheDayOrderBook = client.GetOrderBook($"{"BTC"}-{coinOfTheDay}");
                var btcOrderBook = client.GetOrderBook($"{"USDT"}-{"BTC"}");

                if (coinOfTheDayOrderBook.Success && btcOrderBook.Success)
                {
                    var coinOfTheDayLastPrice = coinOfTheDayOrderBook.Result.Buy.First();
                    var btcLastPrice = btcOrderBook.Result.Buy.First();

                    var rate = coinOfTheDayLastPrice.Rate;

                    // This equals to roughly 15$ worth of buying on BTC-alt coin market. TODO - pull out the markets and price to app.config
                    var amountToBuy = 15 / (btcLastPrice.Rate * rate);

                    // TODO - verify if the above is correct before putting a buy order.
                    //client.PlaceOrder(OrderType.Buy, $"{"BTC"}-{coinOfTheDay}", amountToBuy, rate);
                }
            }
        }
    }
}
