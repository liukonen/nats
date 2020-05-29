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
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem mnIndexOnly { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnIndexWLoad { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnMultiLine { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnMultiThread { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnRam { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnSingleThread { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnSmart_click { get; set; }

		[Action ("help_click:")]
		partial void help_click (Foundation.NSObject sender);

		[Action ("mnIndex_click:")]
		partial void mnIndex_click (Foundation.NSObject sender);

		[Action ("mnIndexLoad_Click:")]
		partial void mnIndexLoad_Click (Foundation.NSObject sender);

		[Action ("mnMultiline_click:")]
		partial void mnMultiline_click (Foundation.NSObject sender);

		[Action ("mnMultiThread_click:")]
		partial void mnMultiThread_click (Foundation.NSObject sender);

		[Action ("mnRam_Click:")]
		partial void mnRam_Click (Foundation.NSObject sender);

		[Action ("mnSingleThread_click:")]
		partial void mnSingleThread_click (Foundation.NSObject sender);

		[Action ("mnSmartSearch_click:")]
		partial void mnSmartSearch_click (Foundation.NSObject sender);

		[Action ("Save_click:")]
		partial void Save_click (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (mnIndexOnly != null) {
				mnIndexOnly.Dispose ();
				mnIndexOnly = null;
			}

			if (mnIndexWLoad != null) {
				mnIndexWLoad.Dispose ();
				mnIndexWLoad = null;
			}

			if (mnMultiLine != null) {
				mnMultiLine.Dispose ();
				mnMultiLine = null;
			}

			if (mnMultiThread != null) {
				mnMultiThread.Dispose ();
				mnMultiThread = null;
			}

			if (mnRam != null) {
				mnRam.Dispose ();
				mnRam = null;
			}

			if (mnSingleThread != null) {
				mnSingleThread.Dispose ();
				mnSingleThread = null;
			}

			if (mnSmart_click != null) {
				mnSmart_click.Dispose ();
				mnSmart_click = null;
			}
		}
	}
}
