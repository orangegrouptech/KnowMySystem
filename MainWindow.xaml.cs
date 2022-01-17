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
using ModernWpf;
using Microsoft.VisualBasic.Devices;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoadingPage loadingpage = new LoadingPage();
        public MainWindow()
        {
            InitializeComponent();
            compName.Content = Environment.MachineName;
            this.Hide();
            RegistryKey checkdarklightmode = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Orange Group\KnowYourSystem");
            if(checkdarklightmode.GetValue("DarkMode") == null)
            {
                systemRadioButton.IsChecked = true;
            } else
            if((int)checkdarklightmode.GetValue("DarkMode") == 1)
            {
                darkRadioButton.IsChecked = true;
            } else if((int)checkdarklightmode.GetValue("DarkMode") == 0)
            {
                lightRadioButton.IsChecked = true;
            } else
            {
                systemRadioButton.IsChecked = true;
            }
            loadingpage.Show();
            Main2();
        }

        private async void Main2()
        {
            //CPU
            loadingpage.loadingLabel.Content = "Loading: CPU Info";
            loadingpage.progressBar.Value = 0;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                cpu.Content = "CPU: " + (string)mo["Name"];
            }
            await Delay(200);


            //GPU
            loadingpage.loadingLabel.Content = "Loading: GPU Info";
            loadingpage.progressBar.Value = 9;
            using (var searcher1 = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher1.Get())
                {
                    gpu.Content = "GPU: " + obj["Name"];
                }
            }
            await Delay(200);


            //RAM
            loadingpage.loadingLabel.Content = "Loading: RAM Info";
            loadingpage.progressBar.Value = 18;
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(objectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("Select * from Win32_PhysicalMemory");
            var ramspeed = "";
            foreach (ManagementObject obj in searcher2.Get())
            {
                ramspeed = Convert.ToString(obj["Speed"]);
                if (ramspeed == null || ramspeed == "") ramspeed = "Unknown ";
            }
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                var newram = Convert.ToInt32(managementObject["TotalVisibleMemorySize"]) / 1000 / 1000;
                ram.Content = "RAM: " + newram + "GB " + ramspeed + "MHz";
            }
            await Delay(200);


            //Storage
            loadingpage.loadingLabel.Content = "Loading: Storage Info";
            loadingpage.progressBar.Value = 27;
            DriveInfo mainDrive = new DriveInfo(System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
            var totalsize = mainDrive.TotalSize / 1024 / 1024 / 1024;
            storage.Content = "Storage on Windows drive: " + totalsize + "GB";
            await Delay(200);


            //CPU Architecture
            loadingpage.loadingLabel.Content = "Loading: CPU Architecture Info";
            loadingpage.progressBar.Value = 36;
            bool is64 = System.Environment.Is64BitOperatingSystem;
            if (is64 == true)
            {
                cpuArchitecture.Content = "CPU Architecture: 64-bit";
            } else
            {
                cpuArchitecture.Content = "CPU Architecture: 32-bit";
            }
            await Delay(200);


            //BIOS Mode
            loadingpage.loadingLabel.Content = "Loading: BIOS Mode Info";
            loadingpage.progressBar.Value = 45;
            Process process2 = new Process();
            process2.StartInfo.UseShellExecute = false;
            process2.StartInfo.RedirectStandardOutput = true;
            process2.StartInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            process2.StartInfo.Arguments = "bcdedit";
            process2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process2.StartInfo.CreateNoWindow = true;
            process2.StartInfo.Verb = "runas";
            process2.Start();
            string s2 = process2.StandardOutput.ReadToEnd();
            process2.WaitForExit();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem");
            await Delay(100);

            using (StreamWriter outfile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\BIOSMode.txt"))
            {
                outfile.Write(s2);
            }
            using (StreamReader sr = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\BIOSMode.txt"))
            {
                string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\BIOSMode.txt");
                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i].ToLower().Contains(@"path                    \windows\system32\winload.efi")/* || lines[20].Contains(@"path                    \WINDOWS\system32\winload.efi")*/)
                    {
                        biosMode.Content = "BIOS Mode: UEFI";
                        break;
                    }
                    else
                    {
                        biosMode.Content = "BIOS Mode: Legacy BIOS";
                    }
                }

            }
            await Delay(200);


            //Secure Boot
            loadingpage.loadingLabel.Content = "Loading: Secure Boot Info";
            loadingpage.progressBar.Value = 54;
            try
            {
                RegistryKey securebootstatuskey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SecureBoot\State");
                var securebootstatus = securebootstatuskey.GetValue("UEFISecureBootEnabled");
                if (Convert.ToInt32(securebootstatus) == 1)
                {
                    secureBoot.Content = "Secure Boot: Enabled";
                }
                else if (Convert.ToInt32(securebootstatus) == 0)
                {
                    secureBoot.Content = "Secure Boot: Disabled";
                }
            }
            catch
            {
                secureBoot.Content = "Secure Boot: Registry Entry not found.";
            }
            await Delay(200);


            //TPM
            loadingpage.loadingLabel.Content = "Loading: TPM Info";
            loadingpage.progressBar.Value = 63;
            RegistryKey checkwindowsversionminor = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var versionminor = checkwindowsversionminor.GetValue("DisplayVersion");
            if(versionminor == null)
            {
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\wmic.exe", Properties.Resources.wmicwin7);
            } else
            {
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\wmic.exe", Properties.Resources.wmicwin10);
            }
            Process wmicTPMVersionProcess = new Process();
            wmicTPMVersionProcess.StartInfo.UseShellExecute = false;
            wmicTPMVersionProcess.StartInfo.RedirectStandardOutput = true;
            wmicTPMVersionProcess.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\wmic.exe";
            wmicTPMVersionProcess.StartInfo.Arguments = @"/namespace:\\root\CIMV2\Security\MicrosoftTpm path Win32_Tpm get /value";
            wmicTPMVersionProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            wmicTPMVersionProcess.StartInfo.CreateNoWindow = true;
            wmicTPMVersionProcess.StartInfo.Verb = "runas";
            wmicTPMVersionProcess.Start();
            string s = wmicTPMVersionProcess.StandardOutput.ReadToEnd();
            wmicTPMVersionProcess.WaitForExit();

            using (StreamWriter outfile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\TPMresult.txt"))
            {
                outfile.Write(s);
                await Delay(200);
            }
            using (StreamReader sr = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\TPMresult.txt"))
            {
                string[] resultLines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\TPMresult.txt");
                sr.Close();

                bool tpmEnabled = false;
                bool tpmActivated = false;
                bool tpmOwned = false;
                Version tpmVersion = null;

                foreach (string resultLine in resultLines)
                {
                    if (resultLine == "IsEnabled_InitialValue=TRUE")
                    {
                        tpmEnabled = true;
                    }
                    else if (resultLine == "IsActivated_InitialValue=TRUE")
                    {
                        tpmActivated = true;
                    }
                    else if (resultLine == "IsOwned_InitialValue=TRUE")
                    {
                        tpmOwned = true;
                    }
                    else if (resultLine.Contains("SpecVersion="))
                    {
                        tpmVersion = new Version(resultLine.Replace("SpecVersion=", string.Empty).Split(',')[0].TrimStart().TrimEnd());
                    }
                }

                if (tpmEnabled)
                {
                    if (tpmActivated && tpmOwned)
                    {
                        tpm.Content = "TPM: Version " + tpmVersion + ", Present and enabled";
                    }
                    else
                    {
                        tpm.Content = "TPM: Version " + tpmVersion + ", Present but not enabled";
                    }
                }
                else
                {
                    tpm.Content = "TPM: Not present";
                }
            }
            await Delay(200);


            //check mobo model
            loadingpage.loadingLabel.Content = "Loading: Motherboard model";
            loadingpage.progressBar.Value = 72;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    motherboard.Content = "Motherboard: " + obj["Product"];
            }
            searcher.Dispose();
            await Delay(200);


            //check Windows version
            loadingpage.loadingLabel.Content = "Loading: Operating System info";
            loadingpage.progressBar.Value = 81;
            RegistryKey checkwindowsversion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            //var productname = Convert.ToString(checkwindowsversion.GetValue("ProductName"));
            ComputerInfo computerInfo = new ComputerInfo();
            string productname = computerInfo.OSFullName.Replace("Microsoft ", "");
            editionValue.Content = productname;

            RegistryKey checkwindowsminorversion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var minorversion = checkwindowsminorversion.GetValue("DisplayVersion");

            RegistryKey checkbuild = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var buildnumber = checkbuild.GetValue("CurrentBuildNumber");

            if (Convert.ToInt32(buildnumber) > 21390)
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 11 logo.png"));
                versionValue.Content = minorversion;

                //build number
                buildNumberValue.Content = buildnumber;


                //check branch
                var branchraw = checkwindowsversion.GetValue("BuildBranch");
                var branch = branchraw.ToString().Replace("_", "__");
                branchValue.Content = branch;

                //check insider
                RegistryKey checkinsider = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\Applicability");
                var insiderstatus = Convert.ToString(checkinsider.GetValue("BranchName"));
                if (insiderstatus != null && insiderstatus == "ReleasePreview")
                {
                    insiderStatusValue.Content = "Yes";
                    insiderChannelValue.Content = "Release Preview";
                } else if (insiderstatus != null && insiderstatus == "Beta")
                {
                    insiderStatusValue.Content = "Yes";
                    insiderChannelValue.Content = "Beta";
                } else if (insiderstatus != null && insiderstatus == "Dev")
                {
                    insiderStatusValue.Content = "Yes";
                    insiderChannelValue.Content = "Dev";
                } else
                {
                    insiderStatusValue.Content = "No";
                    insiderChannelValue.Content = "N/A";
                    insiderStatusValue.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
                    insiderChannelValue.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
                    branchValue.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
                }
            }
            else if (productname.Contains("Windows 10"))
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 10 logo.png"));
                versionValue.Content = minorversion;

                //build number
                buildNumberValue.Content = buildnumber;


                //check branch
                var branchraw = checkwindowsversion.GetValue("BuildBranch");
                var branch = branchraw.ToString().Replace("_", "__");
                branchValue.Content = branch;


                //check insider
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
                    insiderStatusValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                    insiderChannelValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                    branchValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                }
            }
            else if (productname.Contains("Windows 8"))
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 8 logo.png"));
                versionValue.Content = "N/A";
                insiderStatusValue.Content = "N/A";
                insiderChannelValue.Content = "N/A";
                branchValue.Content = "N/A";
                branchValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderStatusValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderChannelValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                //build number
                buildNumberValue.Content = buildnumber;
            }
            else if (productname.Contains("Windows 7"))
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows 7 logo.png"));
                versionValue.Content = "N/A";
                insiderStatusValue.Content = "N/A";
                insiderChannelValue.Content = "N/A";
                branchValue.Content = "N/A";
                branchValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderStatusValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderChannelValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                //build number
                buildNumberValue.Content = buildnumber;
            } else
            {
                windowsLogo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/windows unknown logo.png"));
                versionValue.Content = "Not Supported";
                branchValue.Content = "Not Supported";
                insiderStatusValue.Content = "Not Supported";
                insiderChannelValue.Content = "Not Supported";
                branchValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderStatusValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                insiderChannelValue.SetResourceReference(Control.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
                //build number
                buildNumberValue.Content = buildnumber;
            }
            await Delay(200);


            //Startup apps
            loadingpage.loadingLabel.Content = "Loading: Startup apps list";
            loadingpage.progressBar.Value = 90;
            var names = new List<string>();
            var namesuser = new List<string>();
            var statuses = new List<string>();
            RegistryKey run = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            RegistryKey runuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            RegistryKey run32status = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run32");
            RegistryKey status = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run");
            RegistryKey userstatus = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run");
            RegistryKey shellkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            RegistryKey userinitkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            var userstartupfolder = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            var allusersstartupfolder = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            var statusResult = "";
            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Name";
            nameColumn.Binding = new Binding("Name");
            nameColumn.Width = 200;
            startupItemsList.Columns.Add(nameColumn);
            DataGridTextColumn locationColumn = new DataGridTextColumn();
            locationColumn.Header = "Location";
            locationColumn.Width = 340;
            locationColumn.Binding = new Binding("Location");
            startupItemsList.Columns.Add(locationColumn);
            DataGridTextColumn startupTypeColumn = new DataGridTextColumn();
            startupTypeColumn.Header = "Startup Type";
            startupTypeColumn.Width = 130;
            startupTypeColumn.Binding = new Binding("Type");
            startupItemsList.Columns.Add(startupTypeColumn);
            DataGridTextColumn statusColumn = new DataGridTextColumn();
            statusColumn.Header = "Status";
            statusColumn.Width = 80;
            statusColumn.Binding = new Binding("Status");
            startupItemsList.Columns.Add(statusColumn);

            var shellKeyContent = shellkey.GetValue("Shell");
            if (Convert.ToString(shellKeyContent).ToLower() == "explorer.exe")
            { }
            else
            {
                string[] shellKeyContentString = shellKeyContent.ToString().Split(',');
                for (var i = 0; i < shellKeyContentString.Count(); i++)
                {
                    var fileinfo = new FileInfo(shellKeyContentString[i]).Name;
                    if (shellKeyContentString[i] == "explorer.exe") { }
                    else
                    {
                        startupItemsList.Items.Add(new DataTemplate()
                        {
                            Name = "[SUSPICIOUS] " + fileinfo,
                            Location = shellKeyContentString[i],
                            Type = "Shell",
                            Status = "Enabled",
                        });
                    }
                }
            }

            var userinitKeyContent = userinitkey.GetValue("Userinit");
            if (Convert.ToString(userinitKeyContent).ToLower() == @"c:\windows\system32\userinit.exe,")
            { }
            else
            {
                string[] userinitKeyContentString = userinitKeyContent.ToString().Replace(@"C:\Windows\system32\userinit.exe,", "").Split(',');
                for (var i = 0; i < userinitKeyContentString.Count(); i++)
                {
                    var fileinfo = new FileInfo(userinitKeyContentString[i]).Name;
                    if (userinitKeyContentString[i] == "explorer.exe") { }
                    else
                    {
                        startupItemsList.Items.Add(new DataTemplate()
                        {
                            Name = "[SUSPICIOUS] " + fileinfo,
                            Location = userinitKeyContentString[i],
                            Type = "Userinit",
                            Status = "Enabled"
                        });
                    }
                }
            }

            foreach (string file in userstartupfolder)
            {
                var filename = new FileInfo(file).Name;
                var filelocation = new FileInfo(file).FullName;
                if (filename == "desktop.ini") { }
                else
                {
                    startupItemsList.Items.Add(new DataTemplate()
                    {
                        Name = "[SUSPICIOUS] " + filename,
                        Location = filelocation,
                        Type = "Startup Folder (User)",
                        Status = "Enabled"
                    });
                }
            }

            foreach (string file in allusersstartupfolder)
            {
                var filename = new FileInfo(file).Name;
                var filelocation = new FileInfo(file).FullName;
                if (filename == "desktop.ini") { }
                else
                {
                    startupItemsList.Items.Add(new DataTemplate()
                    {
                        Name = "[SUSPICIOUS] " + filename,
                        Location = filelocation,
                        Type = "Startup Folder (All Users)",
                        Status = "Enabled"
                    });
                }
            }

            foreach (string value in run.GetValueNames())
            {
                if (editionValue.Content.ToString().Contains("Windows 7"))
                {
                    names.Add(value);
                    statusResult = "Enabled";
                }
                else
                {
                    names.Add(value);
                    var statusCheck = (byte[])status.GetValue(value);
                    if (statusCheck == null) break;
                    var statusCheckString = BitConverter.ToString(statusCheck);
                    if (statusCheckString == null || !statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Enabled";
                    }
                    else if (statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Disabled";
                    }
                }

                startupItemsList.Items.Add(new DataTemplate()
                {
                    Name = value,
                    Location = Convert.ToString(run.GetValue(value)),
                    Type = "Run (HKLM)",
                    Status = statusResult
                });

                startupItemsList.RowHeight = 30;
            }

            foreach (string value in runuser.GetValueNames())
            {
                if (editionValue.Content.ToString().Contains("Windows 7"))
                {
                    namesuser.Add(value);
                    statusResult = "Enabled";
                }
                else
                {
                    namesuser.Add(value);
                    var statusCheck = (byte[])userstatus.GetValue(value);
                    if (statusCheck == null) break;
                    var statusCheckString = BitConverter.ToString(statusCheck);
                    if (statusCheckString == null || !statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Enabled";
                    }
                    else if (statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Disabled";
                    }
                }

                startupItemsList.Items.Add(new DataTemplate()
                {
                    Name = value,
                    Location = Convert.ToString(runuser.GetValue(value)),
                    Type = "Run (HKCU)",
                    Status = statusResult
                });

                startupItemsList.RowHeight = 30;
            }

            await Delay(200);




            loadingpage.loadingLabel.Content = "Loading: Done";
            loadingpage.progressBar.Value = 100;
            await Delay(200);
            loadingpage.Close();
            this.Show();
        }

        public class DataTemplate {
            public string Name { get; set; }
            public string Location { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
        }

        private void renamePCButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(editionValue.Content.ToString().Contains("Windows 10"))
            {
                Process opensettings = new Process();
                opensettings.StartInfo.FileName = "ms-settings:about";
                opensettings.StartInfo.UseShellExecute = true;
                opensettings.Start();
                Process renamecomp = new Process();
                renamecomp.StartInfo.FileName = "C:\\Windows\\System32\\SystemSettingsAdminFlows.exe";
                renamecomp.StartInfo.Arguments = "RenamePC";
                renamecomp.StartInfo.Verb = "runas";
                renamecomp.Start();
            } else
            {
                Process.Start("sysdm.cpl");
            }
        }

        private async Task Delay(int howlong)
        {
            await Task.Delay(howlong);
        }

        private void startupItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTemplate selectedRow = (DataTemplate)startupItemsList.SelectedItem;
            if (selectedRow == null) { }
            else
            {
                startupItemsList.ContextMenu.IsEnabled = true;
                if (selectedRow.Status == "Enabled")
                {
                    EnableMenuItem.Header = "Disable";
                    EnableMenuItem.Click += new RoutedEventHandler(DisableMenuItem_Click);
                    EnableDisableButton.IsEnabled = true;
                    EnableDisableButton.Content = "Disable";
                    EnableDisableButton.Click += new RoutedEventHandler(DisableMenuItem_Click);
                }
                else if (selectedRow.Status == "Disabled")
                {
                    EnableMenuItem.Header = "Enable";
                    EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    EnableDisableButton.IsEnabled = true;
                    EnableDisableButton.Content = "Enable";
                    EnableDisableButton.Click += new RoutedEventHandler(EnableMenuItem_Click);
                }
            }
        }

        private void EnableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate selectedRow = (DataTemplate)startupItemsList.SelectedItem;
            var name = selectedRow.Name;
            RegistryKey enablestartupitem = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
            enablestartupitem.SetValue(Convert.ToString(name), new byte[] { 0x02, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
            RegistryKey enablestartupitemuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
            enablestartupitemuser.SetValue(Convert.ToString(name), new byte[] { 0x02, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
            selectedRow.Status = "Enabled";
            EnableMenuItem.Header = "Disable";
            EnableMenuItem.Click += new RoutedEventHandler(DisableMenuItem_Click);
            EnableDisableButton.Content = "Disable";
            EnableDisableButton.Click += new RoutedEventHandler(DisableMenuItem_Click);
            startupItemsList.Items.Refresh();
        }

        private void DisableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate selectedRow = (DataTemplate)startupItemsList.SelectedItem;
            var name = selectedRow.Name;
            var location = selectedRow.Location;
            if (editionValue.Content.ToString().Contains("Windows 7"))
            {
                var warning = MessageBox.Show("Hey, I see you're using Windows 7.\nNo, I'm not going to be as annoying as GWX and bug you to upgrade to Windows 10 (even though you should). \nI just want to say that in Windows 7, there's no such thing as enabling or disabling startup items, so to disable them, a delete of the startup entry is required. This operation is not reversible. \nDo you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(warning == MessageBoxResult.Yes)
                {
                    if (selectedRow.Type == "Shell")
                    {
                        RegistryKey shellkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                        shellkey.SetValue("Shell", shellkey.GetValue("Shell").ToString().Replace(location, ""));
                        startupItemsList.Items.Remove(selectedRow);
                        EnableMenuItem.Header = "Enable";
                        EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                    else if (selectedRow.Type == "Userinit")
                    {
                        RegistryKey userinitkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                        userinitkey.SetValue("Userinit", userinitkey.GetValue("Userinit").ToString().Replace(location, ""));
                        startupItemsList.Items.Remove(selectedRow);
                        EnableMenuItem.Header = "Enable";
                        EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                    else if (selectedRow.Type == "Startup Folder (User)")
                    {
                        File.Delete(selectedRow.Location);
                        startupItemsList.Items.Remove(selectedRow);
                        EnableMenuItem.Header = "Enable";
                        EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                    else if (selectedRow.Type == "Startup Folder (All Users)")
                    {
                        File.Delete(selectedRow.Location);
                        startupItemsList.Items.Remove(selectedRow);
                        EnableMenuItem.Header = "Enable";
                        EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                    else
                    {
                        RegistryKey disablestartupitem = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                        if (disablestartupitem.GetValue(Convert.ToString(name)) == null) { } else { disablestartupitem.DeleteValue(Convert.ToString(name)); };
                        RegistryKey disablestartupitemuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                        if (disablestartupitemuser.GetValue(Convert.ToString(name)) == null) { } else { disablestartupitemuser.DeleteValue(Convert.ToString(name)); };
                        startupItemsList.Items.Remove(selectedRow);
                    }
                }
            }
            else
            {
                if (selectedRow.Type == "Shell")
                {
                    var warning = MessageBox.Show("In order to disable a startup program that's registered under the Shell key, full removal of the startup program will be needed. This action is permanent and you will NOT be able to re-enable this startup program. \nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (warning == MessageBoxResult.Yes)
                    {
                        RegistryKey shellkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                        shellkey.SetValue("Shell", shellkey.GetValue("Shell").ToString().Replace(location, ""));
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                }
                else if (selectedRow.Type == "Userinit")
                {
                    var warning = MessageBox.Show("In order to disable a startup program that's registered under the Userinit key, full removal of the startup program will be needed. This action is permanent and you will NOT be able to re-enable this startup program. \nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (warning == MessageBoxResult.Yes)
                    {
                        RegistryKey userinitkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                        userinitkey.SetValue("Userinit", userinitkey.GetValue("Userinit").ToString().Replace(location, ""));
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                }
                else if (selectedRow.Type == "Startup Folder (User)")
                {
                    var warning = MessageBox.Show("In order to disable a startup program that's in the user startup folder, full removal of the startup program will be needed. This action is permanent and you will NOT be able to re-enable this startup program. \nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (warning == MessageBoxResult.Yes)
                    {
                        File.Delete(selectedRow.Location);
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                }
                else if (selectedRow.Type == "Startup Folder (All Users)")
                {
                    var warning = MessageBox.Show("In order to disable a startup program that's in the all users startup folder, full removal of the startup program will be needed. This action is permanent and you will NOT be able to re-enable this startup program. \nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (warning == MessageBoxResult.Yes)
                    {
                        File.Delete(selectedRow.Location);
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                    }
                }
                else
                {
                    RegistryKey disablestartupitem = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                    disablestartupitem.SetValue(Convert.ToString(name), new byte[] { 0x33, 0x32, 0xFF, 0x00, 0xB3, 0xBB, 0x5E, 0x22, 0xE5, 0xC6, 0xD7, 0x01 });
                    RegistryKey disablestartupitemuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                    disablestartupitemuser.SetValue(Convert.ToString(name), new byte[] { 0x33, 0x32, 0xFF, 0x00, 0xB3, 0xBB, 0x5E, 0x22, 0xE5, 0xC6, 0xD7, 0x01 });
                    selectedRow.Status = "Disabled";
                    EnableDisableButton.Content = "Enable";
                    EnableDisableButton.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    startupItemsList.Items.Refresh();
                }
            }
        }

        private void OpenFileLocationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate selectedRow = (DataTemplate)startupItemsList.SelectedItem;
            var fileLocation = selectedRow.Location;
            Process.Start("explorer.exe", "/select, "+Convert.ToString(fileLocation));
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //LoadingPage loadingPage = new LoadingPage();
            //loadingPage.Show();
            //this.Hide();
            //loadingPage.progressBar.IsIndeterminate = true;
            startupItemsList.Columns.Clear();
            startupItemsList.Items.Clear();
            ProgressRing.IsActive = true;
            RefreshButton.IsEnabled = false;
            EnableDisableButton.IsEnabled = false;
            await Delay(1000);
            //Startup apps
            var names = new List<string>();
            var namesuser = new List<string>();
            var statuses = new List<string>();
            RegistryKey run = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            RegistryKey runuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            RegistryKey run32status = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run32");
            RegistryKey status = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run");
            RegistryKey userstatus = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run");
            RegistryKey shellkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            RegistryKey userinitkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            var userstartupfolder = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            var allusersstartupfolder = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            var statusResult = "";
            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Name";
            nameColumn.Binding = new Binding("Name");
            nameColumn.Width = 200;
            startupItemsList.Columns.Add(nameColumn);
            DataGridTextColumn locationColumn = new DataGridTextColumn();
            locationColumn.Header = "Location";
            locationColumn.Width = 340;
            locationColumn.Binding = new Binding("Location");
            startupItemsList.Columns.Add(locationColumn);
            DataGridTextColumn startupTypeColumn = new DataGridTextColumn();
            startupTypeColumn.Header = "Startup Type";
            startupTypeColumn.Width = 130;
            startupTypeColumn.Binding = new Binding("Type");
            startupItemsList.Columns.Add(startupTypeColumn);
            DataGridTextColumn statusColumn = new DataGridTextColumn();
            statusColumn.Header = "Status";
            statusColumn.Width = 80;
            statusColumn.Binding = new Binding("Status");
            startupItemsList.Columns.Add(statusColumn);

            var shellKeyContent = shellkey.GetValue("Shell");
            if (Convert.ToString(shellKeyContent).ToLower() == "explorer.exe")
            { }
            else
            {
                string[] shellKeyContentString = shellKeyContent.ToString().Split(',');
                for (var i = 0; i < shellKeyContentString.Count(); i++)
                {
                    var fileinfo = new FileInfo(shellKeyContentString[i]).Name;
                    if (shellKeyContentString[i] == "explorer.exe") { }
                    else
                    {
                        startupItemsList.Items.Add(new DataTemplate()
                        {
                            Name = "[SUSPICIOUS] " + fileinfo,
                            Location = shellKeyContentString[i],
                            Type = "Shell",
                            Status = "Enabled",
                        });
                    }
                }
            }

            var userinitKeyContent = userinitkey.GetValue("Userinit");
            if (Convert.ToString(userinitKeyContent).ToLower() == @"c:\windows\system32\userinit.exe,")
            { }
            else
            {
                string[] userinitKeyContentString = userinitKeyContent.ToString().Replace(@"C:\Windows\system32\userinit.exe,", "").Split(',');
                for (var i = 0; i < userinitKeyContentString.Count(); i++)
                {
                    var fileinfo = new FileInfo(userinitKeyContentString[i]).Name;
                    if (userinitKeyContentString[i] == "explorer.exe") { }
                    else
                    {
                        startupItemsList.Items.Add(new DataTemplate()
                        {
                            Name = "[SUSPICIOUS] " + fileinfo,
                            Location = userinitKeyContentString[i],
                            Type = "Userinit",
                            Status = "Enabled"
                        });
                    }
                }
            }

            foreach (string file in userstartupfolder)
            {
                var filename = new FileInfo(file).Name;
                var filelocation = new FileInfo(file).FullName;
                if (filename == "desktop.ini") { }
                else
                {
                    startupItemsList.Items.Add(new DataTemplate()
                    {
                        Name = "[SUSPICIOUS] " + filename,
                        Location = filelocation,
                        Type = "Startup Folder (User)",
                        Status = "Enabled"
                    });
                }
            }

            foreach (string file in allusersstartupfolder)
            {
                var filename = new FileInfo(file).Name;
                var filelocation = new FileInfo(file).FullName;
                if (filename == "desktop.ini") { }
                else
                {
                    startupItemsList.Items.Add(new DataTemplate()
                    {
                        Name = "[SUSPICIOUS] " + filename,
                        Location = filelocation,
                        Type = "Startup Folder (All Users)",
                        Status = "Enabled"
                    });
                }
            }

            foreach (string value in run.GetValueNames())
            {
                if (editionValue.Content.ToString().Contains("Windows 7"))
                {
                    names.Add(value);
                    statusResult = "Enabled";
                }
                else
                {
                    names.Add(value);
                    var statusCheck = (byte[])status.GetValue(value);
                    if (statusCheck == null) break;
                    var statusCheckString = BitConverter.ToString(statusCheck);
                    if (statusCheckString == null || !statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Enabled";
                    }
                    else if (statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Disabled";
                    }
                }

                startupItemsList.Items.Add(new DataTemplate()
                {
                    Name = value,
                    Location = Convert.ToString(run.GetValue(value)),
                    Type = "Run (HKLM)",
                    Status = statusResult
                });

                startupItemsList.RowHeight = 30;
            }

            foreach (string value in runuser.GetValueNames())
            {
                if (editionValue.Content.ToString().Contains("Windows 7"))
                {
                    namesuser.Add(value);
                    statusResult = "Enabled";
                }
                else
                {
                    namesuser.Add(value);
                    var statusCheck = (byte[])userstatus.GetValue(value);
                    if (statusCheck == null) break;
                    var statusCheckString = BitConverter.ToString(statusCheck);
                    if (statusCheckString == null || !statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Enabled";
                    }
                    else if (statusCheckString.Contains("D7-01"))
                    {
                        statusResult = "Disabled";
                    }
                }

                startupItemsList.Items.Add(new DataTemplate()
                {
                    Name = value,
                    Location = Convert.ToString(runuser.GetValue(value)),
                    Type = "Run (HKCU)",
                    Status = statusResult
                });

                startupItemsList.RowHeight = 30;
            }

            await Delay(200);
            await Delay(2000);
            startupItemsList.Items.Refresh();
            ProgressRing.IsActive = false;
            RefreshButton.IsEnabled = true;
            this.Show();
    }

        private void OpenStartupEntryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem");
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe", Properties.Resources.regjump);
            var selectedItem = (DataTemplate)startupItemsList.SelectedItem;

            if(selectedItem.Type == "Run (HKLM)")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\" + selectedItem.Name;
                process.Start();
            } else if (selectedItem.Type == "Run (HKCU)")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\" + selectedItem.Name;
                process.Start();
            } else if (selectedItem.Type == "Shell")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\" + selectedItem.Name;
                process.Start();
            } else if (selectedItem.Type == "Userinit")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\" + selectedItem.Name;
                process.Start();
            } else if (selectedItem.Type == "Startup Folder (User)")
            {
                Process.Start("explorer.exe", "/select " + Environment.GetFolderPath(Environment.SpecialFolder.Startup) + selectedItem.Name);
            } else if (selectedItem.Type == "Startup Folder (All Users)")
            {
                Process.Start("explorer.exe", "/select " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + selectedItem.Name);
            }
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
            Process process = new Process();
            process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
            process.StartInfo.Arguments = @"/accepteula HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            process.Start();
        }

        private void RunAllUsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
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

        private void lightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Orange Group\KnowYourSystem");
            registryKey.SetValue("DarkMode", 0);
        }

        private void darkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Orange Group\KnowYourSystem");
            registryKey.SetValue("DarkMode", 1);
        }

        private void systemRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = null;
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Orange Group\KnowYourSystem");
            if (registryKey.GetValue("DarkMode") == null) { } else registryKey.DeleteValue("DarkMode");
        }
    }
}
