using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NitrControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragMainWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            } catch
            {

            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndeterminate(StartBtn, true);
            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, true);
            await Task.Delay(1000);
            ButtonProgressAssist.SetIsIndeterminate(StartBtn, false);
            ButtonProgressAssist.SetIsIndicatorVisible(StartBtn, false);
            var bc = new BrushConverter();
            var redBackround = (Brush)bc.ConvertFrom("#E74C3C");
            StartBtn.Background = redBackround;
            StartBtn.BorderBrush = redBackround;
            PackIcon packIcon = new PackIcon();
            packIcon.Kind = PackIconKind.Stop;
            StartBtn.Content = packIcon;
            StatusLabel.Content = "Running";
            StatusLabel.Foreground = (Brush)bc.ConvertFrom("#2ECC71");
            StatusIcon.Foreground = (Brush)bc.ConvertFrom("#2ECC71");
        }
    }
}
