using System;
using System.Linq;
using System.Net.Http;
using System.Xml;

namespace CurrencyWatcher
{
    class Program
    {
        static readonly string sek_exchange_rate = "http://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/sek.xml";
        static NotificationManager _notificationManager;

        static void Main(string[] args) {
            var apiKey = ParseApiKey(args);
            var rawData = WebClient.Get(sek_exchange_rate);
            var parser = new ExchangeRateParser(rawData);
            var list = parser.GetExchangeRates();
            var analyser = new ExchangeRateAnalyser(list);
            analyser.RateAboveAverage += Analyser_RateAboveAverage;
            analyser.RateBelowAverage += Analyser_RateBelowAverage;
            _notificationManager = new NotificationManager(apiKey);

            analyser.Analyse();

            Console.ReadKey();
        }

        private static string ParseApiKey(string[] args) {
            var keyName = "-apikey";
            var keyValue = args.Where(_ => _.Contains(keyName))
                               .Select(_ => _.Substring(keyName.Length, _.Length - keyName.Length))
                               .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(keyValue)) {
                Console.WriteLine("To get notified by pushbullet you need to provide an api key in the folowing format:");
                Console.WriteLine("-apikey <your_access_token_here>");
            }

            return keyValue;
        }

        private static void Analyser_RateBelowAverage(object sender, ExchangeRateEvent e) {
            _notificationManager.Send(e.Title, e.Message);
        }

        private static void Analyser_RateAboveAverage(object sender, ExchangeRateEvent e) {
            _notificationManager.Send(e.Title, e.Message);
        }
    }
}
