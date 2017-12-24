using System;
using System.Configuration;

namespace McAfeeTradingBot.Helpers
{
    internal class SettingsValidator
    {
        internal bool ValidateTwitterSettings()
        {
            if(string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TwitterApiKey"]))
            {
                Console.WriteLine("You need to specify Twitter API key.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TwitterApiSecret"]))
            {
                Console.WriteLine("You need to specify Twitter API secret.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TwitterAccessToken"]))
            {
                Console.WriteLine("You need to specify Twitter access token.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TwitterAccessTokenSecret"]))
            {
                Console.WriteLine("You need to specify Twitter access token secret.");
                return false;
            }

            return true;
        }

        internal bool ValidateBittrexSettings()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BittrexApiKey"]))
            {
                Console.WriteLine("You need to specify Bittrex API key.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BittrexApiSecret"]))
            {
                Console.WriteLine("You need to specify Bittrex API secret.");
                return false;
            }

            return true;
        }
    }
}
