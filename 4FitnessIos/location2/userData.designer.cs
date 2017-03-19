// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
    [Register ("userData")]
    partial class userData
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnGo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField emailTB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField firstNameTB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField lastNameTB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView meimg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton meImgBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField phoneTB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton seriuosBtn { get; set; }


        [Action ("BtnGo_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnGo_TouchUpInside (UIButton sender);


        [Action ("MeImgBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MeImgBtn_TouchUpInside (UIButton sender);


        [Action ("SeriuosBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SeriuosBtn_TouchUpInside (UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnGo != null) {
                btnGo.Dispose ();
                btnGo = null;
            }

            if (emailTB != null) {
                emailTB.Dispose ();
                emailTB = null;
            }

            if (firstNameTB != null) {
                firstNameTB.Dispose ();
                firstNameTB = null;
            }

            if (lastNameTB != null) {
                lastNameTB.Dispose ();
                lastNameTB = null;
            }

            if (meimg != null) {
                meimg.Dispose ();
                meimg = null;
            }

            if (meImgBtn != null) {
                meImgBtn.Dispose ();
                meImgBtn = null;
            }

            if (phoneTB != null) {
                phoneTB.Dispose ();
                phoneTB = null;
            }

            if (seriuosBtn != null) {
                seriuosBtn.Dispose ();
                seriuosBtn = null;
            }
        }
    }
}