using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using iNKORE.UI.WPF.Modern.Helpers.Styles;
using KnowMySystem.Pages.Hardware_Specifications_Pages;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static LoadingPage loadingPage = new LoadingPage();
        private static OperatingSystemPage operatingSystemPage = new OperatingSystemPage();
        private static HardwareSpecificationsPage hardwareSpecificationsPage = new HardwareSpecificationsPage(operatingSystemPage);
        private static StartupItemsPage startupItemsPage = new StartupItemsPage();
        private static SettingsPage settingsPage = new SettingsPage();

        private static CPUPage cpuPage;

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
            await Delay(200);
            hardwareSpecificationsPage.RetrieveCPUInfo();
            cpuPage = new CPUPage();

            // GPU
            loadingPage.loadingLabel.Content = "Loading: GPU Info";
            loadingPage.progressBar.Value = 9;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveGPUInfo();

            // RAM
            loadingPage.loadingLabel.Content = "Loading: RAM Info";
            loadingPage.progressBar.Value = 18;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveRAMInfo();

            // Storage
            loadingPage.loadingLabel.Content = "Loading: Storage Info";
            loadingPage.progressBar.Value = 27;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveStorageInfo();

            // CPU Architecture
            loadingPage.loadingLabel.Content = "Loading: CPU Architecture Info";
            loadingPage.progressBar.Value = 36;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveCPUArchitectureInfo();

            // BIOS Mode
            loadingPage.loadingLabel.Content = "Loading: BIOS Mode Info";
            loadingPage.progressBar.Value = 45;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveBIOSModeInfo();


            // Secure Boot
            loadingPage.loadingLabel.Content = "Loading: Secure Boot Info";
            loadingPage.progressBar.Value = 54;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveSecureBootInfo();


            // TPM
            loadingPage.loadingLabel.Content = "Loading: TPM Info";
            loadingPage.progressBar.Value = 63;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveTPMInfo();


            // Check mobo model
            loadingPage.loadingLabel.Content = "Loading: Motherboard model";
            loadingPage.progressBar.Value = 72;
            await Delay(200);
            hardwareSpecificationsPage.RetrieveMotherboardInfo();


            // Check Windows version
            loadingPage.loadingLabel.Content = "Loading: Operating System info";
            loadingPage.progressBar.Value = 81;
            await Delay(200);
            operatingSystemPage.RetrieveOSInfo();

            // Startup apps
            loadingPage.loadingLabel.Content = "Loading: Startup apps list";
            loadingPage.progressBar.Value = 90;
            await Delay(200);
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
