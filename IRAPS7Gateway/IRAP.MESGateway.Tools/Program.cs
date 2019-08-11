using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Tutorials;
using DevExpress.UserSkins;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRAP.MESGateway.Tools
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] arguments)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            ParamHelper.ApplicationArguments = arguments;
            LocalizationHelper.SetCurrentCulture(ParamHelper.ApplicationArguments);
            BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("McSkin");
            AppearanceObject.DefaultFont = new Font("新宋体", 12);
            SkinManager.EnableFormSkins();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static Assembly OnCurrentDomainAssemblyResolve(
            object sender,
            ResolveEventArgs args)
        {
            string partialName = AssemblyHelper.GetPartialName(args.Name).ToLower();
            if (partialName == "entityframework" ||
                partialName == "system.data.sqlite" ||
                partialName == "system.data.sqlite.ef6")
            {
                string path =
                    FilePathUtils.FindFilePath($"Dll\\{partialName}.dll", false);
                return Assembly.LoadFrom(path);
            }
            else
            {
                return null;
            }
        }
    }
}
