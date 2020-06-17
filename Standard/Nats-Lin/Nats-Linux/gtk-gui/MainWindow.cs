
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;

	private global::Gtk.Action FileAction;

	private global::Gtk.Action openAction;

	private global::Gtk.Action saveAction;

	private global::Gtk.Action stopAction;

	private global::Gtk.Action EnginesAction;

	private global::Gtk.RadioAction MultiThreadAction;

	private global::Gtk.RadioAction SingleThreadAction;

	private global::Gtk.Action OptionsAction;

	private global::Gtk.ToggleAction RamAction;

	private global::Gtk.ToggleAction MultiLineAction;

	private global::Gtk.Action HelpAction;

	private global::Gtk.Action dialogInfoAction;

	private global::Gtk.ToggleAction SmartSearchAction;

	private global::Gtk.RadioAction IndexWLoadAction;

	private global::Gtk.RadioAction IndexOnlyAction;

	private global::Gtk.Table table1;

	private global::Gtk.ScrolledWindow GtkScrolledWindow;

	private global::Gtk.TextView textview1;

	private global::Gtk.MenuBar menubar1;

	private global::Gtk.Table table2;

	private global::Gtk.Button button1;

	private global::Gtk.Entry entry1;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup("Default");
		this.FileAction = new global::Gtk.Action("FileAction", global::Mono.Unix.Catalog.GetString("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString("File");
		w1.Add(this.FileAction, null);
		this.openAction = new global::Gtk.Action("openAction", global::Mono.Unix.Catalog.GetString("Open Path"), null, "gtk-open");
		this.openAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Open Path");
		w1.Add(this.openAction, null);
		this.saveAction = new global::Gtk.Action("saveAction", global::Mono.Unix.Catalog.GetString("Save"), null, "gtk-save");
		this.saveAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Save");
		w1.Add(this.saveAction, null);
		this.stopAction = new global::Gtk.Action("stopAction", global::Mono.Unix.Catalog.GetString("Exit"), null, "gtk-stop");
		this.stopAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Exit");
		w1.Add(this.stopAction, null);
		this.EnginesAction = new global::Gtk.Action("EnginesAction", global::Mono.Unix.Catalog.GetString("Engines"), null, null);
		this.EnginesAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Engines");
		w1.Add(this.EnginesAction, null);
		this.MultiThreadAction = new global::Gtk.RadioAction("MultiThreadAction", global::Mono.Unix.Catalog.GetString("Multi Thread"), null, null, 0);
		this.MultiThreadAction.Group = new global::GLib.SList(global::System.IntPtr.Zero);
		this.MultiThreadAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Multi Thread");
		w1.Add(this.MultiThreadAction, null);
		this.SingleThreadAction = new global::Gtk.RadioAction("SingleThreadAction", global::Mono.Unix.Catalog.GetString("Single Thread"), null, null, 0);
		this.SingleThreadAction.Group = this.MultiThreadAction.Group;
		this.SingleThreadAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Single Thread");
		w1.Add(this.SingleThreadAction, null);
		this.OptionsAction = new global::Gtk.Action("OptionsAction", global::Mono.Unix.Catalog.GetString("Options"), null, null);
		this.OptionsAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Options");
		w1.Add(this.OptionsAction, null);
		this.RamAction = new global::Gtk.ToggleAction("RamAction", global::Mono.Unix.Catalog.GetString("Ram"), null, null);
		this.RamAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Ram");
		w1.Add(this.RamAction, null);
		this.MultiLineAction = new global::Gtk.ToggleAction("MultiLineAction", global::Mono.Unix.Catalog.GetString("Multi Line"), null, null);
		this.MultiLineAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Multi Line");
		w1.Add(this.MultiLineAction, null);
		this.HelpAction = new global::Gtk.Action("HelpAction", global::Mono.Unix.Catalog.GetString("Help"), null, null);
		this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Help");
		w1.Add(this.HelpAction, null);
		this.dialogInfoAction = new global::Gtk.Action("dialogInfoAction", global::Mono.Unix.Catalog.GetString("Info"), null, "gtk-dialog-info");
		this.dialogInfoAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Info");
		w1.Add(this.dialogInfoAction, null);
		this.SmartSearchAction = new global::Gtk.ToggleAction("SmartSearchAction", global::Mono.Unix.Catalog.GetString("Smart Search"), null, null);
		this.SmartSearchAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Smart Search");
		w1.Add(this.SmartSearchAction, null);
		this.IndexWLoadAction = new global::Gtk.RadioAction("IndexWLoadAction", global::Mono.Unix.Catalog.GetString("Index w/ Load"), null, null, 0);
		this.IndexWLoadAction.Group = this.SingleThreadAction.Group;
		this.IndexWLoadAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Index w/ Load");
		w1.Add(this.IndexWLoadAction, null);
		this.IndexOnlyAction = new global::Gtk.RadioAction("IndexOnlyAction", global::Mono.Unix.Catalog.GetString("Index Only"), null, null, 0);
		this.IndexOnlyAction.Group = this.SingleThreadAction.Group;
		this.IndexOnlyAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Index Only");
		w1.Add(this.IndexOnlyAction, null);
		this.UIManager.InsertActionGroup(w1, 0);
		this.AddAccelGroup(this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.table1 = new global::Gtk.Table(((uint)(3)), ((uint)(1)), false);
		this.table1.Name = "table1";
		this.table1.RowSpacing = ((uint)(6));
		this.table1.ColumnSpacing = ((uint)(6));
		// Container child table1.Gtk.Table+TableChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.textview1 = new global::Gtk.TextView();
		this.textview1.CanFocus = true;
		this.textview1.Name = "textview1";
		this.GtkScrolledWindow.Add(this.textview1);
		this.table1.Add(this.GtkScrolledWindow);
		global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this.GtkScrolledWindow]));
		w3.TopAttach = ((uint)(2));
		w3.BottomAttach = ((uint)(3));
		// Container child table1.Gtk.Table+TableChild
		this.UIManager.AddUiFromString(@"<ui><menubar name='menubar1'><menu name='FileAction' action='FileAction'><menuitem name='openAction' action='openAction'/><menuitem name='saveAction' action='saveAction'/><menuitem name='stopAction' action='stopAction'/></menu><menu name='EnginesAction' action='EnginesAction'><menuitem name='MultiThreadAction' action='MultiThreadAction'/><menuitem name='SingleThreadAction' action='SingleThreadAction'/><menuitem name='IndexWLoadAction' action='IndexWLoadAction'/><menuitem name='IndexOnlyAction' action='IndexOnlyAction'/></menu><menu name='OptionsAction' action='OptionsAction'><menuitem name='RamAction' action='RamAction'/><menuitem name='MultiLineAction' action='MultiLineAction'/><menuitem name='SmartSearchAction' action='SmartSearchAction'/></menu><menu name='HelpAction' action='HelpAction'><menuitem name='dialogInfoAction' action='dialogInfoAction'/></menu></menubar></ui>");
		this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget("/menubar1")));
		this.menubar1.Name = "menubar1";
		this.table1.Add(this.menubar1);
		global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1[this.menubar1]));
		w4.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table1.Gtk.Table+TableChild
		this.table2 = new global::Gtk.Table(((uint)(1)), ((uint)(2)), false);
		this.table2.Name = "table2";
		this.table2.RowSpacing = ((uint)(6));
		this.table2.ColumnSpacing = ((uint)(6));
		// Container child table2.Gtk.Table+TableChild
		this.button1 = new global::Gtk.Button();
		this.button1.CanFocus = true;
		this.button1.Name = "button1";
		this.button1.UseUnderline = true;
		this.button1.Label = global::Mono.Unix.Catalog.GetString("Search");
		this.table2.Add(this.button1);
		global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table2[this.button1]));
		w5.LeftAttach = ((uint)(1));
		w5.RightAttach = ((uint)(2));
		w5.XOptions = ((global::Gtk.AttachOptions)(4));
		w5.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table2.Gtk.Table+TableChild
		this.entry1 = new global::Gtk.Entry();
		this.entry1.CanFocus = true;
		this.entry1.Name = "entry1";
		this.entry1.IsEditable = true;
		this.entry1.InvisibleChar = '•';
		this.table2.Add(this.entry1);
		global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table2[this.entry1]));
		w6.YOptions = ((global::Gtk.AttachOptions)(4));
		this.table1.Add(this.table2);
		global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1[this.table2]));
		w7.TopAttach = ((uint)(1));
		w7.BottomAttach = ((uint)(2));
		w7.YOptions = ((global::Gtk.AttachOptions)(4));
		this.Add(this.table1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 1109;
		this.DefaultHeight = 648;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.openAction.Activated += new global::System.EventHandler(this.OpenPathEvent);
		this.saveAction.Activated += new global::System.EventHandler(this.SaveEvent);
		this.stopAction.Activated += new global::System.EventHandler(this.ExitEvent);
		this.dialogInfoAction.Activated += new global::System.EventHandler(this.InfoActivated);
		this.button1.Clicked += new global::System.EventHandler(this.searchClick);
	}
}
