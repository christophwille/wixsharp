//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

public class InstallScript
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",

                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new WixSharp.File(@"readme.txt")),

                new Binary("Fake CRT.msi"),
                new ManagedAction(InstallScript.InstallCRTAction,
                                  Return.check,
                                  When.Before,
                                  Step.LaunchConditions,
                                  Condition.NOT_Installed,
                                  Sequence.InstallUISequence));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");

        Compiler.BuildMsi(project);
    }

    [CustomAction]
    public static ActionResult InstallCRTAction(Session session)
    {
        //This can be successfully executed only from UISequence
        if (!IsCRTInstalled())
        {
            //extract CRT msi into temp directory
            string CRTMsiFile = Path.ChangeExtension(Path.GetTempFileName(), ".msi");
            string CRTMsiId = "Fake CRT.msi".Expand();//Expand() is needed to normalize file name into file ID

            session.SaveBinary(CRTMsiId, CRTMsiFile);

            //install CTR
            Process.Start(CRTMsiFile).WaitForExit();

            if (!IsCRTInstalled()) //there is no warranty that CRT installation succeeded
            {
                var result = MessageBox.Show("CRT is not installed.\n\nDo you want to continue without CRT?", "Prerequisites is not found", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return ActionResult.UserExit;
            }
        }

        return ActionResult.Success;
    }

    static bool IsCRTInstalled()
    {
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{6F330B47-2577-43AD-1195-1861BA25889B}"))
            return key != null;
    }
}