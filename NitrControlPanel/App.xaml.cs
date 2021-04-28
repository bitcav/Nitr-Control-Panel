using System;
using System.Diagnostics;
using System.Windows;

namespace NitrControlPanel
{


    public partial class App
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            if (SingleInstance.AlreadyRunning())
            {
                MessageBox.Show("Another instance is running.");
                Environment.Exit(0);            
            }

            base.OnStartup(e);
        }
    }
}