using AppKit;
using Foundation;

namespace Nats
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        #region Properties
        public ViewController NatsGui { get; set; } = null;
        #endregion

        #region Base items
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            NatsGui.Path = DirPicker();
            //Disable items not working on first release
            mnIndexOnly.Hidden = true;
            mnIndexWLoad.Hidden = true;
            mnSmart_click.Hidden = true;
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
        #endregion

        #region Menu Items

        #region File

        [Export("openDocument:")]
        void OpenDialog(NSObject sender)
        {
            NatsGui.Path = DirPicker();

        }

        partial void Save_click(NSObject sender)
        {
            var dialog = new NSSavePanel()
            {
                Title = "Save results",
                AllowedFileTypes = new string[] { "txt" },
                AllowsOtherFileTypes = false,
                ShowsTagField = false,
                NameFieldStringValue = string.Concat(NatsGui.keywords, " ", System.DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"), ".txt")
            };
            if (dialog.RunModal() == 1)
            {
                if (dialog.Url != null && !System.IO.File.Exists(dialog.Url.Path))
                {
                    System.IO.File.WriteAllText(dialog.Url.Path, NatsGui.results);
                }
                else { }
            }

        }
        #endregion

        #region Engines

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

        /// <summary>
        /// Index disabled, DB issues
        /// </summary>
        /// <param name="sender"></param>
        partial void mnIndex_click(NSObject sender)
        {
            mnSingleThread.State = NSCellStateValue.Off;
            mnMultiThread.State = NSCellStateValue.Off;
            mnIndexOnly.State = NSCellStateValue.On;
            mnIndexWLoad.State = NSCellStateValue.Off;
            NatsGui.SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex;

        }

        /// <summary>
        /// Index disabled DB isues
        /// </summary>
        /// <param name="sender"></param>
        partial void mnIndexLoad_Click(NSObject sender)
        {
            mnSingleThread.State = NSCellStateValue.Off;
            mnMultiThread.State = NSCellStateValue.Off;
            mnIndexOnly.State = NSCellStateValue.Off;
            mnIndexWLoad.State = NSCellStateValue.On;
            NatsGui.SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.indexgenerateandsearch;

        }
        #endregion

        #region Options

        /// <summary>
        /// Disabled due to SS not working on mac
        /// </summary>
        /// <param name="sender"></param>
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
        #endregion

        #region help
        partial void help_click(NSObject sender)
        {
            (new NSAlert() { MessageText = "Help", InformativeText = nats_standard.Nats.GUIHelpFile(), AlertStyle = NSAlertStyle.Informational }).RunModal();
        }
        #endregion

        #endregion

        #region Helper
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
        #endregion
    }
}
