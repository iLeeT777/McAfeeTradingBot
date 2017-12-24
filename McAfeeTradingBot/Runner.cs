using System;
using System.Collections.Generic;
using System.Configuration;
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

        // poll Twitter every 0.5 seconds
        private readonly Timer _twitterListener = new Timer(500);

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
            if (!_validator.ValidateTwitterSettings() || !_validator.ValidateBittrexSettings())
                return;

            SetTwitterCredentials();
            SetBittrexCredentials();

            _currenciesCache = _dataProvider.GetCurrencies();

            _twitterListener.Start();

            Console.ReadKey();
        }

        private void OnTwitterListenerElapsed(object sender, ElapsedEventArgs e)
        {
            // Stop the timer while we are processings as MS will otherwise fire another elapsed event on a different thread pool thread.
            _twitterListener.Stop();

            var mcAfeeLastTweet = User.GetUserFromScreenName(McAfeeTwitterName).Status;

            if (_altCoinFinder.TryFindCoinOfTheDay(mcAfeeLastTweet.Text))
            {
                // Coin found! Place order.
                // TODO use bittrex data provider to place the order.
            }
            else
            {
                // Coin not found, keep on polling Twitter.
                _twitterListener.Start();
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
