using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Diagnostics;
using BeCloser.ViewModels;
using Plugin.FirebasePushNotification;

namespace BeCloser.Views
{
    public partial class LoginPage : ContentPage
    {
        public FirebaseHelper db;

        public LoginPage()
        {
            InitializeComponent();

            db = new FirebaseHelper();
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(emailEntry.Text) || string.IsNullOrEmpty(passEntry.Text))
            {
                await DisplayAlert("Empty Values", "Please enter Email and Password", "OK");
            }
            else
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(App.WebAPIKey));

                try
                {
                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(emailEntry.Text, passEntry.Text);
                    var content = await auth.GetFreshAuthAsync();

                    Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(content));

                    App.id = auth.User.LocalId;

                    Models.User userData = new Models.User
                    {
                        uid = auth.User.LocalId,
                        firebaseToken = CrossFirebasePushNotification.Current.Token,
                        email = emailEntry.Text
                    };

                    await db.SaveUpdateUser(userData);

                    await Navigation.PushAsync(new InboxPage());


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    await DisplayAlert("Alert", "Invalid useremail or password", "OK");
                }
            }
        }
    }
}