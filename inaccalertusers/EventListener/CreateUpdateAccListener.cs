using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertusers.LocateUpdate;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.EventListener
{
    public class CreateUpdateAccListener : Java.Lang.Object, IValueEventListener
    {

        DatabaseReference userReference;
        string fbname;
        string fbemail;
        string fbphone;
        public CreateUpdateAccListener(string name, string email, string phone)
        {
            fbname = name;
            fbemail = email;
            fbphone = phone;
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                UpdateAcc();
            }
            else
            {
                CreateNewAcc();
            }
        }

        public void CreateRef()
        {
            FirebaseDatabase database = AppDataHelper.Getdatabase();
            userReference = database.GetReference("users/" + AppDataHelper.Getcurrentuser().Uid);
            userReference.AddListenerForSingleValueEvent(this);
        }

        public void CreateNewAcc()
        {
            HashMap userMap = new HashMap();
            userMap.Put("email", fbemail);
            userMap.Put("phone", fbphone);
            userMap.Put("name", fbname);
            userReference.SetValue(userMap);
        }

        public void UpdateAcc()
        {
            userReference.Child("name").SetValue(fbname);
            userReference.Child("phone").SetValue(fbphone);
            userReference.Child("email").SetValue(fbemail);
        }

    }
}