using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NitrControlPanel
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();

            if (Utils.InternetOK())
            {
                
            string latestVersion = Utils.GetLastVersionNumber();
            string downloadUrl = "https://github.com/bitcav/nitr/releases/download/" + latestVersion + "/nitr_windows_amd64.exe";
            var rootDir = Directory.GetCurrentDirectory();
            var nitrBin = rootDir + "/nitr.exe";

                if (File.Exists(nitrBin))
                {
                    FileVersionInfo localVersion = FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "nitr.exe"));
                    string localFileVersion = localVersion.FileVersion;


                    if (latestVersion == localFileVersion)
                    {
                        Task.Run(function: async () => {
                            await Task.Run(() => {
                                Thread.Sleep(1600);
                                SplashScreenWindow.Dispatcher.Invoke(() => SplashScreenWindow.Hide(), DispatcherPriority.Background);
                            });

                            await Task.Run(() => {
                                Application.Current.Dispatcher.Invoke((Action)delegate {
                                    var mainWindow = new MainWindow();
                                    mainWindow.Show();
                                });
                            });
                        });

                    }
                    else
                    {
                        _ = Utils.DownloadAsync(downloadUrl, nitrBin, DownloadProgress, DownloadLabel, StatusLabel, PercentLabel, SplashScreenWindow);

                    }
                } else
                {
                    _ = Utils.DownloadAsync(downloadUrl, nitrBin, DownloadProgress, DownloadLabel, StatusLabel, PercentLabel, SplashScreenWindow);

                }


            } else
            {
                Task.Run(function: async () => {
                    await Task.Run(() => {
                        Thread.Sleep(1600);
                        SplashScreenWindow.Dispatcher.Invoke(() => SplashScreenWindow.Hide(), DispatcherPriority.Background);
                    });

                    await Task.Run(() => {
                        Application.Current.Dispatcher.Invoke((Action)delegate {
                            var mainWindow = new MainWindow();
                            mainWindow.Show();
                        });
                    });
                });
            }

        }

        private void DragSplash(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            { }
        }

    }
}
