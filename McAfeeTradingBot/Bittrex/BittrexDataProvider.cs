using System;
using System.Collections.Generic;
using System.Configuration;
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

                    // Boost up the order by 8% as the price is being pumped really fast.
                    var rate = coinOfTheDayLastPrice.Rate + coinOfTheDayLastPrice.Rate * 0.08m;

                    var amountToBuy = int.Parse(ConfigurationManager.AppSettings["AmountToBuyInDollars"]) / (btcLastPrice.Rate * rate);

                    Console.WriteLine($"Order details - Amount: [{amountToBuy}]; Rate: [{rate}]");

                    client.PlaceOrder(OrderType.Buy, $"{"BTC"}-{coinOfTheDay}", amountToBuy, rate);
                }
            }
        }
    }
}
