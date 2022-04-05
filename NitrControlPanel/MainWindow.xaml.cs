using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NitrControlPanel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                Inputs inputs = Config.Read();
                SetInputs(inputs);
            } catch
            {
                Config.Default();
                Inputs inputs = Config.Read();
                SetInputs(inputs);
            }

            if (Utils.IsAdmin())
                ServiceCheckBox.IsEnabled = true;
            else
                ServiceCheckBox.IsEnabled = false;

            if (Service.Exists())
            {
                ServiceCheckBox.IsChecked = true;
            }

            if (Service.IsRunning())
            {
                PID.Content = Service.GetProcessID();
                DisableInputs();
                RunningUI();

            }
            else
            {
                StartServerContextMenu.IsEnabled = true;
                StopServerContextMenu.IsEnabled = false;
                myNotifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/red.ico"));
                TrayTextBlock.Text = "NITR Control Panel: Stopped";
            }
        }



        private void SetInputs(Inputs inputs)
        {
            Port.Text = inputs.port;
            LogsCheckBox.IsChecked = inputs.saveLogs;
            WebPanelCheckBox.IsChecked = inputs.openBrowser;
        }

        private void DragMainWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            } catch
            {}
        }

        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            PackIcon packIcon = new PackIcon();
            string servicePIDString = ""; 


            if (Service.IsRunning())
            {
                ButtonProgressAssist.SetIsIndeterminate(StartBtn, true);
                ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, true);

                if (Service.Exists()) 
                    Service.StopService();
                else
                    Service.Stop();

                await Task.Delay(1000);
                ButtonProgressAssist.SetIsIndeterminate(StartBtn, false);
                ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, false);
                EnableInputs();
                StoppedUI();
            }
            else {
                if (Port.Text == "")
                    Port.Text = "8000";

                if (!Utils.IsPortOpen(int.Parse(Port.Text)))
                {     

                    if (Service.Exists())
                    {
                        PID.Content = Service.StartService();
                    }
                        
                    else
                    {
                        servicePIDString = Service.Start();
                        PID.Content = servicePIDString;
                        if (servicePIDString == "")
                        {
                            ButtonProgressAssist.SetIsIndeterminate(StartBtn, true);
                            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, false);
                        }
                        else
                        {
                            ButtonProgressAssist.SetIsIndeterminate(StartBtn, true);
                            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, true);
                            DisableInputs();
                            await Task.Delay(1000);

                            ButtonProgressAssist.SetIsIndeterminate(StartBtn, false);
                            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, false);
                            RunningUI();
                            StartNotification();
                        }
                    }
               
                }
                else
                {
                    StatusLabel.Content = "Port already in use";
                    StatusLabel.Foreground = (Brush)bc.ConvertFrom("#F1C40F");
                    StatusIcon.Foreground = (Brush)bc.ConvertFrom("#F1C40F");
                    Port.Focus();
                }
            }
        }

        private void HideControlPanel(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ShowControlPanel(object sender, RoutedEventArgs e)
        {
            Show();
        }

        public async void StartNotification()
        {
            string DisplayText = $"Server started on port {Port.Text}";
            myNotifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/app.ico"));
            myNotifyIcon.ToolTipText = DisplayText;
            myNotifyIcon.ShowBalloonTip("NITR Control Panel", DisplayText, BalloonIcon.None);
            TrayTextBlock.Text = "NITR Control Panel: Running";
            await Task.Delay(7000);
            myNotifyIcon.HideBalloonTip();
        }

        private void ExitTaskBarBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                var desktopWorkingArea = SystemParameters.WorkArea;
                Left = desktopWorkingArea.Right - Width - 64;
                Top = desktopWorkingArea.Bottom - Height;
        }

        private void SaveConfig(object sender, RoutedEventArgs e)
        {
            Inputs newInputs;
            newInputs.port = Port.Text;
            newInputs.saveLogs = (bool)LogsCheckBox.IsChecked;
            newInputs.openBrowser = (bool)WebPanelCheckBox.IsChecked;
            Config.Write(newInputs);
        }

        private void Port_LostFocus(object sender, RoutedEventArgs e)
        {
            Int32.TryParse(Port.Text, out int port);
            if (Port.Text == "")
                 Port.Text = "8000";
            else if (port > 49151)
                Port.Text = "49151";

            else if (port < 1024)
                Port.Text = "1024";

            Inputs newInputs;
            newInputs.port = Port.Text;
            newInputs.saveLogs = (bool)LogsCheckBox.IsChecked;
            newInputs.openBrowser = (bool)WebPanelCheckBox.IsChecked;
            Config.Write(newInputs);
        }

        private void Port_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void StartServerContextMenu_Click(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            PackIcon packIcon = new PackIcon();
            if (Port.Text == "")
            {
                Port.Text = "8000";
            }

            if (!Utils.IsPortOpen(int.Parse(Port.Text)))
            {
                ButtonProgressAssist.SetIsIndeterminate(StartBtn, true);
                ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, true);
                if (Service.Exists())
                    PID.Content = Service.StartService();
                else
                    PID.Content = Service.Start();
                DisableInputs();
                await Task.Delay(1000);
                ButtonProgressAssist.SetIsIndeterminate(StartBtn, false);
                ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, false);
                RunningUI();
                StartNotification();

            }
            else
            {
                Application.Current.MainWindow.Show();
                StatusLabel.Content = "Port already in use";
                StatusLabel.Foreground = (Brush)bc.ConvertFrom("#F1C40F");
                StatusIcon.Foreground = (Brush)bc.ConvertFrom("#F1C40F");
                Port.Focus();
            }
        }

        private async void StopServerContextMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndeterminate(StartBtn, true);
            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, true);
            if (Service.Exists())
                Service.StopService();
            else
                Service.Stop();
            EnableInputs();
            await Task.Delay(1000);
            ButtonProgressAssist.SetIsIndeterminate(StartBtn, false);
            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, false);  
            StoppedUI();
        }

        private void WebPanelShow_Click(object sender, RoutedEventArgs e)
        {
            Utils.ShowPanel(Port.Text);
        }

        private void EnableInputs()
        {
            Port.IsEnabled = true;
            if (Utils.IsAdmin())
                ServiceCheckBox.IsEnabled = true;
            else
                ServiceCheckBox.IsEnabled = false;

            WebPanelCheckBox.IsEnabled = true;
            LogsCheckBox.IsEnabled = true;
            StartServerContextMenu.IsEnabled = true;
            StopServerContextMenu.IsEnabled = false;
        }

        private void DisableInputs()
        {
            Port.IsEnabled = false;
            ServiceCheckBox.IsEnabled = false;
            WebPanelCheckBox.IsEnabled = false;
            LogsCheckBox.IsEnabled = false;
            StartServerContextMenu.IsEnabled = false;
            StopServerContextMenu.IsEnabled = true;
        }

        private void StoppedUI()
        {
            Logo.Source = new BitmapImage(new Uri("pack://application:,,,/nitr-mini-logo-grey.png"));

            PID.Content = "- - - - -";
            
            var bc = new BrushConverter();

            //Start Button
            var green = (Brush)bc.ConvertFrom("#2ECC71");
            StartBtn.Background = green;
            StartBtn.BorderBrush = green;
            PackIcon packIcon = new PackIcon
            {
                Kind = PackIconKind.Play,
                Width = 22,
                Height = 22
            };
            StartBtn.Content = packIcon;

            //Status Bar
            StatusLabel.Content = "Stopped";
            var red = (Brush)bc.ConvertFrom("#E74C3C");
            StatusLabel.Foreground = red;
            StatusIcon.Foreground = red;

            //Notification
            myNotifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/red.ico"));
            TrayTextBlock.Text = "NITR Control Panel: Stopped";

        }

        private void RunningUI()
        {
            Logo.Source = new BitmapImage(new Uri("pack://application:,,,/nitr-mini-logo.png"));
            
            var bc = new BrushConverter();

            //Start Button
            var red = (Brush)bc.ConvertFrom("#E74C3C");
            StartBtn.Background = red;
            StartBtn.BorderBrush = red;
            PackIcon packIcon = new PackIcon
            {
                Kind = PackIconKind.Stop,
                Width = 22,
                Height = 22
            };
            StartBtn.Content = packIcon;

            //Status Bar
            StatusLabel.Content = "Running";
            var green = (Brush)bc.ConvertFrom("#2ECC71");
            StatusLabel.Foreground = green;
            StatusIcon.Foreground = green;

            //Notification
            myNotifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/green.ico"));
            TrayTextBlock.Text = "NITR Control Panel: Running";
        }

        private void ShowLogs(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("notepad.exe", "nitr.log");
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ServiceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string rootPath = Directory.GetCurrentDirectory();
            string ServiceBinPath = rootPath + @"\nitr.exe";
            
            if (File.Exists(ServiceBinPath)) {
                Service.Install();
            } 
            else
            {
                ServiceCheckBox.IsChecked = false;
                MessageBox.Show("File nitr.exe does not exist.");
            }
            
        }

        private void ServiceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Service.Uninstall();
        }
    }
}
