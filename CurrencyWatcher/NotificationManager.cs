using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace CurrencyWatcher {
    public class NotificationManager {
        private string _apiKey;

        public NotificationManager(string apiKey) {
            _apiKey = apiKey;
        }

        public void Send(string title, string message) {
            Console.WriteLine();
            Console.WriteLine("--- " + title);
            Console.WriteLine("> " + message);
            Console.WriteLine("---");

            if (!string.IsNullOrWhiteSpace(_apiKey)) {
                var client = new PushbulletClient(_apiKey);
                var currentUserInformation = client.CurrentUsersInformation();

                if (currentUserInformation != null) {
                    PushNoteRequest reqeust = new PushNoteRequest() {
                        Email = currentUserInformation.Email,
                        Title = title,
                        Body = message
                    };

                    client.PushNote(reqeust);
                }
            }
        }
    }
}
