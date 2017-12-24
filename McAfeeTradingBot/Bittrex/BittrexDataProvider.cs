using System.Collections.Generic;
using Bittrex.Net;
using Unity.Interception.Utilities;

namespace McAfeeTradingBot.Bittrex
{
    internal class BittrexDataProvider
    {
        // Exclude the top 5 as these have a massive volume and it could be difficult to manipulate anyway.
        private readonly HashSet<string> _excludedCoins = new HashSet<string>
        {
            "Bitcoin", "Ethereum", "Bitcoin Cash", "Ripple", "Litecoin"
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

        internal void PlaceBuyOrder()
        {
            // TODO
        }
    }
}
