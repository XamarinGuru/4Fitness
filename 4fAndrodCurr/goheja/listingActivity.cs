using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;


namespace goheja
{
    [Activity(Label = "listing")]
    public class listingActivity : Activity
    {
        string _userName = "";
        string _psw = "";
		string _firstName;
		string _lastName;
		string _emil;
		bool termsAccepted=false;
		bool termsread;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.lActivity);
            FindViewById<Button>(Resource.Id.btnGo).Click += btnGo_OnClick;
			FindViewById<Button>(Resource.Id.termsBtn).Click += termsBtn_OnClick;
			FindViewById<Button>(Resource.Id.btnTermsAccepted).Click += btnTermsAccepted_OnClick;
			termsread = false;

		





        }
        private void btnGo_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
				trackSvc.Service1 sc = new trackSvc.Service1();
                var firstName = FindViewById<EditText>(Resource.Id.etFirstName);
                var lastName = FindViewById<EditText>(Resource.Id.etlastName);
                var userName = FindViewById<EditText>(Resource.Id.etUserName);
                var psw = FindViewById<EditText>(Resource.Id.etPsw);
				var email = FindViewById<EditText>(Resource.Id.etMail);

				_firstName=firstName.Text;
				_lastName=lastName.Text;
				_userName=userName.Text;
				_psw=psw.Text;
				_emil=email.Text;

			
                string device = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
				string validateMessage= validate(_firstName,_lastName,_userName,_psw,_emil);

				if (validateMessage!="")
				{
					Toast.MakeText(this, validateMessage, ToastLength.Long).Show();
					return;
				}
				if (!(termsAccepted==true))
				{
					Toast.MakeText(this, "Terms Were not accepted", ToastLength.Long).Show();
					return;
				}
			     
				sc.insertNewDevice(firstName.Text, lastName.Text, device, userName.Text, psw.Text,termsread,true,_emil);
                //  var activity1 = new Intent(this, typeof(MainActivity));
                _userName = userName.Text;
                _psw = psw.Text;
                OnDestroy();

                //   StartActivity(activity1);
            }
            catch (Exception err)
            {
                string test = err.ToString();
            }

        }
		private void termsBtn_OnClick(object sender, EventArgs eventArgs)
		{
			var uri = Android.Net.Uri.Parse ("http://go-heja.com/4f/terms.php");
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);
			termsread = true;
		}
		private void btnTermsAccepted_OnClick(object sender, EventArgs eventArgs)
		{
			if (termsread == false) {
				Toast.MakeText(this, "Oh,oh , you didnt read the terms!", ToastLength.Long).Show();
			} else {
				Button b = FindViewById<Button> (Resource.Id.btnTermsAccepted);
				b.Text = "terms accepted, thanks";
				termsAccepted = true;
			}
		}
        protected override void OnDestroy()
        {
            
         
            base.OnDestroy();
			var activity2 = new Intent(this, typeof(MainActivity));
			StartActivity(activity2);
            Finish();
        }
		private string validate (string fName,string lName,string uName,string pw,string em)
		{
			trackSvc.Service1 sc = new trackSvc.Service1();
			string message = "";
			string valiUseNameUni;
			if (em == "")
				message = "Please enter email";
			if (!em.Contains("@")||!em.Contains(".")) message="Email is not valid";
			if (pw == "")
				message = "Please enter password";
			if (uName=="") message="Please enter user name";
			valiUseNameUni = sc.validateNickName (uName);
			if (valiUseNameUni == "1")
				message = "Nick name is taken, please try another";
			if (uName.Contains (" "))
				message = "Please do not use space in nick name";
		
			if (fName == "")
				message = "Please enter first name";

		return message;
		}

    }
}