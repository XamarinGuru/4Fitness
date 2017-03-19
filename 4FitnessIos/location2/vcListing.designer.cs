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
    [Register ("vcListing")]
    partial class vcListing
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton acceprtBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField emailText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField firstNameTexInput { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField lastNameTextInput { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton listBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView listingView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField nickeNameTextInput { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField passwordTextInput { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton termsBtn { get; set; }


        [Action ("AcceprtBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AcceprtBtn_TouchUpInside (UIButton sender);


        [Action ("ListBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ListBtn_TouchUpInside (UIButton sender);


        [Action ("TermsBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TermsBtn_TouchUpInside (UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (acceprtBtn != null) {
                acceprtBtn.Dispose ();
                acceprtBtn = null;
            }

            if (emailText != null) {
                emailText.Dispose ();
                emailText = null;
            }

            if (firstNameTexInput != null) {
                firstNameTexInput.Dispose ();
                firstNameTexInput = null;
            }

            if (lastNameTextInput != null) {
                lastNameTextInput.Dispose ();
                lastNameTextInput = null;
            }

            if (listBtn != null) {
                listBtn.Dispose ();
                listBtn = null;
            }

            if (listingView != null) {
                listingView.Dispose ();
                listingView = null;
            }

            if (nickeNameTextInput != null) {
                nickeNameTextInput.Dispose ();
                nickeNameTextInput = null;
            }

            if (passwordTextInput != null) {
                passwordTextInput.Dispose ();
                passwordTextInput = null;
            }

            if (termsBtn != null) {
                termsBtn.Dispose ();
                termsBtn = null;
            }
        }
    }
}