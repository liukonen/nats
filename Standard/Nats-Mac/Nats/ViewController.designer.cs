// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Nats
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btnSearch { get; set; }

		[Outlet]
		AppKit.NSTextView ResultsTxt { get; set; }

		[Outlet]
		AppKit.NSTextField txtSearch { get; set; }

		[Action ("Search_click:")]
		partial void Search_click (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ResultsTxt != null) {
				ResultsTxt.Dispose ();
				ResultsTxt = null;
			}

			if (txtSearch != null) {
				txtSearch.Dispose ();
				txtSearch = null;
			}

			if (btnSearch != null) {
				btnSearch.Dispose ();
				btnSearch = null;
			}
		}
	}
}
