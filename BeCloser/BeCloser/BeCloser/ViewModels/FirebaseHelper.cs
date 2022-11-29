using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using BeCloser.Models;

namespace BeCloser.ViewModels
{
    public class FirebaseHelper
    {
        public static FirebaseClient firebase = new FirebaseClient(App.FirebaseDBUrl);

        public async Task SaveUpdateUser(User user)
        {
            var existingUser = (await firebase
              .Child("User")
              .OnceAsync<User>()).Where(x => x.Object.uid == user.uid).FirstOrDefault();

            if (existingUser == null)
            {
                await firebase.Child("User").PostAsync(user);
            }
        }

        public async Task<User> GetUser(string userId)
        {
            return (await firebase
              .Child("User")
              .OnceAsync<User>()).Select(x => new User
              {
                  email = x.Object.email,
                  firebaseToken = x.Object.firebaseToken,
                  uid = x.Object.uid
              }).Where(x => x.uid == userId).FirstOrDefault();
        }

        public async Task UpdateUser(User user)
        {
            await firebase
              .Child("User")
              .PutAsync(user);
        }

        public async Task saveMessage(Message m, string userKey)
        {
            await firebase.Child("Chat/" + userKey + "/Message")
                    .PostAsync(m);
        }

        public ObservableCollection<Message> subReply()
        {
            return firebase.Child("Chat")
                            .Child("Monitor")
                            .Child("Message")
                            .AsObservable<Message>()
                            .AsObservableCollection<Message>();
        }
        public async Task<Message> GetReply(string to)
        {
            try
            {
                var allmsg = await GetAllMsgs(to);
                await firebase
                .Child("Chat")
                .Child(to)
                .Child("Message")
                .OnceAsync<Message>();
                return allmsg.FirstOrDefault(a => a.to == to);

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return null;
            }
        }

        public async Task<List<Message>> GetAllMsgs(string to)
        {
            try
            {
                var msglist = (await firebase
                .Child("Chat")
                .Child(to)
                .Child("Message")
                .OnceAsync<Message>()).Select(item =>
                new Message
                {
                    content = item.Object.content,
                    to = item.Object.to,
                    from = item.Object.from,
                    time = item.Object.time
                }).ToList();
                return msglist;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return null;
            }
        }

        public async Task<bool> find(string to, string from, string content)
        {
            try
            {
                var msg = (await firebase
                .Child("Chat/" + to + "/Message")
                .OnceAsync<Message>()).Where(a => a.Object.from == from && a.Object.content == content).FirstOrDefault();
                if (msg == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return false;
            }
        }

        public ObservableCollection<Message> subChat(string userKey)
        {

            return firebase.Child("Chat/" + userKey + "/Message")
                           .AsObservable<Message>()
                           .AsObservableCollection<Message>();
        }

        public async Task<bool> DeleteMsg(string userKey, string from, string content)
        {
            try
            {
                var toDeleteMsg = (await firebase
                .Child("Chat/" + userKey + "/Message")
                .OnceAsync<Message>()).Where(a => a.Object.from == from && a.Object.content == content).FirstOrDefault();
                if (toDeleteMsg == null)
                {
                    return true;
                }
                await firebase.Child("Chat/" + userKey).Child("Message").Child(toDeleteMsg.Key).DeleteAsync();
                return true;
            }

            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return false;
            }

        }
    }
}
