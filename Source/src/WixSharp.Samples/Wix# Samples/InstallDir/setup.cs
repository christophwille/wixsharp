//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main()
    {
        // Wix# recognizes the top level dir as the installation directory and automatically
        // assigns it the INSTALLDIR id.
        // If for whatever reason it is undesirable you can always designate the installation directory
        // by setting the Dir.IsInstallDir to true or use a dedicated 'InstallDir' class (instead of `Dir`)
        // that does it for you automatically.
        //
        // Please for any further information refer to
        // the https://github.com/oleg-shilo/wixsharp/wiki/Deployment-scenarios/_edit#installation-directory

        var project =
                new ManagedProject("My Product",
                    new Dir(@"%ProgramFiles%\My Product",
                        new WixSharp.File(@"myapp.exe",
                            new FileShortcut("My App", "INSTALLDIR") { Advertise = true })));

        project.UI = WUI.WixUI_InstallDir;
        project.LocalizeErrorAndProgressText = false;

        // project.PreserveTempFiles = true;

        project.BeforeInstall += args =>
        {
            AppSearch.GetRelatedProducts(project.UpgradeCode.ToString());

            if (args.IsUninstalling)
                MessageBox.Show(args.InstallDir, "Uninstalling...");
            else
                MessageBox.Show(args.InstallDir, "Installing...");
        };

        project.BuildMsi("setup.msi");
    }
}