using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyWatcher.Model {
    public class ExchangeRate
    {     
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    }
}
