using BeCloser.Models;
using BeCloser.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BeCloser.Views
{
    public partial class ConvMonitor : ContentPage
    {
        ObservableCollection<Message> obsMessages;
        private Message msg;
        private Message repMsg = null;
        private bool timeout = false;
        private FirebaseHelper db = new FirebaseHelper();

        public ConvMonitor(Message message)
        {
            InitializeComponent();

            obsMessages = new ObservableCollection<Message>();
            msg = message;

            Device.InvokeOnMainThreadAsync(async () =>
            {
                var messages = await db.GetAllMsgs(message.to);

                if (obsMessages.Count > 0)
                    obsMessages.Clear();

                if (messages.Count > 0)
                {
                    foreach (var item in messages)
                    {
                        obsMessages.Add(item);
                    }
                }

                lstMsgs.ItemsSource = obsMessages;
            });
            startTimer();
        }

        private void startTimer()
        {
            List<Message> list = new List<Message>();
            list.Add(msg);
            string secs = null;
            string mins = null;
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                
                foreach (var m in list)
                {
                    var timespan = m.time - DateTime.Now;
                    m.Timespan = timespan;
                    secs = m.Seconds;
                    mins = m.Minutes;
                }
                
                
                eventList.ItemsSource = null;
                eventList.ItemsSource = list;

                if(repMsg != null)
                {
                    return false;
                }
                else if(secs.Equals("00") && mins.Equals("00"))
                {
                    userAnswer();
                    timeout = true;
                    return false;
                }               
                else
                {
                    return true;
                }
                
            });
            
        }

        private async void userAnswer()
        {
            bool isActionSelected = false;
            while (!isActionSelected)
            {
                bool choice = await DisplayAlert("Request timout!", "Sorry, your request time is out, try again later :(",
                                                         "Ok, go to home page",
                                                         "Let me check last time");
                if (choice == true)
                {
                    _ = db.DeleteMsg(msg.to, msg.from, msg.content);
                    isActionSelected = true;
                    repMsg = null;
                    _ = Navigation.PushAsync(new MonitorMain());
                }
                else if (choice == false)
                {
                    isActionSelected = true;
                }
            }
        }
    

        private async void Home_Clicked(object sender, EventArgs e)
        {
            if (repMsg != null)
            {
                _ = db.DeleteMsg(repMsg.to, repMsg.from, repMsg.content);
                await Navigation.PushAsync(new MonitorMain());
            }
            else if (timeout == true && repMsg == null)
            {            
                await Navigation.PushAsync(new MonitorMain());
            }
            else
            {
                bool isActionSelected = false;
                while (!isActionSelected)
                {
                    bool choice = await DisplayAlert("Attention!", "Returning to home page will delete your message, do you want to proceed to the home page?",
                                                             "Yes, go to home page",
                                                             "Cancel");
                    if (choice == true)
                    {
                        _ = db.DeleteMsg(msg.to, msg.from, msg.content);
                        isActionSelected = true;
                        repMsg = null;
                        await Navigation.PushAsync(new MonitorMain());
                    }
                    else if (choice == false)
                    {
                        isActionSelected = true;
                    }
                }
            }
        }
        private async void checkResButton_Clicked(object sender, EventArgs e)
        {
            
            repMsg = await db.GetReply("Monitor");
            if (repMsg != null)
            {
                teacherMsgF.IsVisible = true;
                checkResButton.IsVisible = false;
                teacherMsg.Text = repMsg.content;
            }
            if (timeout == true)
            {
                checkResButton.IsEnabled = false;
                _ = db.DeleteMsg(msg.to, msg.from, msg.content);
            }
        }

    }
}