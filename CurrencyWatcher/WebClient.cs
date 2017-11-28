using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CurrencyWatcher {
    public static class WebClient {
        public static string Get(string url) {
            try {
                using (var client = new HttpClient()) {
                    using (HttpResponseMessage response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result) {
                        response.EnsureSuccessStatusCode();
                        return response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception e) {
                return e.Message;
            }
        }
    }
}
