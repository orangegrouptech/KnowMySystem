using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Management;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using Microsoft.VisualBasic.Devices;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using iNKORE.UI.WPF.Modern.Media.Animation;
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using iNKORE.UI.WPF.Modern.Helpers.Styles;
using System.Windows.Shell;
using KnowMySystem.Pages.Hardware_Specifications_Pages;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoadingPage loadingPage = new LoadingPage();
        HardwareSpecificationsPage hardwareSpecificationsPage = new HardwareSpecificationsPage();
        OperatingSystemPage operatingSystemPage = new OperatingSystemPage();
        StartupItemsPage startupItemsPage = new StartupItemsPage();
        SettingsPage settingsPage = new SettingsPage();

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            RegistryKey checkdarklightmode = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Orange Group\KnowYourSystem");
            if (checkdarklightmode.GetValue("DarkMode") == null)
            {
                settingsPage.systemRadioButton.IsChecked = true;
            }
            else
            if ((int)checkdarklightmode.GetValue("DarkMode") == 1)
            {
                settingsPage.darkRadioButton.IsChecked = true;
            }
            else if ((int)checkdarklightmode.GetValue("DarkMode") == 0)
            {
                settingsPage.lightRadioButton.IsChecked = true;
            }
            else
            {
                settingsPage.systemRadioButton.IsChecked = true;
            }
            loadingPage.Show();
            Main2();
        }

        private async void Main2()
        {
            // CPU
            loadingPage.loadingLabel.Content = "Loading: CPU Info";
            loadingPage.progressBar.Value = 0;
            hardwareSpecificationsPage.RetrieveCPUInfo();
            await Delay(200);

            // GPU
            loadingPage.loadingLabel.Content = "Loading: GPU Info";
            loadingPage.progressBar.Value = 9;
            hardwareSpecificationsPage.RetrieveGPUInfo();
            await Delay(200);

            // RAM
            loadingPage.loadingLabel.Content = "Loading: RAM Info";
            loadingPage.progressBar.Value = 18;
            hardwareSpecificationsPage.RetrieveRAMInfo();
            await Delay(200);

            // Storage
            loadingPage.loadingLabel.Content = "Loading: Storage Info";
            loadingPage.progressBar.Value = 27;
            hardwareSpecificationsPage.RetrieveStorageInfo();
            await Delay(200);

            // CPU Architecture
            loadingPage.loadingLabel.Content = "Loading: CPU Architecture Info";
            loadingPage.progressBar.Value = 36;
            hardwareSpecificationsPage.RetrieveCPUArchitectureInfo();
            await Delay(200);

            // BIOS Mode
            loadingPage.loadingLabel.Content = "Loading: BIOS Mode Info";
            loadingPage.progressBar.Value = 45;
            hardwareSpecificationsPage.RetrieveBIOSModeInfo();
            await Delay(200);


            // Secure Boot
            loadingPage.loadingLabel.Content = "Loading: Secure Boot Info";
            loadingPage.progressBar.Value = 54;
            hardwareSpecificationsPage.RetrieveSecureBootInfo();
            await Delay(200);


            // TPM
            loadingPage.loadingLabel.Content = "Loading: TPM Info";
            loadingPage.progressBar.Value = 63;
            hardwareSpecificationsPage.RetrieveTPMInfo();
            await Delay(200);


            // Check mobo model
            loadingPage.loadingLabel.Content = "Loading: Motherboard model";
            loadingPage.progressBar.Value = 72;
            hardwareSpecificationsPage.RetrieveMotherboardInfo();
            await Delay(200);

            // Check Windows version
            loadingPage.loadingLabel.Content = "Loading: Operating System info";
            loadingPage.progressBar.Value = 81;
            operatingSystemPage.RetrieveOSInfo();
            await Delay(200);

            // Startup apps
            loadingPage.loadingLabel.Content = "Loading: Startup apps list";
            loadingPage.progressBar.Value = 90;
            startupItemsPage.RetrieveStartupItems(operatingSystemPage);

            loadingPage.loadingLabel.Content = "Loading: Done";
            loadingPage.progressBar.Value = 100;
            await Delay(200);
            loadingPage.Close();
            this.Show();
        }

        private async Task Delay(int howlong)
        {
            await Task.Delay(howlong);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void UserStartupFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
        }

        private void AllUsersStartupFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
        }

        private void RunUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe", Properties.Resources.regjump);
            Process process = new Process();
            process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
            process.StartInfo.Arguments = @"/accepteula HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            process.Start();
        }

        private void RunAllUsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe", Properties.Resources.regjump);
            Process process = new Process();
            process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
            process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            process.Start();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
            WindowChrome.SetWindowChrome(this, new WindowChrome()
            {
                GlassFrameThickness = new Thickness(0, 1, 0, 0),
                UseAeroCaptionButtons = false,
                CornerRadius = new CornerRadius(0),
                ResizeBorderThickness = new Thickness(4),
                NonClientFrameEdges = NonClientFrameEdges.None,
                CaptionHeight = 36d,

            });

            WindowHelper.SetApplyBackground(this, false);
            Acrylic10Helper.Apply(this, true);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            iNKORE.UI.WPF.Modern.Controls.Page page;
            if (args.IsSettingsSelected)
            {
                page = settingsPage;
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                switch (item.Tag)
                {
                    case "HardwareSpecificationsPage":
                        page = hardwareSpecificationsPage;
                        break;
                    case "OperatingSystemPage":
                        page = operatingSystemPage;
                        break;
                    case "StartupItemsPage":
                        page = startupItemsPage;
                        break;
                    case "CPUPage":
                        CPUPage cpuPage = new CPUPage();
                        page = cpuPage;
                        break;
                    default:
                        page = hardwareSpecificationsPage;
                        break;
                }
            }
            ContentFrame.Navigate(page);
        }
    }
}
