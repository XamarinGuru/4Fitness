using System;
using Foundation;
using UIKit;
using CoreLocation;
using System.Threading.Tasks;


namespace location2
{
	
	public partial class ViewController : UIViewController
	{
		

		trackSvc.Service1 meServ;
		string deviceId;
		string[] athData;
		Reachability.Reachability connection;
		public bool startStop;
		public bool paused;
		bool toolStatus=false;
		string selected="";
		bool isTimerStarted=false;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			stopBtn.Enabled = false;
			startStop = false;
			paused = false;
			this.Title = "4Fitness live";
			connection = new Reachability.Reachability ();
			if (!connection.IsHostReachable("www.google.com")) 
			{
				new UIAlertView(null, "No internet connection!", null, "OK", null).Show();
				this.Title = "No internet connection..";
				return;
			}

			else
			{
				UIApplication.SharedApplication.IdleTimerDisabled = true;
				///check if device exists in service
				/// if not - list the user using vcListing
				var id = default(string);
				id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();//check the device id
				meServ = new location2.trackSvc.Service1 ();
				try{
					deviceId=meServ.getListedDeviceId (id);
				}
				catch{
					new UIAlertView(null, "You are not connected to Lamdan services...", null, "OK", null).Show();
					deviceId = "tempDeviceId";
				}

				if (deviceId == "0") {
					vcListing controller = this.Storyboard.InstantiateViewController ("vcListing") as vcListing;
					this.NavigationController.PushViewController (controller, true);
				} else {


					athData = meServ.getAthDataByDeviceId (NSUserDefaults.StandardUserDefaults.StringForKey("deviceId"));
					if (athData == null) athData = meServ.getAthDataByDeviceId(id);

					NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");
					NSUserDefaults.StandardUserDefaults.SetString (athData [0].ToString (), "firstName");
					NSUserDefaults.StandardUserDefaults.SetString (athData [1].ToString (), "lastName");
					NSUserDefaults.StandardUserDefaults.SetString (athData [2].ToString (), "id");
					NSUserDefaults.StandardUserDefaults.SetString (athData [3].ToString (), "country");
					NSUserDefaults.StandardUserDefaults.SetString (athData [4].ToString (), "userName");
					NSUserDefaults.StandardUserDefaults.SetString (athData [5].ToString (), "password");


				}
				altimg.Layer.ZPosition = 1;
				bpmImg.Layer.ZPosition = 1;
				distImg.Layer.ZPosition = 1;
				wattImg.Layer.ZPosition = 1;
				speedImg.Layer.ZPosition = 1;

				lblSpeed.Layer.ZPosition = 1;
				bpmLbl.Layer.ZPosition = 1;
				wattLbl.Layer.ZPosition = 1;
				lblDist.Layer.ZPosition = 1;
				lblAlt.Layer.ZPosition = 1;

				speedTypeLbl.Layer.ZPosition = 1;
				bpmValueLbl.Layer.ZPosition = 1;
				wattLbl.Layer.ZPosition = 1;
				distTypLbl.Layer.ZPosition = 1;
				altTypeLbl.Layer.ZPosition = 1;

				CalenBtn.Layer.ZPosition = 1;

				meBtn.Enabled = false;
				meBtn.Hidden = true;
				watchBtn.Enabled = false;
				watchBtn.Hidden = true;
				CalenBtn.Enabled = false;
				CalenBtn.Hidden = true;

				startStopBtn.Enabled = false;
				startStopBtn.Hidden = true;


				selectedBtn.Enabled = false;
				selectedBtn.Hidden = true;





				///user is checked

				///wv in main page will be used for weather when there is no event
				/// and ongoing map while event
				//var url = "http://google.com";
				//wvOngoing.LoadRequest(new NSUrlRequest(new NSUrl(url)));

				//Manager.LocationUpdated += HandleLocationChanged;
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#region Computed Properties
		public static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public static LocationManager Manager { get; set;}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
			// As soon as the app is done launching, begin generating location updates in the location manager
			Manager = new LocationManager();
			Manager.StartLocationUpdates();
		}

		#endregion

		#region Public Methods
		CLLocation _lastLocation;
		double _currentDistance=0;
		Double _lastAltitude;
		DateTime _dt;
		double _speed;
		string _bearing;
		float currdistance = 0;
		int flag=0;

		public void HandleLocationChanged (object sender, LocationUpdatedEventArgs e)
		{
			if (flag == 2) 
			{
				var url = "http://go-heja.com/4f/mobongoingApl.php?txt="+NSUserDefaults.StandardUserDefaults.StringForKey("userName"); // NOTE: https secure request
				wvOngoing.LoadRequest(new NSUrlRequest(new NSUrl(url)));
			}
			if (startStop) {
				if (this.Title.Contains ("GPS")) {
					this.Title = "On the go";
				}
				// Handle foreground updates
				CLLocation location = e.Location;

				if (!paused) {
					try {//we might have coordinat thta is not initiated
						//_currentDistance = _currentDistance  + calculate.distance(location.Coordinate.Latitude,location.Coordinate.Longitude, _lastLocation.Coordinate.Latitude,_lastLocation.Coordinate.Longitude,'k');
						if (location != null & _lastLocation != null) {
							_currentDistance = _currentDistance + location.DistanceFrom (_lastLocation) / 1000;
						}
						_lastAltitude = NSUserDefaults.StandardUserDefaults.DoubleForKey ("lastAltitude") + calculate.difAlt (_lastAltitude, location.Altitude);
					} catch {
					}
				}
				_bearing = calculate.getDirection (location.Course);
				_dt = DateTime.Now;


				if (selected == "bike") 
				{
					_speed = location.Speed * 3.6;
				}
				if (selected == "run") 
				{
					if (location.Speed > 0) {
						_speed = 16.6666 / location.Speed;
					}
					else 
					{
						_speed = 0;
					}	
				}
				float course = float.Parse (location.Course.ToString ());

				currdistance = float.Parse (_currentDistance.ToString ());
				float currAlt = float.Parse (_lastAltitude.ToString ());
				float currspeed = float.Parse (_speed.ToString ());
				try {
					if (!paused)
					{
						meServ.updateMomgoData (NSUserDefaults.StandardUserDefaults.StringForKey ("firstName").ToString () + " " + NSUserDefaults.StandardUserDefaults.StringForKey ("lastName").ToString (), location.Coordinate.Latitude.ToString () + "," + location.Coordinate.Longitude.ToString (), _dt, true, NSUserDefaults.StandardUserDefaults.StringForKey ("deviceId").ToString (), currspeed, true,(NSUserDefaults.StandardUserDefaults.StringForKey ("id").ToString ()), NSUserDefaults.StandardUserDefaults.StringForKey ("country").ToString (), currdistance, true, currAlt, true, course, true, 0, true,selected);

						if (currspeed < 0)
							currspeed = 0;
						lblSpeed.Text = currspeed.ToString ("0.00");
						lblAlt.Text = currAlt.ToString ("0.00");
						//lblDir.Text = " "+_bearing ;
						lblDist.Text = _currentDistance.ToString ("0.00");
					}

					//end
					//save this used location as last location
					_lastLocation = location;
					NSUserDefaults.StandardUserDefaults.SetDouble (_currentDistance, "lastDistance"); 
					NSUserDefaults.StandardUserDefaults.SetDouble (currAlt, "lastAltitude"); 

				} catch (Exception err) {

				}

			}
			if (flag <=2) {

				flag++;
			}




		}


		/*partial void BtnStress_TouchUpInside (UIButton sender)
		{
			//bool accepted=MessageBox.ShowAsync("ACTIVATE SOS?","",MessageBoxButton.YesNo);
		
				new UIAlertView(null, "Not implemented yet", null, "OK", null).Show();


			//meServ.updateMomgoData(NSUserDefaults.StandardUserDefaults.StringForKey("firstName").ToString()+" "+NSUserDefaults.StandardUserDefaults.StringForKey("lastName").ToString(),_lastLocation.Coordinate.Longitude.ToString()+","+_lastLocation.Coordinate.Latitude.ToString(),_dt,true,NSUserDefaults.StandardUserDefaults.StringForKey("deviceId").ToString(),0.01f,true,int.Parse(NSUserDefaults.StandardUserDefaults.IntForKey("id").ToString()),true,NSUserDefaults.StandardUserDefaults.StringForKey("country").ToString().Trim(),currdistance,true,float.Parse(_lastAltitude.ToString()),true,0f,true,2,true);
				Manager.LocationUpdated -= HandleLocationChanged;
			//new UIAlertView(null, "SOS call", null, null, null).Show();
		}*/


		partial void StartStopBtn_TouchUpInside (UIButton sender)
		{
			
			if (paused)
			{
				startStopBtn.SetBackgroundImage(UIImage.FromFile ("pauseBtn.png"), UIControlState.Normal);
				paused=false;
				stopBtn.SetBackgroundImage(UIImage.FromFile (""), UIControlState.Normal);
				stopBtn.Enabled=false;
				this.Title="On the go...";
			}
			else
			{
				if (!startStop)
				{
					
					if (!isTimerStarted)   StartTimer();
					isTimerStarted=true;
					startStopBtn.SetBackgroundImage(UIImage.FromFile ("pauseBtn.png"), UIControlState.Normal);
					stopBtn.SetBackgroundImage(UIImage.FromFile (""), UIControlState.Normal);
					stopBtn.Enabled=true;
					connection = new Reachability.Reachability ();
					if (!connection.IsHostReachable("www.google.com")) 
					{
						new UIAlertView(null, "No internet connection!", null, "OK", null).Show();
						this.Title="No intenet connecion..."	;
						return;
					}
					else
					{
						if (deviceId == "0") {
							new UIAlertView(null, "You are not listed in 4Fitness Live!", null, "OK", null).Show();
							vcListing controller = this.Storyboard.InstantiateViewController ("vcListing") as vcListing;
							this.NavigationController.PushViewController (controller, true);
						} else {
							this.Title="Searching for GPS...";
							Manager.LocationUpdated += HandleLocationChanged;

							//startBtn.Enabled=false;
							//cant use google maps, untill ill get the hang of app maps ill use the browser in btnMap
							//
							//

						}
					}
					startStop=true;
					paused=false;

				}
				else
				{
					startStopBtn.SetBackgroundImage(UIImage.FromFile ("resume.png"), UIControlState.Normal);
					stopBtn.SetBackgroundImage(UIImage.FromFile ("stop.png"), UIControlState.Normal);
					this.Title="Paused...";
					stopBtn.Enabled=true;
					paused=true;


				}
			}


		}

		//partial void MapBtn_TouchUpInside (UIButton sender)
		//{
		//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/  ski/mob/mobileDay.php?userNickName="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")+"&userId=+"+NSUserDefaults.StandardUserDefaults.IntForKey("id")));
		//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/surfski/mobongoingApl.php?txt="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")));
		//}





		partial void CalenBtn_TouchUpInside (UIButton sender)
		{
			//NSUserDefaults.StandardUserDefaults.SetString ("calen", "source");
			//UIcalendar calendarPage = this.Storyboard.InstantiateViewController ("UIcalendar") as UIcalendar;
			//this.NavigationController.PushViewController (calendarPage, true);
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/4f/mobda.php?userNickName="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")+"&userId=+"+NSUserDefaults.StandardUserDefaults.IntForKey("id")));
			//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/gh/hia.php"));
		}

		partial void StopBtn_TouchUpInside (UIButton sender)
		{
			_duration=0;
			NSUserDefaults.StandardUserDefaults.SetInt (0,"timer");
			timerLbl.Text="";
			startStopBtn.SetBackgroundImage(UIImage.FromFile ("btn_go.png"), UIControlState.Normal);
			startStopBtn.Hidden=true;
			startStopBtn.Enabled=false;
			selectBikeBtn.Hidden=false;
			selectBikeBtn.Enabled=true;
			selectRunBtn.Hidden=false;
			selectRunBtn.Enabled=true;
			selectedBtn.Hidden=true;

			stopBtn.SetBackgroundImage(UIImage.FromFile (""), UIControlState.Normal);
			stopBtn.Enabled=false;
			startStop=false;
			paused=false;

			Manager.LocationUpdated -= HandleLocationChanged;
			this.Title="4Fitness ready...";

			try{
				meServ.updateMomgoData(NSUserDefaults.StandardUserDefaults.StringForKey("firstName").ToString()+" "+NSUserDefaults.StandardUserDefaults.StringForKey("lastName").ToString(),_lastLocation.Coordinate.Latitude.ToString()+","+_lastLocation.Coordinate.Longitude.ToString(),_dt,true,NSUserDefaults.StandardUserDefaults.StringForKey("deviceId").ToString(),float.Parse(_lastLocation.Speed.ToString()),true,(NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString()),NSUserDefaults.StandardUserDefaults.StringForKey("country").ToString(),currdistance,true,float.Parse(NSUserDefaults.StandardUserDefaults.DoubleForKey ("lastAltitude").ToString()),true,float.Parse(_lastLocation.Course.ToString()),true,2,true,selected);
			}
			catch{
			}
			lblAlt.Text="0.0";
			//lblDir.Text=" Direction";
			lblDist.Text="0.00";
			lblSpeed.Text="0.00";
			this.Title="4Fitness ready..";
			_lastLocation=null;
			_currentDistance=0;
			_lastAltitude=0;
			_speed=0;
			_bearing="";
			currdistance=0;
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastDistance"); 
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastAltitude"); 
			flag=0;
			timerLbl.Text="";
			paused=true;
			startStop = true;
		}

		partial void ToolBtn_TouchUpInside (UIButton sender)
		{
			if(toolStatus)
			{
			meBtn.Enabled = true;
			meBtn.Hidden = false;
			watchBtn.Enabled = true;
			watchBtn.Hidden = false;
			CalenBtn.Enabled = true;
			CalenBtn.Hidden = false;
			toolBtn.SetBackgroundImage(UIImage.FromFile ("min.png"), UIControlState.Normal);
				toolStatus=false;
			}
			else
			{
				meBtn.Enabled = false;
				meBtn.Hidden = true;
				watchBtn.Enabled = false;
				watchBtn.Hidden = true;
				CalenBtn.Enabled = false;
				CalenBtn.Hidden = true;	
				toolBtn.SetBackgroundImage(UIImage.FromFile ("plu.png"), UIControlState.Normal);
				toolStatus=true;
		     }
		}

		partial void WatchBtn_TouchUpInside (UIButton sender)
		{
			//NSUserDefaults.StandardUserDefaults.SetString ("watch", "source");
			//UIcalendar calendarPage = this.Storyboard.InstantiateViewController ("UIcalendar") as UIcalendar;
			//this.NavigationController.PushViewController (calendarPage, true);
			//"http://go-heja.com/gh/mob/sync.php?userId=" + contextPref.GetString ("storedAthId", "0").ToString () + "&mog=gh&url=uurrll"
			
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/gh/mob/sync.php?userId=" + NSUserDefaults.StandardUserDefaults.StringForKey("id") + "&mog=4f&url=uurrll"));
		}

		partial void MeBtn_TouchUpInside (UIButton sender)
		{
			userData controller = this.Storyboard.InstantiateViewController ("userData") as userData;
			this.NavigationController.PushViewController (controller, true);
			
		}

		partial void SelectRunBtn_TouchUpInside (UIButton sender)
		{
			speedTypeLbl.Text="min/km";
			startStopBtn.Enabled = true;
			startStopBtn.Hidden = false;
			selected="run";
			selectBikeBtn.Hidden=true;
			selectBikeBtn.Enabled=false;
			selectRunBtn.Hidden=true;
			selectRunBtn.Enabled=false;
			selectedBtn.SetBackgroundImage(UIImage.FromFile ("runRound.png"), UIControlState.Normal);
			selectedBtn.Hidden=false;
			selectedBtn.Enabled=false;
		}

		partial void SelectBikeBtn_TouchUpInside (UIButton sender)
		{
			speedTypeLbl.Text="km/h";
			startStopBtn.Enabled = true;
			startStopBtn.Hidden = false;
			selected="bike";
			selectBikeBtn.Hidden=true;
			selectBikeBtn.Enabled=false;
			selectRunBtn.Hidden=true;
			selectRunBtn.Enabled=false;
			selectedBtn.SetBackgroundImage(UIImage.FromFile ("bikeRound.png"), UIControlState.Normal);
			selectedBtn.Hidden=false;
			selectedBtn.Enabled=false;
		}
		#endregion
		private int _duration = 0;

		public async void StartTimer1() {
			if (startStop) 
			{
				_duration = 0;
			} else 
			{
				_duration = (int)NSUserDefaults.StandardUserDefaults.IntForKey ("timer");
			}

			// tick every second while game is in progress
			while (!paused) {
				
				await Task.Delay (1000);
				_duration++;
				NSUserDefaults.StandardUserDefaults.SetInt (_duration,"timer");
				string s = TimeSpan.FromSeconds(_duration).ToString(@"hh\:mm\:ss");
			
				timerLbl.Text =s;



			}
		}
		async Task StartTimer() {
			_duration = 0;
			while (true) {

				await Task.Delay (1000);
				if(!paused) _duration++;
				NSUserDefaults.StandardUserDefaults.SetInt (_duration,"timer");
				string s = TimeSpan.FromSeconds(_duration).ToString(@"hh\:mm\:ss");

				timerLbl.Text =s;



			}
		}
	
			

		

	}
}

