using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRAP.MESGateway.Tools
{
    public class ModulesNavigator
    {
        private RibbonControl ribbon;
        private PanelControl panel;

        public ModulesNavigator(RibbonControl ribbon, PanelControl panel)
        {
            this.ribbon = ribbon;
            this.panel = panel;
        }

        public BaseModule CurrentModule
        {
            get
            {
                if (panel.Controls.Count == 0)
                {
                    return null;
                }
                return panel.Controls[0] as BaseModule;
            }
        }

        public void ChangeGroup(NavBarGroup group, object moduleData)
        {
            bool allowSetVisiblePage = true;
            NavBarGroupTagObject groupObject = group.Tag as NavBarGroupTagObject;
            if (groupObject == null)
            {
                return;
            }

            List<RibbonPage> deferredPageToShow = new List<RibbonPage>();
            foreach (RibbonPage page in ribbon.Pages)
            {
                if (!string.IsNullOrEmpty($"{page.Tag}"))
                {
                    bool isPageVisible = groupObject.Name.Equals(page.Tag);
                    if (isPageVisible != page.Visible && isPageVisible)
                    {
                        deferredPageToShow.Add(page);
                    }
                    else
                    {
                        page.Visible = isPageVisible;
                    }
                }

                if (page.Visible && allowSetVisiblePage)
                {
                    ribbon.SelectedPage = page;
                    allowSetVisiblePage = false;
                }
            }

            bool firstShow = groupObject.Module == null;
            if (firstShow)
            {
                if (SplashScreenManager.Default == null)
                {
                    SplashScreenManager.ShowForm(
                        ribbon.FindForm(),
                        typeof(Forms.wfMain),
                        false,
                        true);
                }

                ConstructorInfo constructorInfoObj =
                    groupObject.ModuleType.GetConstructor(Type.EmptyTypes);
                if (constructorInfoObj != null)
                {
                    groupObject.Module = constructorInfoObj.Invoke(null) as BaseModule;
                    groupObject.Module.InitModule(ribbon, moduleData);
                }

                if (SplashScreenManager.Default != null)
                {
                    Form frm = moduleData as Form;
                    if (frm != null)
                    {
                        if (SplashScreenManager.FormInPendingState)
                        {
                            SplashScreenManager.CloseForm();
                        }
                        else
                        {
                            SplashScreenManager.CloseForm(false, 500, frm);
                        }
                    }
                    else
                    {
                        SplashScreenManager.CloseForm();
                    }
                }
            }

            foreach (RibbonPage page in deferredPageToShow)
            {
                page.Visible = true;
            }

            foreach (RibbonPage page in ribbon.Pages)
            {
                if (page.Visible)
                {
                    ribbon.SelectedPage = page;
                    break;
                }
            }

            if (groupObject.Module != null)
            {
                if (panel.Controls.Count > 0)
                {
                    if (panel.Controls[0] is BaseModule currentModule)
                    {
                        currentModule.HideModule();
                    }
                }
                panel.Controls.Clear();
                panel.Controls.Add(groupObject.Module);
                groupObject.Module.Dock = DockStyle.Fill;
                //groupObject.Module.ShowModule(firstShow);
            }
        }
    }

    public class BaseControl : XtraUserControl
    {
        public BaseControl()
        {
            if (!DesignTimeTools.IsDesignMode)
            {
                LookAndFeel.ActiveLookAndFeel.StyleChanged +=
                    new EventHandler(ActiveLookAndFeel_StyleChanged);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignTimeTools.IsDesignMode)
            {
                LookAndFeelStyleChanged();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !DesignTimeTools.IsDesignMode)
            {
                LookAndFeel.ActiveLookAndFeel.StyleChanged -=
                    new EventHandler(ActiveLookAndFeel_StyleChanged);
            }

            base.Dispose(disposing);
        }

        void ActiveLookAndFeel_StyleChanged(object sender, EventArgs e)
        {
            LookAndFeelStyleChanged();
        }

        protected virtual void LookAndFeelStyleChanged() { }
    }

    public class NavBarGroupTagObject
    {
        public NavBarGroupTagObject(string name, Type moduleType)
        {
            Name = name;
            ModuleType = moduleType;
            Module = null;
        }

        public string Name { get; }
        public Type ModuleType { get; }
        public BaseModule Module { get; set; }
    }

    public class BaseModule : BaseControl
    {
        protected string partName = string.Empty;

        public virtual string ModuleName
        {
            get { return GetType().Name; }
        }

        public string PartName
        {
            get { return partName; }
        }

        internal MainForm OwnerForm
        {
            get { return FindForm() as MainForm; }
        }

        private void SetMenuManager(ControlCollection controlCollection, IDXMenuManager manager)
        {
            foreach (Control ctrl in controlCollection)
            {
                if (ctrl is GridControl grid)
                {
                    grid.MenuManager = manager;
                    break;
                }

                if (ctrl is BaseEdit edit)
                {
                    edit.MenuManager = manager;
                    break;
                }

                if (ctrl is TreeList tlist)
                {
                    tlist.MenuManager = manager;
                    break;
                }

                SetMenuManager(ctrl.Controls, manager);
            }
        }

        internal virtual void InitModule(IDXMenuManager manager, object data)
        {
            SetMenuManager(Controls, manager);
        }

        internal virtual void HideModule() { }

        internal virtual void ShowModule(bool firstShow)
        {
            MainForm owner = OwnerForm;
            if (owner == null)
            {
                return;
            }

            //owner.SaveAsMenuItem.Enabled = SaveAsEnable;
            //owner.SaveAttachmentMenuItem.Enabled = SaveAttachmentEnable;
            //owner.SaveCalendar.Visible = SaveCalendarVisible;
            //owner.EnableLayoutButtons(true);

            //ShowReminder();
            //ShowInfo();
        }

        internal virtual void FocusObject(object obj) { }

        protected internal virtual void ButtonClick(MenuItem menuItemName) { }
        protected internal virtual void BarListItemClick(MenuItem menuItemName, int index) { }
        protected internal virtual void ShowDataChanged(DeviceTreeDataSourceChangedEventArgs args) { }
        protected internal virtual void ShowDataChanged(ServiceTreeDataSourceChangedEventArgs args) { }
    }
}
