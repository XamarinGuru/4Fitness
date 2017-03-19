using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Drawing;
using System.IO;



namespace location2	
{
	
	partial class userData : UIViewController
	{
		
		UIImage temMeImg;
		UIImagePickerController	imagePicker = new UIImagePickerController ();
		public override void ViewDidLoad ()
		{
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);
			if (!setPersonalData ()) 
			{
				new UIAlertView(null, "Error getting user data!", null, "OK", null).Show();
			}
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;
			try
			{
				if (getImg ()!=null)
				{
				meimg.Image = getImg ();
					temMeImg=getImg ();
				}
			}
			catch (Exception err){
				string test = err.ToString();
			}



		}
		
		public userData (IntPtr handle) : base (handle)
		{
		}



		partial void MeImgBtn_TouchUpInside (UIButton sender)
		{
			
			NavigationController.PresentModalViewController(imagePicker, true);
		}
		protected void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			bool isImage = false;
			switch(e.Info[UIImagePickerController.MediaType].ToString()) {
			case "public.image":
				Console.WriteLine("Image selected");
				isImage = true;
				break;
			case "public.video":
				Console.WriteLine("Video selected");
				break;
			}

			// get common info (shared between images and video)
			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine("Url:"+referenceURL.ToString ());

			// if it was an image, get the other image info
			if(isImage) {
				// get the original image
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if(originalImage != null) {
					// do something with the image
					Console.WriteLine ("got the original image");

					meimg.Image=originalImage;
					temMeImg = originalImage;
					saveImage (originalImage);
					//imageView.Image = originalImage; // display
				}
			} else { // if it's a video
				// get video url
				NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
				if(mediaURL != null) {
					Console.WriteLine(mediaURL.ToString());
				}
			}
			// dismiss the picker
			imagePicker.DismissViewControllerAsync(true);
		}
		void Handle_Canceled (object sender, EventArgs e) {
			imagePicker.DismissViewControllerAsync(true);

		}
		private void saveImage (UIImage img)
		{	
			trackSvc.Service1 sv = new trackSvc.Service1 ();
			try
			{
				var photo = img;//.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
				var documentsDirectory = Environment.GetFolderPath
					(Environment.SpecialFolder.Personal);
				string jpgFilename = System.IO.Path.Combine (documentsDirectory, "meImg.jpg"); // hardcoded filename, overwritten each time
				NSData imgData = photo.AsJPEG();

				NSError err = null;
				if (imgData.Save(jpgFilename, false, out err)) {
					//Console.WriteLine("saved as " + jpgFilename);
					//new UIAlertView(null, "saved as " + jpgFilename, null, "OK", null).Show();
				} else {
					//Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
					new UIAlertView(null, "NOT saved as " + jpgFilename + " because" + err.LocalizedDescription, null, "OK", null).Show();
				}
			}	
			catch (Exception err)
			{
				bool out1 = false;
				bool out2 = false;
				sv.getErrprFromMobile (err.ToString (),NSUserDefaults.StandardUserDefaults.StringForKey ("id").ToString(),out out1,out out2);
				new UIAlertView(null, "Save error", null, "OK", null).Show();

			}

		}
		private UIImage getImg()
		{
			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string jpgFilename = System.IO.Path.Combine (documentsDirectory, "meImg.jpg");

			UIImage currentImage = UIImage.FromFile (jpgFilename);
			return currentImage;
			
		}
		private bool setPersonalData ()
		{
			try
			{
			trackSvc.Service1 sv = new trackSvc.Service1();
			string _dId =	NSUserDefaults.StandardUserDefaults.StringForKey ("deviceId");
			string[] athData = sv.getAthDataByDeviceId(_dId);
			firstNameTB.Text =athData [0].ToString ();
			lastNameTB.Text =athData [1].ToString ();
			emailTB.Text =athData [6].ToString ();
		    phoneTB.Text =athData [7].ToString ();
				return true;
			}
			catch 
			{
				return false;
			}


		}
		private bool savedata()
		{
			trackSvc.Service1 sv = new trackSvc.Service1 ();
			try {
			Byte[] myByteArray;
			using (NSData imageData = MaxResizeImage(temMeImg, 90f,90f).AsPNG()) {
				myByteArray = new Byte[imageData.Length];
				System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));
			}
			//source = Xamarin.Forms.ImageSource.FromStream(() => ((UIImageView)temMeImg).Image.AsPNG().AsStream());
			int tempp=myByteArray.Length;


				sv.updateAthPersonalData (NSUserDefaults.StandardUserDefaults.StringForKey ("id").ToString(), firstNameTB.Text, lastNameTB.Text, phoneTB.Text,emailTB.Text,myByteArray);
				return true;
			} catch (Exception err) {
				bool out1=false;
				bool out2 = false;

				sv.getErrprFromMobile (err.ToString (),NSUserDefaults.StandardUserDefaults.StringForKey ("id").ToString(),out out1,out out2);
				return false;
			}
		
		}

		partial void BtnGo_TouchUpInside (UIButton sender)
		{
			if (!savedata())
			{
				new UIAlertView(null, "Error saving data!", null, "OK", null).Show();
			}
			else
			{
				new UIAlertView(null, "Data Saved! We hope you got serious...", null, "OK", null).Show();
			}
		}
		public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			try
			{
				
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new SizeF((float) width,(float) height));
			sourceImage.Draw(new RectangleF(0, 0,(float) width,(float) height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
			}
			catch 
			{
				return null;
			}
		}

		partial void SeriuosBtn_TouchUpInside (UIButton sender)
		{
			NSUserDefaults.StandardUserDefaults.SetString ("userdata", "source");
			UIcalendar calendarPage = this.Storyboard.InstantiateViewController ("UIcalendar") as UIcalendar;
			this.NavigationController.PushViewController (calendarPage, true);
			//this.NavigationController.PushViewController (calendarPage, true);
			//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/gh/profile.php?txt="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")+"&source=mobile"));
		}
	}
}
