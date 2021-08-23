using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers
{
    [Activity(Label = "userinfoinsertActivity")]
    public class userinfoinsertActivity : AppCompatActivity
    {

        TextInputLayout fbnametext;
        TextInputLayout fbemailtext;
        TextInputLayout fbphonetext;
        Button continuebtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.userinfoinsert);

            fbnametext = (TextInputLayout)FindViewById(Resource.Id.fbname);
            fbemailtext = (TextInputLayout)FindViewById(Resource.Id.fbemail);
            fbphonetext = (TextInputLayout)FindViewById(Resource.Id.fbphone);
            continuebtn = (Button)FindViewById(Resource.Id.continuebtnlayout);

            string name = Intent.GetStringExtra("fbname" ?? "Empty");
            string email = Intent.GetStringExtra("fbemail" ?? "Empty");
            string uid = Intent.GetStringExtra("fbuid" ?? "Empty");

            checktext(name,email,uid);

            continuebtn.Click += Continuebtn_Click;
        }

        private void Continuebtn_Click(object sender, EventArgs e)
        {
            
        }

        void checktext(string takename, string takeemail, string takeuid)
        {
            
            fbnametext.EditText.Text = takename;
            fbemailtext.EditText.Text = takeemail;
            fbphonetext.EditText.Text = "";

            if (takeemail == "")
            {
                fbemailtext.Enabled = true;
            }
            else
            {
                fbemailtext.Enabled = false;
            }
            if (takename == "")
            {
                fbnametext.Enabled = true;
            }
            else
            {
                fbnametext.Enabled = false;
            }
        }
    }
}