using System;
using Gtk;
using System.ComponentModel;
public partial class MainWindow : Gtk.Window
{
    #region Globals
    string path = string.Empty;
    string results = string.Empty;
    NATS.ArgumentsObject.ArgumentsObject arguments = null;
    public BackgroundWorker worker = new BackgroundWorker() { WorkerSupportsCancellation = true };
    #endregion

    #region Form Load

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        this.Title = "Nats";
        path = PathPicker();
        this.Title = "Nats - " + path;
        worker.DoWork += Worker_DoWork;
        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;

    }

    #endregion

    #region Menu Events

    #region File

    protected void OpenPathEvent(object sender, EventArgs e)
    {
        path = PathPicker();
        this.Title = "Nats - " + path;
    }
 
    protected void SaveEvent(object sender, EventArgs e)
    {
        if (arguments != null)
        {
            Gtk.FileChooserDialog filechooser =
                     new Gtk.FileChooserDialog("Choose the file to Save",
                         this,
                         FileChooserAction.Save,
                         "Cancel", ResponseType.Cancel,
                         "Save", ResponseType.Ok);
            try
            {

                FileFilter filter = new FileFilter();
                filter.AddPattern("*.txt");
                filechooser.Filter = filter;
                filechooser.CurrentName = string.Concat((arguments == null) ? string.Empty : arguments.KeywordSearch,
                    " ", System.DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"), ".txt");
                if (filechooser.Run() == (int)ResponseType.Ok)
                {
                    try
                    {
                        string fileresponse = filechooser.Filename;
                        if (System.IO.File.Exists(fileresponse)) { msgbox("err", "File already exists, please use a different name"); }
                        else { System.IO.File.WriteAllText(fileresponse, results); msgbox("done", filechooser.Filename); }
                    }
                    catch (Exception x) { msgbox("error", x.Message); }
                }
                else { Console.WriteLine("grr"); msgbox("did not save", "did not save, no filename selected"); }
            }

            finally { filechooser.Destroy(); }

        }
        else { msgbox("err", "you need to perform a search first"); }
    }


    protected void ExitEvent(object sender, EventArgs e)
    {
        Environment.Exit(0);
    }
    #endregion

    #region Help
    protected void InfoActivated(object sender, EventArgs e)
    {
        msgbox("About Nats", String.Concat("Nats Text Editor",
            Environment.NewLine, "Copyright 2020 Luke Liukonen",
            Environment.NewLine, "MIT License",
            Environment.NewLine, "GTK GUI License GNU Lesser General Public License"));
    }

    #endregion
    #endregion

    #region Page Events

    protected void searchClick(object sender, EventArgs e)
    {
        if (worker.IsBusy)
        {
            worker.CancelAsync();
            button1.Label = "Search";
            entry1.Visible = true;
        }
        else
        {
            entry1.Visible = false;
            bool useSmartSearch = SmartSearchAction.Active;//false;
            NATS.Filters.FileExtentionFilter.filterType filter = NATS.Filters.FileExtentionFilter.filterType.DenyList;
            string Denylist = string.Join("|", nats_standard.Nats.DefaultDenyList());

            arguments = new NATS.ArgumentsObject.ArgumentsObject(filter, Denylist, useSmartSearch)
            {
                DirectoryPath = this.path,
                KeywordSearch = entry1.Text,
                MemoryLoad = RamAction.Active,
                MultiLine = MultiLineAction.Active,
                ThreadCount = 4,
                SearchType = GetSearchType()
            };


            if (!string.IsNullOrWhiteSpace(path))
            {
                textview1.Buffer.Text = string.Empty;
                worker.RunWorkerAsync();
                button1.Label = "Cancel";
            }
            else
            {
                msgbox("alert", "your path is not defined, please select a path.");
            }
        }
    }

    private NATS.ArgumentsObject.ArgumentsObject.eSearchType GetSearchType()
    {

        if (MultiThreadAction.Active) return NATS.ArgumentsObject.ArgumentsObject.eSearchType.Threaded;
        if (IndexWLoadAction.Active) return NATS.ArgumentsObject.ArgumentsObject.eSearchType.indexgenerateandsearch;
        if (IndexOnlyAction.Active) return NATS.ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex;
        return NATS.ArgumentsObject.ArgumentsObject.eSearchType.Single;
    }


    #region Background worker

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
    textview1.Buffer.Text = results;
        entry1.Visible = true;
        button1.Label = "Search";

        if (e.Error == null)
        {
            this.Title = "Nats- Done -" + path;
        }
        else
        {
            textview1.Buffer.Text = e.Error.Message;
        }

    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {

        nats_standard.Nats nats = new nats_standard.Nats();
        results = nats.OldSearch(arguments);

    }
    #endregion

    #endregion

    #region Helper
    void msgbox(string title, string message)
    {
        Dialog dialog = null;
        try
        {
            dialog = new Dialog(title, this,
            DialogFlags.DestroyWithParent | DialogFlags.Modal,
            ResponseType.Ok);
            dialog.AddButton("ok", ResponseType.Ok);
            dialog.VBox.Add(new Label(message));
            dialog.ShowAll();
            dialog.Run();
        }
        finally
        {
            if (dialog != null)
                dialog.Destroy();
        }
    }

    private string PathPicker()
    {
        Gtk.FileChooserDialog filechooser =
                 new Gtk.FileChooserDialog("Choose the file to open",
                     this,
                     FileChooserAction.SelectFolder,
                     "Cancel", ResponseType.Cancel,
                     "Open", ResponseType.Accept);
        try
        {
            filechooser.Run();
            return filechooser.CurrentFolder;
        }
        catch { return string.Empty; }
        finally { filechooser.Destroy(); }

    }

    #endregion
}
