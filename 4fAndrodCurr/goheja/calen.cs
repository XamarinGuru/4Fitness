﻿
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using Android.Preferences;
using Android.Content.PM;
using Android.Webkit;
using Android.Net;
using goheja.Services;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
namespace goheja
{
	[Activity (Label = "Calendar and reports")]			
	public class calen : Activity
	{
		WebView wv1;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.calen);
			showPage ();





		
		}
		protected override void OnStop()
		{
			base.OnStop();
		}

		protected override void OnResume()
		{
			base.OnResume();

		}
		protected override void OnPause()
		{
			base.OnPause();
		}

		protected override void OnDestroy()
		{

			base.OnDestroy();
		}

		private void showPage()
		{
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			wv1 = FindViewById<WebView> (Resource.Id.webViewCalen);
			wv1.Settings.JavaScriptEnabled = true;
			wv1.Settings.AllowContentAccess = true;
			wv1.Settings.EnableSmoothTransition ();
			wv1.Settings.LoadsImagesAutomatically = true;
			wv1.Settings.SetGeolocationEnabled (true);

			wv1.Settings.SetGeolocationEnabled (true);
			wv1.SetWebViewClient(new WebViewClient());
			string nickName = contextPref.GetString ("storedUserName", "");
			string id = contextPref.GetString ("storedAthId", "");
			wv1.LoadUrl ("http://go-heja.com/4f/mobda.php?userNickName=" + nickName+"&userId="+id);
			
		}
	}
}

