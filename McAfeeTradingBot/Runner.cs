using System;
using System.Configuration;
using McAfeeTradingBot.Helpers;
using Tweetinvi;

namespace McAfeeTradingBot
{
    internal class Runner
    {
        private const string McAfeeTwitterName = "officialmcafee";

        private readonly SettingsValidator _validator;

        public Runner(SettingsValidator validator)
        {
            _validator = validator;
        }

        internal void Run()
        {
            if (!_validator.ValidateTwitterSettings())
                return;

            SetTwitterCredentials();

            var mcAfeeLastTweet = User.GetUserFromScreenName(McAfeeTwitterName).Status;

            Console.WriteLine(mcAfeeLastTweet.Text);
        }

        private static void SetTwitterCredentials()
        {
            Auth.SetUserCredentials(
                ConfigurationManager.AppSettings["TwitterApiKey"],
                ConfigurationManager.AppSettings["TwitterApiSecret"],
                ConfigurationManager.AppSettings["TwitterAccessToken"],
                ConfigurationManager.AppSettings["TwitterAccessTokenSecret"]);
        }
    }
}
