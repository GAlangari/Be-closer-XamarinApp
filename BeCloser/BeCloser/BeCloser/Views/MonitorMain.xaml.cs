using BeCloser.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BeCloser.Views
{
    public partial class MonitorMain : ContentPage
    {
        public List<string> WAY { get; set; }
        public List<string> TN { get; set; }
        public string teacherID;


        public MonitorMain()
        {
            InitializeComponent();

            WAY = new List<string>()
            {
                "Student",
                "Teacher",
                "Faculty member",
                "Other"
            };
            whoAreYou.ItemsSource = WAY;
            whoAreYou.SelectedItem = WAY[0];

            List<Teacher> teachersDetails = AppConstants.Constants.teachers;
            TN = new List<string>();

            foreach (Teacher t in teachersDetails)
            {
                TN.Add(t.name);
            }
            teacherName.ItemsSource = TN;
            teacherName.SelectedItem = TN[0];
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            // checking the fields
            if (reqName.Text != null && teacherID != null)
            {
                Message msg = new Message
                {
                    from = reqName.Text,
                    to = teacherID,
                    time = DateTime.Now,
                    isTeacher = App.IsTeaher
                };

                if (reqID.Text != null)
                    msg.from += ", ID: " + reqID.Text;

                Navigation.PushAsync(new ComTypeMonitor(msg));
            }
            else
            {
                DisplayAlert("Failed", "Please fill the required fields", "Ok");
            }

        }

        private void teacherName_SelectedIndexChanged(object sender, EventArgs e)
        {
            teacherID = AppConstants.Constants.find((string)teacherName.SelectedItem);
        }
    }
}