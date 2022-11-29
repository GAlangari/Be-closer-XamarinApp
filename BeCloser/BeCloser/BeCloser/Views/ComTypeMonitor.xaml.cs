using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using BeCloser.Models;
using BeCloser.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BeCloser.Views
{
    public partial class ComTypeMonitor : ContentPage
    {
        public FirebaseHelper db = new FirebaseHelper();
        private Message msg;

        public ComTypeMonitor(Message m)
        {
            InitializeComponent();
            msg = m;
        }

        private async void Send_Clicked(object sender, EventArgs e)
        {
            if (haveQ.IsChecked)
            {
                if (msg.content != null)
                    msg.content += ", " + haveQLabel.Text;
                else
                    msg.content += haveQLabel.Text;
            }

            if (seeU.IsChecked)
            {
                if (msg.content != null)
                    msg.content += ", " + seeULabel.Text;
                else
                    msg.content += seeULabel.Text;
            }
            if (seePaper.IsChecked)
            {
                if (msg.content != null)
                    msg.content += ", " + seePaperLabel.Text;
                else
                    msg.content += seePaperLabel.Text;
            }
            if (other.IsChecked && otherEntry.Text != null)
            {
                if (msg.content != null)
                    msg.content += ", " + otherEntry.Text;
                else
                    msg.content += otherEntry.Text;
            }

            if (msg.content != null)
            {
                App.TeacherId = msg.to;

                // Setting the time for the timer
                msg.time = new DateTime(DateTime.Now.Ticks + new TimeSpan(0,0,5,0).Ticks);

                await db.saveMessage(msg, msg.to);

                var userData = await db.GetUser(msg.to);
                var result = SendNotification(userData.firebaseToken, $"{msg.from} send you a new message", msg.content);
                Debug.WriteLine(result);
                await Navigation.PushAsync(new ConvMonitor(msg));
            }
        }

        public string SendNotification(string DeviceToken, string title, string msg)
        {
            var result = "-1";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(App.FirebaseUrl);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", App.ServerKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", App.SenderId));
            httpWebRequest.Method = "POST";

            var payload = new
            {
                to = DeviceToken,
                notification = new
                {
                    body = msg,
                    title = title,
                    sound="default",
                    content_available = true
                },
                data = new
                {
                    priority = "high"
                }
            };

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private void other_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            otherEntry.IsVisible = other.IsChecked;

        }
    }
}