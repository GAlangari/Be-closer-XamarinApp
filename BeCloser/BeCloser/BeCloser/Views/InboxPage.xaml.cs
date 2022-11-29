using System;
using Xamarin.Forms;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Essentials;
using BeCloser.Models;
using BeCloser.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Plugin.FirebasePushNotification;

namespace BeCloser.Views
{
    public partial class InboxPage : ContentPage
    {
        Message msg;
        FirebaseHelper db;

        ObservableCollection<Message> obsMessages;

        public InboxPage()
        {
            InitializeComponent();

            db = new FirebaseHelper();
            obsMessages = new ObservableCollection<Message>();

            Debug.WriteLine($"Token: {CrossFirebasePushNotification.Current.Token}");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await GetProfileInformationAndRefreshToken();
            lstMsgs.ItemsSource = db.subChat(App.id);
            
        }

        private async Task GetProfileInformationAndRefreshToken()
        {
            lstMsgs.IsRefreshing = true;

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(App.WebAPIKey));

            try
            {
                //This is the saved firebaseauthentication that was saved during the time of login
                var savedfirebaseauth = JsonConvert.DeserializeObject<FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                //Here we are Refreshing the token
                var refreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);

                App.id = refreshedContent.User.LocalId;

                Models.User userData = new Models.User
                {
                    email = refreshedContent.User.Email,
                    uid = refreshedContent.User.LocalId,
                    firebaseToken = CrossFirebasePushNotification.Current.Token
                };

                await db.SaveUpdateUser(userData);

                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(refreshedContent));

                lstMsgs.IsRefreshing = false;

            }
            catch (Exception ex)
            {
                lstMsgs.IsRefreshing = false;
                Debug.WriteLine(ex.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no!  Token expired", "OK");
            }
        }

        async void refreshingPulled(System.Object sender, System.EventArgs e)
        {
            lstMsgs.IsRefreshing = true;

            await GetConversation();

            lstMsgs.IsRefreshing = false;
        }

        async Task GetConversation()
        {

            lstMsgs.ItemsSource = await db.GetAllMsgs(App.id);
        }

        async void lstMsgs_ItemSelected(object sender, ItemTappedEventArgs e)
        {
            var selectedMsg = (Message)e.Item;
            await Navigation.PushAsync(new ConvPage(selectedMsg),true);
        }

        void Logout_Tapped(object sender, EventArgs e)
        {
            Preferences.Remove("MyFirebaseRefreshToken");
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}