using AppKit;
using Foundation;
using NATS.ArgumentsObject;

namespace Nats
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        #region Properties
        public ViewController NatsGui { get; set; } = null;
        #endregion

        #region Base items
        public AppDelegate(){}

        public override void DidFinishLaunching(NSNotification notification)
        {
            NatsGui.Path = DirPicker();
        }

        public override void WillTerminate(NSNotification notification)
        {
            nats_standard.Index.LiteDbAbstraction.Instance().Dispose();
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
                Title = "Save results", AllowedFileTypes = new string[] { "txt" },AllowsOtherFileTypes = false,
                ShowsTagField = false,NameFieldStringValue = string.Concat(NatsGui.keywords, " ", System.DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"), ".txt")
            };
            if (dialog.RunModal() == 1)
            {
                if (dialog.Url != null && !System.IO.File.Exists(dialog.Url.Path)) System.IO.File.WriteAllText(dialog.Url.Path, NatsGui.results);
            }

        }
        #endregion

        #region Engines

        partial void mnSingleThread_click(NSObject sender)
        {
            handleSearchType(NATS.ArgumentsObject.ArgumentsObject.eSearchType.Single);
        }

        partial void mnMultiThread_click(NSObject sender)
        {
            handleSearchType(NATS.ArgumentsObject.ArgumentsObject.eSearchType.Threaded);
        }


        partial void mnIndex_click(NSObject sender)
        {
            handleSearchType(NATS.ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex);
        }

        partial void mnIndexLoad_Click(NSObject sender)
        {
            handleSearchType(ArgumentsObject.eSearchType.indexgenerateandsearch);
        }

        private void handleSearchType(ArgumentsObject.eSearchType searchType)
        {
            mnSingleThread.State = (searchType == ArgumentsObject.eSearchType.Single) ? NSCellStateValue.On : NSCellStateValue.Off;
            mnMultiThread.State = (searchType == ArgumentsObject.eSearchType.Threaded) ? NSCellStateValue.On : NSCellStateValue.Off;
            mnIndexOnly.State = (searchType == ArgumentsObject.eSearchType.LocalIndex) ? NSCellStateValue.On : NSCellStateValue.Off;
            mnIndexWLoad.State = (searchType == ArgumentsObject.eSearchType.indexgenerateandsearch) ? NSCellStateValue.On : NSCellStateValue.Off;
            NatsGui.SearchType = searchType;
        }

        #endregion

        #region Options

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
            var dialog = new NSOpenPanel() { Title = "Choose single directory | NATS", ShowsResizeIndicator = true, ShowsHiddenFiles = false, CanChooseDirectories = true, CanChooseFiles = false };

            if (dialog.RunModal() == 1 && dialog.Url != null) return dialog.Url.Path;
            return string.Empty;
        }
        #endregion
    }
}
