using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyWatcher.Model
{
    public class ExchangeRateList : List<ExchangeRate>
    {
        public string Currency { get; set; }
        public string CurrencyDenominator { get; set; }
        public string Unit { get; set; }
        public string Title { get; set; }
        public DateTime Extracted { get; set; }
    }
}
