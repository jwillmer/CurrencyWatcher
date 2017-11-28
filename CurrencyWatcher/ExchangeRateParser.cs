using CurrencyWatcher.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CurrencyWatcher {
    public class ExchangeRateParser {
        private readonly string _xml;
        public readonly bool IsValidXml;

        public ExchangeRateParser(string xml) {
            IsValidXml = IsXmlValid(xml);
            if (IsValidXml) {
                _xml = xml;
            }
        }

        private bool IsXmlValid(string xml) {
            try {
                new XmlDocument().LoadXml(xml);
                return true;
            }
            catch {
                return false;
            }
        }

        public ExchangeRateList GetExchangeRates() {
            if (!IsValidXml) return null;

            try {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_xml);

                var extracted = doc["CompactData"]["Header"]["Extracted"].InnerText;
                var dataset = doc["CompactData"]["DataSet"];
                var group = dataset["Group"];

                var exchangeRateList = new ExchangeRateList {
                    Currency = group.GetAttribute("CURRENCY"),
                    CurrencyDenominator = group.GetAttribute("CURRENCY_DENOM"),
                    Unit = group.GetAttribute("UNIT"),
                    Title = group.GetAttribute("TITLE_COMPL"),
                    Extracted = DateTime.Parse(extracted),
                };

                foreach (XmlElement exchangeRate in dataset["Series"]) {
                    var timestamp = exchangeRate.GetAttribute("TIME_PERIOD");
                    var value = exchangeRate.GetAttribute("OBS_VALUE");

                    exchangeRateList.Add(new ExchangeRate {
                        Timestamp = DateTime.Parse(timestamp),
                        Value = decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture)
                    });
                }

                return exchangeRateList;
            } catch {
                return null;
            }
        }
    }
}
