using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for OperatingSystemPage.xaml
    /// </summary>
    public partial class OperatingSystemPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        public OperatingSystemPage()
        {
            InitializeComponent();
        }

        public async void RetrieveOSInfo()
        {
            RegistryKey checkwindowsversion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            //var productname = Convert.ToString(checkwindowsversion.GetValue("ProductName"));
            ComputerInfo computerInfo = new ComputerInfo();
            string productname = computerInfo.OSFullName.Replace("Microsoft ", "");
            editionValue.Content = productname;

            RegistryKey checkwindowsminorversion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var minorversion = checkwindowsminorversion.GetValue("DisplayVersion");

            RegistryKey checkbuild = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var buildnumber = checkbuild.GetValue("CurrentBuildNumber");

            versionValue.Content = minorversion;

            // build number
            buildNumberValue.Content = buildnumber;


            // check branch
            var branchraw = checkwindowsversion.GetValue("BuildBranch");
            var branch = branchraw.ToString().Replace("_", "__");
            branchValue.Content = branch;

            // check insider
            RegistryKey checkinsider = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\Applicability");
            var insiderstatus = Convert.ToString(checkinsider.GetValue("BranchName"));
            if (insiderstatus != null && insiderstatus == "ReleasePreview")
            {
                insiderStatusValue.Content = "Yes";
                insiderChannelValue.Content = "Release Preview";
            }
            else if (insiderstatus != null && insiderstatus == "Beta")
            {
                insiderStatusValue.Content = "Yes";
                insiderChannelValue.Content = "Beta";
            }
            else if (insiderstatus != null && insiderstatus == "Dev")
            {
                insiderStatusValue.Content = "Yes";
                insiderChannelValue.Content = "Dev";
            }
            else
            {
                insiderStatusValue.Content = "No";
                insiderChannelValue.Content = "N/A";
                insiderStatusValue.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
                insiderChannelValue.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
                branchValue.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
            }

            if (Convert.ToInt32(buildnumber) > 21390)
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 11 logo.png"));

            }
            else if (productname.Contains("Windows 10"))
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 10 logo.png"));
            }
            else if (productname.Contains("Windows 8"))
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 8 logo.png"));
                versionValue.Content = "N/A";
                insiderStatusValue.Content = "N/A";
                branchValue.Content = "N/A";
            }
            else if (productname.Contains("Windows 7"))
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 7 logo.png"));
                versionValue.Content = "N/A";
                insiderStatusValue.Content = "N/A";
                branchValue.Content = "N/A";
            }
            else
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows unknown logo.png"));
                versionValue.Content = "Not Supported";
                branchValue.Content = "Not Supported";
                insiderStatusValue.Content = "Not Supported";
                insiderChannelValue.Content = "Not Supported";
                branchValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderStatusValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderChannelValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
            }
            await Task.Delay(200);
        }
    }
}
