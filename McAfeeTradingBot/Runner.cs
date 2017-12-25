using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;
using Bittrex.Net;
using McAfeeTradingBot.Bittrex;
using McAfeeTradingBot.Helpers;
using Tweetinvi;

namespace McAfeeTradingBot
{
    internal class Runner
    {
        private const string McAfeeTwitterName = "officialmcafee";

        // poll Twitter every 0.1 seconds
        private readonly Timer _twitterListener = new Timer(50);

        private readonly SettingsValidator _validator;
        private readonly BittrexDataProvider _dataProvider;
        private readonly AltCoinFinder _altCoinFinder;

        private IDictionary<string, string> _currenciesCache;

        public Runner(SettingsValidator validator,
            BittrexDataProvider dataProvider,
            AltCoinFinder altCoinFinder)
        {
            _validator = validator;
            _dataProvider = dataProvider;
            _altCoinFinder = altCoinFinder;

            _twitterListener.Elapsed += OnTwitterListenerElapsed;
        }

        internal void Run()
        {
            if (!_validator.ValidateTwitterSettings() 
                || !_validator.ValidateBittrexSettings()
                || !_validator.ValidateBuyAmount())
                return;

            SetTwitterCredentials();
            SetBittrexCredentials();

            _currenciesCache = _dataProvider.GetCurrencies();

            Console.WriteLine("Connected... listening for McAfee's tweet.");

            _twitterListener.Start();

            Console.ReadKey();
        }

        private void OnTwitterListenerElapsed(object sender, ElapsedEventArgs e)
        {
            // Stop the timer while we are processing as MS will otherwise fire another elapsed event on a different thread pool thread.
            _twitterListener.Stop();

            try
            {
                var mcAfeeLastTweet = User.GetUserFromScreenName(McAfeeTwitterName).Status;

                if (_altCoinFinder.TryFindCoinOfTheDay(mcAfeeLastTweet.Text, _currenciesCache.Values, out var coinOfTheDay))
                {
                    Console.WriteLine("Coin of the day [{0}] found!", coinOfTheDay);
                    Console.WriteLine("Placing BUY order...");

                    var shortName = _currenciesCache
                        .FirstOrDefault(x => x.Value.Equals(coinOfTheDay, StringComparison.OrdinalIgnoreCase)).Key;

                    _dataProvider.PlaceBuyOrder(shortName);
                }
                else
                {
                    // Coin not found, keep on polling Twitter.
                    _twitterListener.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excetpion {ex.Message}");
            }
        }

        private static void SetTwitterCredentials()
        {
            Auth.SetUserCredentials(
                ConfigurationManager.AppSettings["TwitterApiKey"],
                ConfigurationManager.AppSettings["TwitterApiSecret"],
                ConfigurationManager.AppSettings["TwitterAccessToken"],
                ConfigurationManager.AppSettings["TwitterAccessTokenSecret"]);
        }

        private static void SetBittrexCredentials()
        {
            BittrexDefaults.SetDefaultApiCredentials(
                ConfigurationManager.AppSettings["BittrexApiKey"],
                ConfigurationManager.AppSettings["BittrexApiSecret"]);
        }
    }
}
