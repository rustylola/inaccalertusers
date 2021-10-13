using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.EventListener
{
    public class UserProfileEventListener : Java.Lang.Object, IValueEventListener
    {
        //define shared preference
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                string name, email, phone;
                name = (snapshot.Child("name") != null) ? snapshot.Child("name").Value.ToString() : "";
                email = (snapshot.Child("email") != null) ? snapshot.Child("email").Value.ToString() : "";
                phone = (snapshot.Child("phone") != null) ? snapshot.Child("phone").Value.ToString() : "";

                editor.PutString("name",name);
                editor.PutString("email",email);
                editor.PutString("phone", phone);
                editor.Apply();
            }
        }

        public void Create()
        {
            editor = preferences.Edit();
            FirebaseDatabase database = AppDataHelper.Getdatabase();
            string userID = AppDataHelper.Getcurrentuser().Uid;
            DatabaseReference userReference = database.GetReference("users/" + userID);
            userReference.AddValueEventListener(this);
        }
        public void Logout()
        {
            editor = preferences.Edit();
            editor.Clear();
        }
    }
}