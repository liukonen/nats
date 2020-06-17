using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using System.Collections.Generic;
using System.ComponentModel;
namespace Nats
{
    public partial class ViewController : NSViewController
    {

        #region Globals
        public Boolean UseRam = true;
        public Boolean UseSmartSearch = false;
        public Boolean UseMultiLine = false;
        public NATS.ArgumentsObject.ArgumentsObject.eSearchType SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Threaded;
        public List<string> DenylistItems = new List<string>(nats_standard.Nats.DefaultDenyList());
        public string Path = string.Empty;
        public System.ComponentModel.BackgroundWorker worker = new BackgroundWorker() { WorkerSupportsCancellation = true };
        public string results = string.Empty;
        public string keywords = string.Empty;
        #endregion

        #region Application Access
        public static AppDelegate App { get { return (AppDelegate)NSApplication.SharedApplication.Delegate; } }
        #endregion

        #region Computed Properties
        public override NSObject RepresentedObject { get { return base.RepresentedObject; } set { base.RepresentedObject = value; } }
        #endregion

        #region Constructors
        public ViewController(IntPtr handle) : base(handle)
        {
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }
        #endregion

        #region Background Worker
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ResultsTxt.Value = results;
            txtSearch.Enabled = true;
            btnSearch.Title = "Search";

            if (e.Error == null)
            { (new NSAlert() { AlertStyle = NSAlertStyle.Informational, MessageText = "Done", InformativeText = "search complete" }).RunModal(); }
            else
            { (new NSAlert() { AlertStyle = NSAlertStyle.Critical, MessageText = "Done", InformativeText = e.Error.Message}).RunModal();
                
                System.IO.File.WriteAllText("err.log", e.Error.ToString());
                string s = System.IO.Path.GetFullPath("err.log");
                Console.WriteLine(s);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var FilterType = NATS.Filters.FileExtentionFilter.filterType.DenyList;
            string Filter = string.Join("|", DenylistItems);
            var Args = new NATS.ArgumentsObject.ArgumentsObject(FilterType, Filter, UseSmartSearch)
            {
                DirectoryPath = Path,
                KeywordSearch = keywords,
                MemoryLoad = UseRam,
                MultiLine = UseMultiLine,
                SearchType = SearchType,
                ThreadCount = 4
            };
            nats_standard.Nats nats = new nats_standard.Nats();
            results = nats.OldSearch(Args);

        }
        #endregion

        #region Override Methods
        public override void ViewDidLoad() { base.ViewDidLoad(); }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            App.NatsGui = this;
        }

        public override void ViewWillDisappear()
        {
            base.ViewDidDisappear();
            App.NatsGui = null;
    }

        partial void Search_click(NSObject sender)
        {

            if (worker.IsBusy) { btnSearch.Title = "Search"; worker.CancelAsync(); }
            else
            {
                keywords = txtSearch.StringValue;
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    worker.RunWorkerAsync();
                    btnSearch.Title = "Cancel";
                }
                else { (new NSAlert() { AlertStyle = NSAlertStyle.Informational, MessageText = "alert", InformativeText = "your path is not defined, please select a path." }).RunModal(); }
            }
        }
        #endregion
    }
}
