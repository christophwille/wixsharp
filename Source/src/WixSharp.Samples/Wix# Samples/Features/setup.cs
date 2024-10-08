//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;

// using System.IO;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        // Note you can detect at runtime if the feature has been marked for installation by using condition
        // like this Condition.Create("ADDLOCAL >< \"your_feature_name\"")

        var binaries = new Feature("MyApp Binaries", "Application binaries", "FEATURE_INSTALL_PATH2");
        var docs = new Feature("MyApp Documentation");
        var docsLight = new Feature("MyApp Light Documentation");
        var tuts = new Feature("MyApp Tutorial");
        var test = new Feature("TEST");

        docs.Add(tuts);
        binaries.Add(docs);
        binaries.Add(docsLight);

        var project =
            new ManagedProject("MyProduct",
                new Dir(test, @"%ProgramFiles%\My Company\My Product",
                    new File(binaries, @"Files\Bin\MyApp.exe"),
                    new Dir(new Id("FEATURE_INSTALL_PATH2"), @"Docs\Manual",
                        new File(docs, @"Files\Docs\Manual.txt"),
                        new File(@"Files\Docs\Tutorial.txt")
                        {
                            Features = new[] { docsLight, tuts }
                        }),
                    new Dir(docs, "logdocs", new DirPermission("Everyone", GenericPermission.All)),
                    new Dir(tuts, "logtuts", new DirPermission("Everyone", GenericPermission.All))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_FeatureTree;

        project.DefaultFeature = binaries; //this line is optional

        project.DefaultDeferredProperties += ",FEATURE_INSTALL_PATH2";
        project.AfterInstall += (SetupEventArgs e) =>
        {
            try
            {
                MessageBox.Show(e.Session.Property("FEATURE_INSTALL_PATH2"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        };

        project.AddCustomActionRefAssembliesOf(typeof(Script));

        // project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}