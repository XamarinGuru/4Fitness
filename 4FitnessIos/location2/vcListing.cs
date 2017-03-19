using Foundation;
using System;
using UIKit;

namespace location2
{
	partial class vcListing : UIViewController
	{
		
		string id = default(string);
		bool username,firstname,password,termsAccepted,email;
		string validationMessage = "Note red fields!";
		trackSvc.Service1 meServ = new trackSvc.Service1 ();
		public vcListing (IntPtr handle) : base (handle)
		{
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.listingView.AccessibilityScroll (UIAccessibilityScrollDirection.Up);
			username = true;
			firstname=true;
			password=true;
			termsAccepted = false;
			id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
			NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId"); 
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);



		}

		partial void ListBtn_TouchUpInside (UIButton sender)
		{
			if (termsAccepted == false)
			{
				new UIAlertView(null, "You didnt accept terms!", null, "OK", null).Show();
				return;
			}
			validate();
			if (password == false || firstname == false || username == false)
			{
				new UIAlertView(null, "Note red fields!", null, "OK", null).Show();

			}
			else
			{
				try
				{

					meServ.insertNewDevice(this.firstNameTexInput.Text, this.lastNameTextInput.Text, id, this.nickeNameTextInput.Text, this.passwordTextInput.Text, true, true, emailText.Text);

					ViewController controller = this.Storyboard.InstantiateViewController("ViewController") as ViewController;
					this.NavigationController.PushViewController(controller, true);

				}
				catch (Exception err)
				{
					new UIAlertView(null, err.ToString(), null, "OK", null).Show();
				}
				//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/gh/hia.php"));

				//ViewController calendarPage = this.Storyboard.InstantiateViewController ("ViewController") as ViewController;
				//this.NavigationController.PushViewController (calendarPage, true);
			}

		}
		private void validate()
		{
			

				// perform a simple "required" validation
			if (firstNameTexInput.Text.Length <= 0) {
				// need to update on the main thread to change the border color
				InvokeOnMainThread (() => {
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.firstNameTexInput.Layer.BorderColor = UIColor.Red.CGColor;
					this.firstNameTexInput.Layer.BorderWidth = 3;
					this.firstNameTexInput.Layer.CornerRadius = 5;
					firstname = false;
				});
			} else {
				firstname = true;
			}
				

				// perform a simple "required" validation
			if ( nickeNameTextInput.Text.Length <= 0 ) {
					// need to update on the main thread to change the border color
					InvokeOnMainThread ( () => {
						//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
						this.nickeNameTextInput.Layer.BorderColor = UIColor.Red.CGColor;
						this.nickeNameTextInput.Layer.BorderWidth = 3;
						this.nickeNameTextInput.Layer.CornerRadius = 5;
						username=false;
					} );
			}else {
				username = true;
			}
			if ( nickeNameTextInput.Text.Length >= 8 ) {
				// need to update on the main thread to change the border color
				InvokeOnMainThread ( () => {
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.nickeNameTextInput.Layer.BorderColor = UIColor.Red.CGColor;
					this.nickeNameTextInput.Layer.BorderWidth = 3;
					this.nickeNameTextInput.Layer.CornerRadius = 5;
					username=false;
				} );
			}else {
				username = true;
			}



				// perform a simple "required" validation
			if ( passwordTextInput.Text.Length <= 0 ) {
					// need to update on the main thread to change the border color
					InvokeOnMainThread ( () => {
						//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
						this.passwordTextInput.Layer.BorderColor = UIColor.Red.CGColor;
						this.passwordTextInput.Layer.BorderWidth = 3;
						this.passwordTextInput.Layer.CornerRadius = 5;
						password=false;
					} );
			}else {
				password = true;
			}

			// perform a simple "required" validation
			if ( !(emailText.Text.Contains("@"))||!(emailText.Text.Contains("."))) {
				// need to update on the main thread to change the border color
				InvokeOnMainThread ( () => {
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.passwordTextInput.Layer.BorderColor = UIColor.Red.CGColor;
					this.passwordTextInput.Layer.BorderWidth = 3;
					this.passwordTextInput.Layer.CornerRadius = 5;
					email=false;
					validationMessage="E-mail not valid!";
				} );
			}else {
				email = true;
			}



		}


		partial void AcceprtBtn_TouchUpInside (UIButton sender)
		{
			if (!validateUserNickName())
			{
				new UIAlertView(null, "Nick name is taken, try another.", null, "OK", null).Show();
				return;
			}
			termsAccepted=true;
			acceprtBtn.SetTitle("Terms accepted!  ",UIControlState.Normal);
		}

		partial void TermsBtn_TouchUpInside (UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/4f/terms.php/"));
		}
		private bool validateUserNickName()
		{
			
			string validate = meServ.validateNickName (nickeNameTextInput.Text);
			if (validate != "1") {
				return true;
			} else {
				return false;
			}
		}
	}
}
