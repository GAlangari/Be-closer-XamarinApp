using BeCloser.Models;
using BeCloser.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BeCloser.Views
{
    public partial class ConvPage : ContentPage
    {
        private string reply;
        private Message msg;
        private List<Message> list = new List<Message>();
        FirebaseHelper db = new FirebaseHelper();

        public ConvPage(Message m)
        {
            InitializeComponent();

            msg = m;
            list.Add(msg);
            lstMsgs.ItemsSource = list;

        }

        private void Reply_Clicked(object sender, EventArgs e)
        {
            if (reply != null)
            {
                DisplayAlert("replied", "reply sent", "ok");

                Message msgReply = new Message
                {
                    from = App.id,
                    to = "Monitor",
                    content = reply,
                    time = DateTime.Now,
                    isTeacher = App.IsTeaher
                };

                _ = db.saveMessage(msgReply, "Monitor");
                _ = db.DeleteMsg(msg.to, msg.from, msg.content);

                Navigation.PopAsync();

            }
            else
            {
                DisplayAlert("Failed", "Please select a reply", "Ok");
            }
        }

        private void otherRadio_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            otherEditor.IsVisible = otherRadio.IsChecked;
        }

        private void ReplyRadio_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton r = sender as RadioButton;
            reply = (string)r.Content;
        }

        private void otherEditor_Completed(object sender, EventArgs e)
        {
            Editor ed = sender as Editor;
            reply = ed.Text;
        }
    }
}