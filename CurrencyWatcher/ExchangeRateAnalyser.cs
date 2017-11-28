using System;
using System.Collections.Generic;
using System.Text;
using CurrencyWatcher.Model;
using System.Linq;

namespace CurrencyWatcher {
    public class ExchangeRateAnalyser {
        public event EventHandler<ExchangeRateEvent> RateAboveAverage;
        public event EventHandler<ExchangeRateEvent> RateBelowAverage;

        private ExchangeRateList _list;

        public ExchangeRateAnalyser(ExchangeRateList list) {
            _list = list;
        }

        public void Analyse() {
            var compareToMonths = 3;

            var currentDate = DateTime.Today;
            var currentExchangerate = _list.Last().Value;
            var monthlyAverage = GetMonthlyAverage();
            var lastThreeMonths = GetLastMonths(compareToMonths);
            var lastThreeMonthsAverage = monthlyAverage.TakeLast(compareToMonths).Average(_ => _.Average);
            lastThreeMonths.Sort((a, b) => a.Value.CompareTo(b.Value));
            var lastThreeMonthsPeak = Math.Round(lastThreeMonths.Last().Value, 2);
            var lastThreeMonthLow = Math.Round(lastThreeMonths.First().Value, 2);
            var isBelowAverage = currentExchangerate < lastThreeMonthsAverage;
            var currentMonthAverage = Math.Round(GetMonthlyAverage(true).Last().Average, 2);

            var title = isBelowAverage ? "Rate below average" : "Rate above average";
            var percent = isBelowAverage ? Math.Round((currentExchangerate / lastThreeMonthsAverage) * 100, 2) :
                                           Math.Round((currentExchangerate / lastThreeMonthsAverage) * 100 - 100, 2);
            var scale = isBelowAverage ? "below" : "above";

            var e = new ExchangeRateEvent {
                Title = title,
                Message = $"The current exchange rate ({_list.Currency}/{_list.CurrencyDenominator}) is {percent}% {scale} the average of the last {compareToMonths} months."
                          + Environment.NewLine + Environment.NewLine +
                          $"Last 3 month exchange rate peak: {lastThreeMonthsPeak} {_list.Unit}"
                          + Environment.NewLine +
                          $"Last 3 month exchange rate low:  {lastThreeMonthLow} {_list.Unit}"
                          + Environment.NewLine +
                          $"Current month average:           {currentMonthAverage} {_list.Unit}"
                          + Environment.NewLine + Environment.NewLine +
                          $"Current rate: {_list.Last().Value} {_list.Unit}/{_list.CurrencyDenominator}"
            };


            if (isBelowAverage) {
                RateBelowAverage?.Invoke(this, e);
            }
            else {
                RateAboveAverage?.Invoke(this, e);
            }
        }

        private List<MonthAverage> GetMonthlyAverage(bool keepUncompleteMonth = false) {
            var list = _list.GroupBy(_ => new { _.Timestamp.Year, _.Timestamp.Month }).Select(_ => new MonthAverage {
                Average = _.Average(p => p.Value),
                Year = _.Key.Year,
                Month = _.Key.Month
            }).ToList();

            if (!keepUncompleteMonth && list.Last().Month == DateTime.Today.Month) {
                list.Remove(list.Last());
            }

            return list;
        }

        private List<ExchangeRate> GetLastMonths(int months) {
            var startDate = DateTime.Today.AddMonths(-months);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            return _list.Where(_ => _.Timestamp >= startDate && _.Timestamp < endDate).Select(_ => _).ToList();
        }
    }



    public class ExchangeRateEvent : EventArgs {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
