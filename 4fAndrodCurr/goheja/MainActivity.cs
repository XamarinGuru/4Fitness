using System;
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

	[Activity(Label = "4Fitness", MainLauncher = false, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait,LaunchMode=LaunchMode.SingleInstance)]
    public class MainActivity : Activity, ILocationListener
    {
		//ConnectivityManager connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);
        Location _currentLocation;
        Location _lastLocation;
        LocationManager _locationManager;
        TextView _locationText;
        TextView _speedText;
        TextView _altitudeText;
        TextView _GradientText;
        TextView _distance;
		TextView timer;
	    	Button startEvent;
		trackSvc.Service1 svc;
        string _locationProvider;
        string athId;
        string athFirstName;
        string athLastName;
        string athCountry;
        private NotificationManager notificationManager;
        float lastAlt;
        float dist;
        float gainAlt;
		string android_id;
		int fFlag=1;
		Android.Locations.Location location;
		ConnectivityManager connectivityManager ;
		Button startBtn;
		NetworkInfo activeConnection;
		bool isStarted;
		Button btnStopPractice; 
		bool isPaused;
		Timer _timer;
		int duration;
		int lapDuration;
		Button btnSelectBike;
		Button btnSelectRun;
		string type="bike";
		Button selectLbl;
		Button dummyBtn;
		TextView speedLbl;
		ImageView profile;
		Button deviceBtn;
		Button timerBtn;
		Button btnLL;
		Button btnLapDist;
		float distForLap;
		DateTime tempTime;//for vibrate
		ISharedPreferences prefs;
		handleRecord updateRecord ;
		NetworkInfo internetavilable; 
		bool isOnline;
		WebView wv;
		ISharedPreferences contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
		string internetState;
		bool isNewUser = false;
		Bundle bundle;




        public void OnLocationChanged(Location location)
        {

        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }


        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnResume()
        {
			if (isNewUser) OnCreate(bundle);
            base.OnResume();
			setBitmapImg ();
          //  _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
           

        }

        protected override void OnDestroy()
        {
			
           // Toast.MakeText(this, "test", ToastLength.Long).Show();
            base.OnDestroy();
			DateTime destroytTime = DateTime.Now;
           // _locationManager.RemoveUpdates(this);
            notificationManager.CancelAll();

				
			Android.OS.Process.KillProcess (Android.OS.Process.MyPid ());
			//if (isStarted) 
			//{
			//	svc.updateMomgoData (athFirstName + " " + athLastName, String.Format ("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), destroytTime, true, android_id, 0f, true, athId, athCountry, prefs.GetFloat ("dist", 0f), true, gainAlt, true, _currentLocation.Bearing, true, 2, true, type);
			//}
            Finish();


        }

        protected override void OnCreate(Bundle bundle)
        {
			base.OnCreate (bundle);
			startLocationService ();
			SetContentView (Resource.Layout.Main);

			prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
			prefs.Edit().PutFloat("gainAlt", 0f).Commit();
			prefs.Edit().PutFloat("lastAlt", 0f).Commit();
			prefs.Edit().PutFloat("dist", 0f).Commit();

		
			initiatAth();
			trackSvc.Service1 test = new trackSvc.Service1();
			SetContentView(Resource.Layout.Main);
			string _deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
			//_locationText = FindViewById<TextView>(Resource.Id.location_text);
			_speedText = FindViewById<TextView>(Resource.Id.tvSpeed);
			_altitudeText = FindViewById<TextView>(Resource.Id.tvAltitude);

			_distance = FindViewById<TextView>(Resource.Id.tvDistance);

			startEvent = FindViewById<Button> (Resource.Id.StartPractice);
			wv=FindViewById<WebView> (Resource.Id.wvCurrentEvent);
			FindViewById<Button>(Resource.Id.meCalendar).Click += meCalendar_OnClick;
			FindViewById<Button>(Resource.Id.StartPractice).Click += StartPractice_OnClick;
			FindViewById<Button> (Resource.Id.stopPractice).Click+=stopPractice_OnClick;
			FindViewById<Button> (Resource.Id.btnSelectRun).Click+=selectRun_OnClick;
			FindViewById<Button> (Resource.Id.btnSlectBike).Click+=selectBike_OnClick;
			FindViewById<Button> (Resource.Id.deviceBtn).Click+=selectDevice_OnClick;
			deviceBtn=FindViewById<Button> (Resource.Id.deviceBtn);
			FindViewById<ImageView> (Resource.Id.profileIv).Click+=personalData_OnClick;
			dummyBtn= FindViewById<Button> (Resource.Id.dummyType);
			startBtn = FindViewById<Button> (Resource.Id.StartPractice);
			btnStopPractice = FindViewById<Button> (Resource.Id.stopPractice);
			btnSelectBike = FindViewById<Button> (Resource.Id.btnSlectBike);
			btnSelectRun = FindViewById<Button> (Resource.Id.btnSelectRun);
			speedLbl=FindViewById<TextView> (Resource.Id.speedTv);
			btnLL=FindViewById<Button> (Resource.Id.btnLL);
			btnLapDist=FindViewById<Button> (Resource.Id.btnLD);
			selectLbl = FindViewById<Button> (Resource.Id.btnq);
			profile=FindViewById<ImageView> (Resource.Id.profileIv);
			timerBtn=FindViewById<Button> (Resource.Id.btnTotalTime);
			btnStopPractice.Enabled = false;
			this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
			svc = new trackSvc.Service1();
			android_id = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

			var contextEdit = contextPref.Edit();

			wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient(new WebViewClient());
			string nickName = contextPref.GetString("storedUserName", "");
			isStarted = false;
			isPaused = false;
			_timer = new System.Timers.Timer();
			//Trigger event every second
			_timer.Interval = 1000;
			_timer.Elapsed += OnTimedEvent;
			//count down 5 seconds
			duration=0;
			lapDuration = 0;
			startBtn.Enabled = false;
			startBtn.SetBackgroundResource (Resource.Drawable.transparent);
			btnStopPractice.SetBackgroundResource (Resource.Drawable.transparent);
			dummyBtn.SetBackgroundResource (Resource.Drawable.transparent);
			_timer.Enabled = true;
			dummyBtn.Enabled = false;
			btnLL.Enabled = false;
			btnLapDist.Enabled = false;
			btnLL.Text = "";
			btnLapDist.Text = "";
			timerBtn.Text = "";
			distForLap = 0f;
			//ExportBitmapAsPNG2 ();
			tempTime=DateTime.Now;
			updateRecord = new handleRecord ();
			internetavilable = connectivityManager.ActiveNetworkInfo;

			isOnline = (internetavilable != null) && internetavilable.IsConnected;
			wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient (new WebViewClient ());
			contextPref = Application.Context.GetSharedPreferences ("goheja", FileCreationMode.Private);
			nickName = contextPref.GetString ("storedUserName", "");
			contextEdit.PutString ("isOnline", "online");





			///-------------handle the athId
			try
			{
				string[] athData = test.getAthDataByDeviceId(_deviceId);
				contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
				contextEdit = contextPref.Edit();

				contextEdit.PutString("storedFirstName", athData[0].ToString());
				contextEdit.PutString("storedAthId", athData[2].ToString());
				contextEdit.PutString("storedLastName", athData[1].ToString());
				contextEdit.PutString("storedCountry", athData[3].ToString());
				contextEdit.PutString("storedUserName", athData[4].ToString());
				contextEdit.PutString("storedPsw", athData[5].ToString());
				///////
				contextEdit.PutFloat("lastAlt", 0f);
				contextEdit.PutFloat("gainAlt", 0f);
				contextEdit.PutFloat("distance", 0f);
				contextEdit.PutString("prevLoc", "");
				///////
				contextEdit.Commit();
				athId = (contextPref.GetString("storedAthId", "0").ToString());
				athFirstName = contextPref.GetString("storedFirstName", "");
				athLastName = contextPref.GetString("storedLastName", "");
				athCountry = contextPref.GetString("storedCountry", "");
				//athUserName = contextPref.GetString("storedUserName", "");
				//athPsw = contextPref.GetString("storedPsw", "");
				notificationManager.Notify(1, CreateNotification());
				setBitmapImg();




			}
			catch (Exception err)
			{
				string x = err.ToString();

			}


			//---------------the ath id is stored and can be used anywere in the app

		




           

        }

        private void btnExit_OnClick(object sender, EventArgs e)
        {
			
			startEvent.Enabled = true;
			DateTime dt = DateTime.Now;
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			var contextEdit = contextPref.Edit();
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			if (isStarted) {
				isStarted = false;
				isPaused = false;
				try { //there might be times where user will exit with params bot initiated
					svc.updateMomgoData (athFirstName + " " + athLastName,String.Format ("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, 0f, true, athId, athCountry, prefs.GetFloat ("dist", 0f), true, gainAlt, true, _currentLocation.Bearing, true, 2, true,type);
				} catch (Exception err) {
					string t = err.ToString ();
				}
			}
			lastAlt = 0;
			dist = 0;
			gainAlt=0;


			contextEdit.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			contextEdit.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			contextEdit.PutFloat ("dist", 0f);
			prefs.Edit().PutFloat("dist",0f).Commit();
            OnDestroy();
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
			Finish ();
        }
		private void meCalendar_OnClick(object sender, EventArgs e)
		{
			//var activity2go = new Intent(this, typeof(calen));
			//activity2go.PutExtra("MyData", "Data from Activity1");
			//StartActivity(activity2go);
			

			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			string nickName = contextPref.GetString("storedUserName", "");
			var uri = Android.Net.Uri.Parse ("http://go-heja.com/4f/mobda.php?userNickName=" + nickName+"&userId="+contextPref.GetString("storedAthId",""));
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);
		}



		private void StartPractice_OnClick(object sender, EventArgs e)
		{
			
			btnLapDist.Enabled = true;

			btnLL.Enabled = true;
			
			var contextPref = Application.Context.GetSharedPreferences ("goheja", FileCreationMode.Private);
			string nickName = contextPref.GetString ("storedUserName", "");
			//////////////////////////////////////////////////
			var wv = FindViewById<WebView> (Resource.Id.wvCurrentEvent);
			wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient (new WebViewClient ());
			wv.LoadUrl ("http://go-heja.com/4f/mobongoing.php?txt=" + nickName);
			//////////////////////////////////////////////////
			if (isPaused) {
				startBtn.SetBackgroundResource (Resource.Drawable.pauseBtn);
				isPaused = false;
				//btnStopPractice.SetBackgroundResource (Resource.Drawable.white);
				btnStopPractice.Enabled = false;
				btnStopPractice.SetBackgroundResource (Resource.Drawable.transparent);

			} else {
				if (!isStarted) {
					

					isPaused = false;
					startBtn.SetBackgroundResource (Resource.Drawable.pauseBtn);
					//btnStopPractice.SetBackgroundResource (Resource.Drawable.white);
					btnStopPractice.Enabled = false;
					btnStopPractice.SetBackgroundResource (Resource.Drawable.transparent);

					DateTime dt = DateTime.Now;
					try { //there might be times where user will exit with params bot initiated
						svc.updateMomgoData (athFirstName + " " + athLastName, String.Format ("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, 0f, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 1, true,type);
					} catch {
					}
					wv = FindViewById<WebView> (Resource.Id.wvCurrentEvent);
					wv.Settings.JavaScriptEnabled = true;
					wv.SetWebViewClient (new WebViewClient ());
					contextPref = Application.Context.GetSharedPreferences ("goheja", FileCreationMode.Private);
					nickName = contextPref.GetString ("storedUserName", "");
					wv.LoadUrl ("http://go-heja.com/4f/mobongoing.php?txt=" + nickName);
					isStarted = true;
				} else {
					
					startBtn.SetBackgroundResource (Resource.Drawable.resume);
					btnStopPractice.SetBackgroundResource (Resource.Drawable.stop);
					btnStopPractice.Enabled = true;
					//stopLocationService ();
					isPaused = true;
			
				}
			}
		}
		private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!isPaused & isStarted) {
				duration++;
				lapDuration++;
			}
			var timespan = TimeSpan.FromSeconds(duration); 
			var lapTimeSpan =TimeSpan.FromSeconds(lapDuration); 
		    
			RunOnUiThread (() => {
			//	timer.Text = timespan.ToString(@"hh\:mm\:ss");

				timerBtn.Text=timespan.ToString(@"hh\:mm\:ss");

			//	if (type=="bike" & (distForLap%5)<0.01 &!isPaused & isStarted)
			//		{
			//		btnLL.Text="Last lap :"+lapTimeSpan.ToString(@"hh\:mm\:ss");
			//		lapDuration=0;
			//		}
			//	if (type=="bike" & (distForLap%5)<0.01 &!isPaused & isStarted)
			//	{
			//		btnLL.Text="Last lap :"+lapTimeSpan.ToString(@"hh\:mm\:ss");
			//		lapDuration=0;
			//	}
			});


		}

		private void selectDevice_OnClick(object sender, EventArgs e)
		{
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			string nickName = contextPref.GetString("storedUserName", "");
			var uri = Android.Net.Uri.Parse ("http://go-heja.com/gh/mob/sync.php?userId=" + contextPref.GetString ("storedAthId", "0").ToString () + "&mog=4f&url=uurrll");
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);
		}

		private void selectRun_OnClick(object sender, EventArgs e)
		{
			type = "run";

			dummyBtn.SetBackgroundResource (Resource.Drawable.runRound);
			selectLbl.Enabled = false;
			selectLbl.SetBackgroundResource (Resource.Drawable.transparent);
			startBtn.SetBackgroundResource (Resource.Drawable.btn_go);
			dummyBtn.SetBackgroundResource (Resource.Drawable.transparent);
			dummyBtn.Enabled = true;
			startBtn.Enabled = true;
			speedLbl.Text = "min/km";
			btnSelectRun.Enabled = false;
			btnSelectRun.SetBackgroundResource (Resource.Drawable.transparent);
			selectLbl.Enabled = false;
			selectLbl.SetBackgroundResource (Resource.Drawable.transparent);
			selectLbl.Text = "";
			btnSelectBike.SetBackgroundResource (Resource.Drawable.transparent);
			dummyBtn.SetBackgroundResource (Resource.Drawable.runRound);




			//btnSelectRun.Height = "40dp";
		}
		private void setImage_OnClick(object sender, EventArgs e)
		{
			var imageIntent = new Intent ();
			imageIntent.SetType ("image/*");
			imageIntent.SetAction (Intent.ActionGetContent);
			StartActivityForResult (
			Intent.CreateChooser (imageIntent, "Select photo"), 0);
		}
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode == Result.Ok) {
				var imageView =
					FindViewById<ImageView> (Resource.Id.imageView1);
				imageView.SetImageURI (data.Data);
			}
		}
		private void selectBike_OnClick(object sender, EventArgs e)
		{
			type = "bike";


			selectLbl.Enabled = false;
			selectLbl.SetBackgroundResource (Resource.Drawable.transparent);
			btnSelectBike.Enabled = false;
			btnSelectBike.SetBackgroundResource (Resource.Drawable.transparent);
			btnSelectRun.Enabled = false;
			btnSelectRun.SetBackgroundResource (Resource.Drawable.transparent);
			selectLbl.Enabled = false;
			selectLbl.SetBackgroundResource (Resource.Drawable.transparent);
			startBtn.SetBackgroundResource (Resource.Drawable.btn_go);

			dummyBtn.Enabled = true;
			dummyBtn.SetBackgroundResource (Resource.Drawable.bikeRound);
			startBtn.Enabled = true;
			speedLbl.Text = "km/h";
			btnSelectRun.Enabled = false;
			btnSelectBike.SetBackgroundResource (Resource.Drawable.transparent);
			selectLbl.Enabled = false;
			selectLbl.SetBackgroundResource (Resource.Drawable.transparent);
			selectLbl.Text = "";
		}
		private void stopPractice_OnClick(object sender, EventArgs e)
		{
			
			isStarted = false;
			isPaused = false;
			duration = 0;
			//btnStopPractice.SetBackgroundResource (Resource.Drawable.white);
			btnStopPractice.Enabled = false;
			btnStopPractice.SetBackgroundResource (Resource.Drawable.transparent);
			startBtn.SetBackgroundResource (Resource.Drawable.btn_go);
			this.Title = "4Fitness Live is ready...";

			startEvent.Enabled = true;
			DateTime dt = DateTime.Now;
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			var contextEdit = contextPref.Edit();
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			trackSvc.Service1 test=new trackSvc.Service1();
			try //there might be times where user will exit with params bot initiated
			{
				svc.updateMomgoData(athFirstName+" "+athLastName, String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, 0f, true, athId, athCountry,prefs.GetFloat ("dist", 0f),true,gainAlt,true,_currentLocation.Bearing,true,2,true,type);
			}
			catch (Exception err){
				string t = err.ToString ();
			}
			lastAlt = 0;
			dist = 0;
			gainAlt=0;


			contextEdit	.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			contextEdit.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			contextEdit.PutFloat ("dist", 0f);
			prefs.Edit().PutFloat("dist",0f).Commit();
			var wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);
			wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient(new WebViewClient());
			string nickName = contextPref.GetString("storedUserName", "");
			//wv.LoadUrl("http://go-heja.com/goheja/mobongoing.php?txt=" + nickName);
			//wv.LoadUrl ("http://go-heja.com/4f/weather/weather/ws/ws.html");//("http://go-heja.com/surfski/weather/weather/ws/ws.html");
			wv.LoadUrl ("");

			dummyBtn.Enabled = false;
			dummyBtn.SetBackgroundResource (Resource.Drawable.transparent);
			btnLapDist.Text = "";
			btnLapDist.Enabled = false;
			btnLapDist.SetBackgroundResource (Resource.Drawable.transparent);
			btnLL.Text = "";
			btnLL.Enabled = false;
			btnLL.SetBackgroundResource (Resource.Drawable.transparent);
			timerBtn.Text = "";
			timerBtn.Enabled = false;
			timerBtn.SetBackgroundResource (Resource.Drawable.transparent);
			btnLapDist.Enabled = false;
			btnLapDist.SetBackgroundResource (Resource.Drawable.transparent);
			btnLL.Enabled = false;
			btnLL.SetBackgroundResource (Resource.Drawable.transparent);
			btnSelectBike.Enabled = true;
			btnSelectBike.SetBackgroundResource (Resource.Drawable.bikeRound);
			btnSelectRun.SetBackgroundResource (Resource.Drawable.runRound);
			btnSelectRun.Enabled = true;
			startBtn.Enabled = false;
			startBtn.SetBackgroundResource (Resource.Drawable.transparent);
			btnSelectBike.Enabled = true;
			selectLbl.Enabled = true;
			selectLbl.Text = "What's it gonna be?";

		}



		private void startLocationService()
		{
			this.Title = "Searching for GPS...";
			// This event fires when the ServiceConnection lets the client (our App class) know that
			// the Service is connected. We use this event to start updating the UI with location
			// updates from the Service
			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
				//Log.Debug (logTag, "ServiceConnected Event Raised");
				// notifies us of location changes from the system
				App.Current.LocationService.LocationChanged += HandleLocationChanged;
				//notifies us of user changes to the location provider (ie the user disables or enables GPS)
				App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
				App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
				// notifies us of the changing status of a provider (ie GPS no longer available)
				App.Current.LocationService.StatusChanged += HandleStatusChanged;
				 

			} ;

		}






        public Notification CreateNotification()
        {
			
            var contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);
            var builder = new Notification.Builder(this)
                .SetContentTitle("4Fitness on the go")
				.SetSmallIcon(Resource.Drawable.Icon)
                .SetPriority(1)
               .SetContentIntent(contentIntent)
                .SetCategory("tst")
				.SetContentText(string.Format("4Fitness is now tarcking practice"));
            Notification n = builder.Build();
            n.Flags |= NotificationFlags.NoClear;
            return n;

            
        }
       
		private void personalData_OnClick(object sender, EventArgs eventArgs)
		{
			
			
			var activity2go = new Intent(this, typeof(personalData));
			activity2go.PutExtra("MyData", "Data from Activity1");
			StartActivity(activity2go);

		}
        

        public void initiatAth()
        {
			connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);
			notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			activeConnection = connectivityManager.ActiveNetworkInfo;
			isOnline = (activeConnection != null) && activeConnection.IsConnected;

			if (!isOnline) {
				//Toast.MakeText(this, "No internet connection!", ToastLength.Long).Show();
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("No internet connection");
				builder.SetMessage("Oops!No internet connection... Pls try again later");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
				return;
			}
			trackSvc.Service1 test = new trackSvc.Service1();
            string deviceId = "0";
            try
            {
                deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
            }
            catch (Exception err)
            {
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("4Fitness service is not available");
				builder.SetMessage("Oops!Service not available... Pls try again later");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
				return;
            }


            if (deviceId == "0")
            {
				isNewUser = true;
                var activity2 = new Intent(this, typeof(listingActivity));
                //activity2.PutExtra("MyData", "Data from Activity1");
                StartActivity(activity2);
            }
            else
            {

            }

        }
		#region Android Location Service methods

		///<summary>
		/// Updates UI with location data
		/// </summary>
		TimeSpan ts=new TimeSpan(0,0,20);

		public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
		{
			string nickName=contextPref.GetString ("storedUserName","");
			string status="online";

			//record rec = new record ();
			if (!isStarted) {
				this.Title = "4Fitness ready...";
				_speedText.Text="0.0";
				_altitudeText.Text = "0.0";
				//_GradientText.Text = "Bearing";
				_distance.Text = "0.0";
				
			} else {
				
				//var contextPref = Application.Context.GetSharedPreferences ("goheja", FileCreationMode.Private);
				location = e.Location;
				//Log.Debug (logTag, "Foreground updating");
				if (location.Latitude.ToString () != "") {
					this.Title = "On the go";
				}
				try {
					// these events are on a background thread, need to update on the UI thread
					RunOnUiThread (() => {

						ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (ApplicationContext);


						_currentLocation = location;

						///////////////////////////
						if (!isPaused)
						{
						if (_lastLocation != null & _currentLocation != null) {
							dist = prefs.GetFloat ("dist", 0f) + _currentLocation.DistanceTo (_lastLocation) / 1000;

						}			
						//dist = prefs.GetFloat("dist", 0f) + float.Parse(DistanceBetween(_lastLocation, _currentLocation).ToString());
						lastAlt = prefs.GetFloat ("lastAlt", 0f);
						float dAlt = difAlt (lastAlt, float.Parse (_currentLocation.Altitude.ToString ()));
						if (dAlt < 4)
							gainAlt = gainAlt + dAlt;
							

						prefs.Edit ().PutFloat ("gainAlt", gainAlt).Commit ();
						}
						///////////////////////////

						///////////////////////////////////
						if (_currentLocation == null) {
							_locationText.Text = "Unable to determine your location.";
						} else {
							if (true) {//(_currentLocation.Speed>0.3)
						
								if (type=="bike")
								{
								_speedText.Text = (location.Speed * 3.6f).ToString ("0.00") ;
								}
								if (type=="run"&location.Speed>0.1)
								{
									_speedText.Text = (16.6666/(location.Speed)).ToString ("0.00") ;
								}
								if (location.Speed<0.1)
								{
									_speedText.Text = "0.00";
								}
								_altitudeText.Text = gainAlt.ToString ("0.00");
								//_GradientText.Text = "   " + util.getDirection (_currentLocation.Bearing);
								_distance.Text = (dist).ToString ("0.00");
								distForLap=dist;
								if (type=="run")
								{
									btnLapDist.Text="Lap distance : "+(dist%1).ToString("0.00");
									if (dist%1<0.01)
									{
										if (DateTime.Now-tempTime>ts)
										{
										tempTime=DateTime.Now;	
										vibrate(1000);
										}
									}


								}
								if (type=="bike")
								{
									btnLapDist.Text="Lap distance : "+(dist%5).ToString("0.00");
									if (dist%5<0.01)
									{
										if (DateTime.Now-tempTime>ts)
										{
											tempTime=DateTime.Now;	
											vibrate(1000);
										}
									}


								}

								athFirstName = contextPref.GetString ("storedFirstName", "");
								athLastName = contextPref.GetString ("storedLastName", "");

								////----------------------------------------------------------

								DateTime dt = DateTime.Now;


								android_id = Android.Provider.Settings.Secure.GetString (this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

								float speed = float.Parse (_currentLocation.Speed.ToString ());
								speed = speed * 3.6f;

								if (!isPaused)
								{
									internetavilable = connectivityManager.ActiveNetworkInfo;
									record merecord =new record(athFirstName + " " + athLastName, _currentLocation.Latitude, _currentLocation.Longitude,dt,android_id,athId,athCountry,dist,speed,gainAlt,_currentLocation.Bearing,0,type);

									//rec.setRecord(athFirstName + " " + athLastName, _currentLocation.Latitude, _currentLocation.Longitude,dt,android_id,athId,athCountry,dist,speed,gainAlt,_currentLocation.Bearing,0,type);
									
									status=updateRecord.updaterecord(merecord,(internetavilable != null) && internetavilable.IsConnected);//the record and is there internet connection
									// string temp = test.updateData(athFirstName+" "+athLastName, String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, speed, true,athId,true, athCountry);
								//svc.updateMomgoData (athFirstName + " " + athLastName, String.Format ("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, speed, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 0, true,type);
								}

								////----------------------------------------------------------
								_lastLocation = _currentLocation;
								prefs.Edit ().PutFloat ("lastAlt", float.Parse (_currentLocation.Altitude.ToString ())).Commit ();
								prefs.Edit ().PutFloat ("dist", dist).Commit (); 


								//for map refresh 
								if (fFlag == 1||status=="backFromOffline") {//i want to load the nav map only if i allready have points
									//internetavilable != null) && internetavilable.IsConnected


									wv.LoadUrl ("http://go-heja.com/4f/mobongoing.php?txt=" + nickName);
									status="online";

								}
								fFlag = 0;

							}
						}
					});
				} catch (Exception err) {
					string temp = err.ToString ();
				}
			}
		}

		public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
		{
			this.Title=  "GPS disabled";
		}

		public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
		{
			this.Title= "GPS enabled";
		}

		public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
		{
			this.Title ="GPS low signal";
		}
		public void vibrate (long time)
		{
			
			Vibrator vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
			vibrator.Vibrate(time);
		}

		#endregion	

        public static float difAlt(float prev, float curr)
        {
            try
            {
                if ((curr - prev) > 0)
                {
                    return curr - prev;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }

        }
		void setBitmapImg ()
		{


			try
			{
				var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
				var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejamob4fitness/files/me.png");
				var s2 = new FileStream(filePath, FileMode.Open);
				Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
				profile.SetImageBitmap(bitmap2);
				s2.Close();
		   
			}
			catch (Exception err)
			{
				
			}
			finally 
			{
				//s2.Close();

			}

			

		}

		void ExportBitmapAsPNG2()
		{
			try
			{
				Bitmap bitmap = BitmapFactory.DecodeResource(null,Resource.Drawable.ninja);
				var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
				var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.goheja4mobfitness/files/me.png");

				var stream = new FileStream(filePath, FileMode.Create);

				bitmap.Compress (Bitmap.CompressFormat.Png, 100, stream);// Bitmap.CompressFormat.Png, 100, stream);
				stream.Close();

				GC.Collect ();
			}
			catch (Exception err)
			{
				Toast.MakeText(this,err.ToString(), ToastLength.Long).Show();
			}


		}





    }


}

