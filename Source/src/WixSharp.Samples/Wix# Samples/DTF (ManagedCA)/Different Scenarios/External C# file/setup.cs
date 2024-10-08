//css_ref ..\..\..\..\WixSharp.dll;
//css_include CustomAction.cs;
//css_ref System.Core.dll;
//css_ref ..\..\..\..\Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;

using System;
using System.Windows.Forms;
using WixToolset.Dtf.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project()
        {
            UI = WUI.WixUI_ProgressOnly,
            Name = "CustomActionTest",

            Actions = new[]
            {
                new ManagedAction("CustomAction1") { ActionAssembly= "%this%" }
            }
        };

        Compiler.BuildMsi(project);
    }
}