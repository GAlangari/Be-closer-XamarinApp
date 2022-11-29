using BeCloser.Views;
using Plugin.FirebasePushNotification;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BeCloser
{
    public partial class App : Application
    {
        public static string id;
        public static string TeacherId = string.Empty;
        public static bool IsTeaher = false;
        public static string WebAPIKey = "AIzaSyAKOLCs-6yYbUADSPSsWaL6w8I8YQnkybU";
        public static string SenderId = "119444922899";
        public static string ServerKey = "AAAAG8944hM:APA91bHu5ot6z4giKUp56WbJ_0ITE3yiIgC2kyGYcWbvcqYbK4XcDs828l_1mzzc5iGxBuqcWNyPosiH6Y3BPzeXrx-Zk6UbcGkomnLTV9eMtI5zn5NsW2VKd8QVOnpUQVKCMN9uPkTh";
        public static string FirebaseDBUrl = "https://becloser-90142-default-rtdb.firebaseio.com/";
        public static string FirebaseUrl = "https://fcm.googleapis.com/fcm/send"; 

        public App()
        {
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    IsTeaher = true;

                    if (!string.IsNullOrEmpty(Preferences.Get("MyFirebaseRefreshToken", string.Empty)))
                    {
                        MainPage = new NavigationPage(new InboxPage());
                    }
                    else
                    {
                        MainPage = new NavigationPage(new LoginPage());
                    }

                    break;

                case Device.UWP:
                    MainPage = new NavigationPage(new MonitorMain());
                    break;
            }
        }

        protected override void OnStart()
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
                {
                    System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
                };

                CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
                {
                    System.Diagnostics.Debug.WriteLine("Received");
                };

                CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
                {
                    System.Diagnostics.Debug.WriteLine("Opened");
                    foreach (var data in p.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    }
                };
            }

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
