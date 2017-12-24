using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Core.Extensions;

namespace McAfeeTradingBot.Helpers
{
    /// <summary>
    /// Class that tries to find the altcoin i.e. coin of the day, by examing McAfee's last tweet.
    /// </summary>
    internal class AltCoinFinder
    {
        private const string CoinOfTheDayExpression = "Coin of the day";

        internal bool TryFindCoinOfTheDay(string tweet, IEnumerable<string> coins, out string coinOfTheDay)
        {
            coinOfTheDay = string.Empty;

            var coinsFound = new List<string>();

            if (tweet.IndexOf(CoinOfTheDayExpression, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine("Coin of the day tweet found!");

                coins.ForEach(c =>
                {
                    if (tweet.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        coinsFound.Add(c);
                    }
                });
            }

            if (coinsFound.Count == 1)
            {
                // Consider this a success only if the tweet contains the 'coin of the day' wording
                // and if we have only one matching coin that is traded on Bittrex.
                coinOfTheDay = coinsFound.First();
                return true;
            }

            return false;
        }
    }
}
