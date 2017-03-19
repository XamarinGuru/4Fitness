using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Webkit;

namespace goheja
{
    [Activity(Label = "currentEvent")]
    public class currentEvent : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.currentEvent);
            FindViewById<Button>(Resource.Id.btnBckToTransmit).Click += btnBckToTransmit_OnClick;
            var wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);
            wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient (new WebViewClient ());
			//wv.Settings.AllowFileAccess = true;
			//wv.Settings.AllowFileAccessFromFileURLs = true;
			//wv.Settings.AllowUniversalAccessFromFileURLs = true;

			string nickName = contextPref.GetString("storedUserName", "");
            //wv.LoadUrl("http://go-heja.com/goheja/mobongoing.php?txt=" + nickName);
			wv.LoadUrl("http://go-heja.com/goheja/mob/mobileDay.php?userNickName=" + nickName);

            // Create your application here
        }

        private void btnBckToTransmit_OnClick(object sender, EventArgs e)
        {
            OnDestroy();
        }
        protected override void OnDestroy()
        {

            base.OnDestroy();
            var activity2go = new Intent(this, typeof(MainActivity));
            //activity2.PutExtra("MyData", "Data from Activity1");
            StartActivity(activity2go);
            Finish();

        }
    }
}