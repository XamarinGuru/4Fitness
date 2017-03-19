using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
	partial class UIcalendar : UIViewController
	{
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var url="";
			string id=NSUserDefaults.StandardUserDefaults.StringForKey ( "id");
			string userName=NSUserDefaults.StandardUserDefaults.StringForKey ("userName");
			if (NSUserDefaults.StandardUserDefaults.StringForKey ( "source")=="calen")
			{
			url = "http://go-heja.com/4f/mobda.php?userNickName=" + userName + "&userId=" + id; // NOTE: https secure request
			}
			else
			{
				url = "http://go-heja.com/4f/profile.php?txt=" + userName+ "&userId=" + id; // NOTE: https secure request
			}

		   calendarWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

		}
		public UIcalendar (IntPtr handle) : base (handle)
		{
		}
	}
}
