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
    [Register ("UIcalendar")]
    partial class UIcalendar
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView calendarWebView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (calendarWebView != null) {
                calendarWebView.Dispose ();
                calendarWebView = null;
            }
        }
    }
}