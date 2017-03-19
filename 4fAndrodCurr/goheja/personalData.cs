
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.IO;







namespace goheja
{
	[Activity (Label = "Personal data")]			
	public class personalData : Activity
	{
		Button imageBtn;
		ImageView meImage;
		TextView tvFirstName;
		TextView tvLastName;
		TextView tvEmail;
		TextView tvPhone;
		Button btnSave;
		Bitmap serverImg;	
		Button seriousBtn;
		byte[] bitmapByteData = { 0 };

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView(Resource.Layout.personalDetails);
			FindViewById<Button> (Resource.Id.perImage).Click+=setImage_OnClick;
			FindViewById<Button> (Resource.Id.btnSave).Click+=saveData_OnClick;
			tvFirstName = FindViewById<TextView> (Resource.Id.etFirstName);
			tvLastName = FindViewById<TextView> (Resource.Id.etlastName);
			tvEmail = FindViewById<TextView> (Resource.Id.etMail);
			tvPhone = FindViewById<TextView> (Resource.Id.etPhone);
			seriousBtn=FindViewById<Button> (Resource.Id.seriousBtn);
			FindViewById<Button> (Resource.Id.seriousBtn).Click+=seriousBtn__OnClick;


			//imageBtn = FindViewById<Button> (Resource.Id.imageBtn);
			meImage=FindViewById<ImageView> (Resource.Id.ivTest);
			setBitmapImg ();
			setPersonalData ();
			//bitmapByteData [0];


			// Create your application here
		}

		private void saveData_OnClick(object sender, EventArgs e)
		{
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			trackSvc.Service1 sv = new trackSvc.Service1();
			contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);

			sv.updateAthPersonalData (contextPref.GetString ("storedAthId", "0").ToString (), tvFirstName.Text, tvLastName.Text,tvPhone.Text, tvEmail.Text,bitmapByteData);
			var activity2 = new Intent(this, typeof(MainActivity));
			StartActivity(activity2);
		}

		private void setImage_OnClick(object sender, EventArgs e)
		{
			var imageIntent = new Intent ();
			imageIntent.SetType ("image/*");
			imageIntent.SetAction (Intent.ActionGetContent);
			StartActivityForResult (
			Intent.CreateChooser (imageIntent, "Select photo"), 0);
		}
		private void seriousBtn__OnClick(object sender, EventArgs e)
		{

			var activity2go = new Intent(this, typeof(profile));
			activity2go.PutExtra("MyData", "Data from Activity1");
			StartActivity(activity2go);


			/*var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			string nickName = contextPref.GetString("storedUserName", "");
			var uri = Android.Net.Uri.Parse ("http://go-heja.com/gh/profile.php"+"?txt="+nickName+"&source=mobile");
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);*/
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);



			if (resultCode == Result.Ok) {
				
				Bitmap mewbm = NGetBitmap (data.Data);


				//Drawable d = new BitmapDrawable (mewbm);

				string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				Bitmap newBitmap = scaleDown (mewbm,200,true);
				using (var stream = new MemoryStream())
				{
					newBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
					bitmapByteData = stream.ToArray();
				}
				serverImg = newBitmap;
				ExportBitmapAsPNG (GetRoundedCornerBitmap(newBitmap,400));


				//meImage.se//   SetImageURI(data.Data);



			}
		}
		void ExportBitmapAsPNG(Bitmap bitmap)
		{
			try
			{
			var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
			var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejamob4fitness/files/me.png");
			var stream = new FileStream(filePath, FileMode.Create);

			bitmap.Compress (Bitmap.CompressFormat.Png, 100, stream);// Bitmap.CompressFormat.Png, 100, stream);
			stream.Close();
			var s2 = new FileStream(filePath, FileMode.Open);

			//byte[] imageBytes = DBFunctions.getRecipeBlobImage(idOfRecipe);
			Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
			meImage.SetImageBitmap(bitmap2);
			serverImg = bitmap2;
			s2.Close ();
			GC.Collect ();
			}
			catch (Exception err)
				{
				Toast.MakeText(this,err.ToString(), ToastLength.Long).Show();
				}
		}
		private Android.Graphics.Bitmap NGetBitmap(Android.Net.Uri uriImage)
		{
			Android.Graphics.Bitmap mBitmap = null;
			mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriImage);
			return mBitmap;
		
		}
		private static Bitmap GetRoundedCornerBitmap(Bitmap bitmap, int pixels)
		{
			Bitmap output = null;

			try
			{
				output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
				Canvas canvas = new Canvas(output);

				Color color = new Color(66, 66, 66);
				Paint paint = new Paint();
				Rect rect = new Rect(0, 0, bitmap.Width*4/5, bitmap.Height*4/5);
				RectF rectF = new RectF(rect);
				float roundPx = pixels;

				paint.AntiAlias = true;
				canvas.DrawARGB(0, 0, 0, 0);
				paint.Color = color;
				canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

				paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
				canvas.DrawBitmap(bitmap, rect, rect, paint);
			}
			catch (Exception err)
			{
				
				//System.Console.WriteLine ("GetRoundedCornerBitmap Error - " + err.Message);
			}

			return output;
		}

		public static Bitmap scaleDown(Bitmap realImage, float maxImageSize,bool filter) {
			float ratio = Math.Min(
				(float) maxImageSize / realImage.Width,
				(float) maxImageSize / realImage.Width);
			int width =(int) Math.Round((float) ratio * realImage.Width);
			int height = (int)Math.Round((float) ratio * realImage.Width);

			Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width,
				height, filter);
			return newBitmap;
		}
		void setBitmapImg()
		{
			


			try
			{
				var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
				var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejamob4fitness/files/me.png");
				var s2 = new FileStream(filePath, FileMode.Open);
				Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
				meImage.SetImageBitmap(bitmap2);
				s2.Close();

			}
			catch (Exception err)
			{
				
			}
			finally 
			{
				GC.Collect ();
				//s2.Close();

			}



		}
		private void setPersonalData ()
		{
			trackSvc.Service1 sv = new trackSvc.Service1();
			string _dId=Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
			string[] athData = sv.getAthDataByDeviceId(_dId);
			tvFirstName.Text =athData [0].ToString ();
			tvLastName.Text =athData [1].ToString ();
			tvEmail.Text =athData [6].ToString ();
			tvPhone.Text =athData [7].ToString ();


		}

	}
}

