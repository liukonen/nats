using AppKit;
using Foundation;

namespace Nats
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {

        public ViewController NatsGui { get; set; } = null;

        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            NatsGui.Path = DirPicker();
            // Insert code here to initialize your application
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }

        partial void mnSmartSearch_click(NSObject sender)
        {
            mnSmart_click.State = (mnSmart_click.State == NSCellStateValue.On) ? NSCellStateValue.Off : NSCellStateValue.On;
            NatsGui.UseSmartSearch = (mnSmart_click.State == NSCellStateValue.On);
        }
        partial void mnRam_Click(NSObject sender)
        {
            mnRam.State = (mnRam.State == NSCellStateValue.On) ? NSCellStateValue.Off : NSCellStateValue.On;
            NatsGui.UseRam = mnRam.State == NSCellStateValue.On;
        }
        partial void mnMultiline_click(NSObject sender)
        {
            mnMultiLine.State = (mnMultiLine.State == NSCellStateValue.On) ? NSCellStateValue.Off : NSCellStateValue.On;
            NatsGui.UseMultiLine = (mnMultiLine.State == NSCellStateValue.On);
        }
        partial void mnSingleThread_click(NSObject sender)
        {
            mnSingleThread.State = NSCellStateValue.On;
            mnMultiThread.State = NSCellStateValue.Off;
            mnIndexOnly.State = NSCellStateValue.Off;
            mnIndexWLoad.State = NSCellStateValue.Off;
            NatsGui.SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Single;

        }
        partial void mnMultiThread_click(NSObject sender)
        {
            mnSingleThread.State = NSCellStateValue.Off;
            mnMultiThread.State = NSCellStateValue.On;
            mnIndexOnly.State = NSCellStateValue.Off;
            mnIndexWLoad.State = NSCellStateValue.Off;
            NatsGui.SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Threaded;

        }
        partial void mnIndex_click(NSObject sender)
        {
            mnSingleThread.State = NSCellStateValue.Off;
            mnMultiThread.State = NSCellStateValue.Off;
            mnIndexOnly.State = NSCellStateValue.On;
            mnIndexWLoad.State = NSCellStateValue.Off;
            NatsGui.SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex;

        }
        partial void mnIndexLoad_Click(NSObject sender)
        {
            mnSingleThread.State = NSCellStateValue.Off;
            mnMultiThread.State = NSCellStateValue.Off;
            mnIndexOnly.State = NSCellStateValue.Off;
            mnIndexWLoad.State = NSCellStateValue.On;
            NatsGui.SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.indexgenerateandsearch;

        }

        [Export("openDocument:")]
        void OpenDialog(NSObject sender)
        {
            NatsGui.Path = DirPicker();

        }

        static string DirPicker()
        {
            var dialog = new NSOpenPanel()
            {
                Title = "Choose single directory | NATS",
                ShowsResizeIndicator = true,
                ShowsHiddenFiles = false,
                CanChooseDirectories = true,
                CanChooseFiles = false
            };

            if (dialog.RunModal() == 1)
            {
                var result = dialog.Url;

                if (result != null)
                {
                    return result.Path;
                }
            }
            return string.Empty;

        }
    }
}
