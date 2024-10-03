using iNKORE.UI.WPF.Modern.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for StartupItemsPage.xaml
    /// </summary>
    public partial class StartupItemsPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        public StartupItemsPage()
        {
            InitializeComponent();
        }

        public class DataTemplate
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
        }

        private static OperatingSystemPage operatingSystemPage;

        public async void RetrieveStartupItems(OperatingSystemPage osp)
        {
            operatingSystemPage = osp;

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
            RegistryKey startupitemsstatususer = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
            RegistryKey startupitemsstatusallusers = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);

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
            locationColumn.Width = 305;
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
            if (Convert.ToString(shellKeyContent).ToLower() != "explorer.exe")
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
            if (Convert.ToString(userinitKeyContent).ToLower() != @"c:\windows\system32\userinit.exe,")
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
                    try
                    {
                        byte[] value = (byte[])startupitemsstatususer.GetValue(filename);
                        if (value[0] % 2 == 0)
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (User)",
                                Status = "Enabled"
                            });
                        }
                        else
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (User)",
                                Status = "Disabled"
                            });
                        }
                    }
                    catch
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
            }

            foreach (string file in allusersstartupfolder)
            {
                var filename = new FileInfo(file).Name;
                var filelocation = new FileInfo(file).FullName;
                if (filename == "desktop.ini") { }
                else
                {
                    try
                    {
                        byte[] value = (byte[])startupitemsstatusallusers.GetValue(filename);
                        if (value[0] % 2 == 0)
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (All Users)",
                                Status = "Enabled"
                            });
                        }
                        else
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (All Users)",
                                Status = "Disabled"
                            });
                        }
                    }
                    catch
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
            }

            foreach (string value in run.GetValueNames())
            {
                if (operatingSystemPage.editionValue.Content.ToString().Contains("Windows 7"))
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
                if (operatingSystemPage.editionValue.Content.ToString().Contains("Windows 7"))
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

            await Task.Delay(200);
            startupItemsList.Items.SortDescriptions.Clear();
            startupItemsList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(startupItemsList.Columns[0].SortMemberPath, System.ComponentModel.ListSortDirection.Ascending));
            startupItemsList.Items.Refresh();
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
            var location = selectedRow.Location;
            if (selectedRow.Type == "Startup Folder (User)")
            {
                RegistryKey startupitemsstatususer = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
                var filename = name.Replace("[SUSPICIOUS] ", "");
                startupitemsstatususer.SetValue(filename, new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                selectedRow.Status = "Enabled";
                EnableMenuItem.Header = "Disable";
                EnableMenuItem.Click += new RoutedEventHandler(DisableMenuItem_Click);
                EnableDisableButton.Content = "Disable";
                EnableDisableButton.Click += new RoutedEventHandler(DisableMenuItem_Click);
                startupItemsList.Items.Refresh();
            }
            else if (selectedRow.Type == "Startup Folder (All Users)")
            {
                RegistryKey startupitemsstatusallusers = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
                var filename = name.Replace("[SUSPICIOUS] ", "");
                startupitemsstatusallusers.SetValue(filename, new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                selectedRow.Status = "Enabled";
                EnableMenuItem.Header = "Disable";
                EnableMenuItem.Click += new RoutedEventHandler(DisableMenuItem_Click);
                EnableDisableButton.Content = "Disable";
                EnableDisableButton.Click += new RoutedEventHandler(DisableMenuItem_Click);
                startupItemsList.Items.Refresh();
            }
            else
            {
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
        }

        private async void DisableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate selectedRow = (DataTemplate)startupItemsList.SelectedItem;
            var name = selectedRow.Name;
            var location = selectedRow.Location;
            if (operatingSystemPage.editionValue.Content.ToString().Contains("Windows 7"))
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Information";
                dialog.Content = "In Windows 7, there's no such thing as enabling or disabling startup items, so to disable them, a delete of the startup entry is required. This operation is not reversible. \nDo you want to continue?";
                dialog.PrimaryButtonText = "Yes";
                dialog.SecondaryButtonText = "No";
                dialog.DefaultButton = ContentDialogButton.Primary;
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
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
                        startupItemsList.Items.Refresh();
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
                        startupItemsList.Items.Refresh();
                    }
                    else if (selectedRow.Type == "Startup Folder (User)")
                    {
                        File.Delete(selectedRow.Location);
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                        startupItemsList.Items.Refresh();
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
                        startupItemsList.Items.Refresh();
                    }
                    else
                    {
                        RegistryKey disablestartupitem = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                        if (disablestartupitem.GetValue(Convert.ToString(name)) == null) { } else { disablestartupitem.DeleteValue(Convert.ToString(name)); };
                        RegistryKey disablestartupitemuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                        if (disablestartupitemuser.GetValue(Convert.ToString(name)) == null) { } else { disablestartupitemuser.DeleteValue(Convert.ToString(name)); };
                        EnableMenuItem.Header = "Enable";
                        EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                        EnableDisableButton.Content = "Enable";
                        startupItemsList.Items.Remove(selectedRow);
                        startupItemsList.Items.Refresh();
                    }
                }
            }
            else
            {
                if (selectedRow.Type == "Shell")
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Information";
                    dialog.Content = "In order to disable a startup program that's registered under the Shell key, full removal of the startup program will be needed. This action is not permanent and you will NOT be able to re-enable this startup program. \nDo you wish to continue?";
                    dialog.PrimaryButtonText = "Yes";
                    dialog.SecondaryButtonText = "No";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        RegistryKey shellkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                        shellkey.SetValue("Shell", shellkey.GetValue("Shell").ToString().Replace(location, ""));
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                        startupItemsList.Items.Refresh();
                    }
                }
                else if (selectedRow.Type == "Userinit")
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Information";
                    dialog.Content = "In order to disable a startup program that's registered under the Userinit key, full removal of the startup program will be needed. This action is not permanent and you will NOT be able to re-enable this startup program. \nDo you wish to continue?";
                    dialog.PrimaryButtonText = "Yes";
                    dialog.SecondaryButtonText = "No";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        RegistryKey userinitkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                        userinitkey.SetValue("Userinit", userinitkey.GetValue("Userinit").ToString().Replace(location, ""));
                        startupItemsList.Items.Remove(selectedRow);
                        EnableDisableButton.Content = "Enable";
                        EnableDisableButton.IsEnabled = false;
                        EnableMenuItem.IsEnabled = false;
                        startupItemsList.Items.Refresh();
                    }
                }
                else if (selectedRow.Type == "Startup Folder (User)")
                {
                    RegistryKey startupitemsstatususer = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
                    var filename = name.Replace("[SUSPICIOUS] ", "");
                    startupitemsstatususer.SetValue(filename, new byte[] { 0x03, 0x00, 0x00, 0x00, 0x9F, 0xC9, 0xA5, 0xD3, 0xDA, 0x0C, 0xD8, 0x01 });
                    selectedRow.Status = "Disabled";
                    EnableMenuItem.Header = "Enable";
                    EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    EnableDisableButton.Content = "Enable";
                    EnableDisableButton.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    startupItemsList.Items.Refresh();
                }
                else if (selectedRow.Type == "Startup Folder (All Users)")
                {
                    RegistryKey startupitemsstatusallusers = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
                    var filename = name.Replace("[SUSPICIOUS] ", "");
                    startupitemsstatusallusers.SetValue(filename, new byte[] { 0x03, 0x00, 0x00, 0x00, 0x9F, 0xC9, 0xA5, 0xD3, 0xDA, 0x0C, 0xD8, 0x01 });
                    selectedRow.Status = "Disabled";
                    EnableMenuItem.Header = "Enable";
                    EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    EnableDisableButton.Content = "Enable";
                    EnableDisableButton.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    startupItemsList.Items.Refresh();
                }
                else
                {
                    RegistryKey disablestartupitem = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                    disablestartupitem.SetValue(Convert.ToString(name), new byte[] { 0x33, 0x32, 0xFF, 0x00, 0xB3, 0xBB, 0x5E, 0x22, 0xE5, 0xC6, 0xD7, 0x01 });
                    RegistryKey disablestartupitemuser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
                    disablestartupitemuser.SetValue(Convert.ToString(name), new byte[] { 0x33, 0x32, 0xFF, 0x00, 0xB3, 0xBB, 0x5E, 0x22, 0xE5, 0xC6, 0xD7, 0x01 });
                    selectedRow.Status = "Disabled";
                    EnableMenuItem.Header = "Enable";
                    EnableMenuItem.Click += new RoutedEventHandler(EnableMenuItem_Click);
                    EnableDisableButton.Content = "Enable";
                    startupItemsList.Items.Refresh();
                }
            }
        }

        private void OpenFileLocationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate selectedRow = (DataTemplate)startupItemsList.SelectedItem;
            var fileLocation = selectedRow.Location;
            if (fileLocation.Contains("/"))
            {
                var index = fileLocation.IndexOf("/");
                fileLocation = fileLocation.Substring(0, index - 1);
            }
            else if (fileLocation.Contains("-"))
            {
                //Too bad for program names that contain a -
                var index = fileLocation.IndexOf("-");
                fileLocation = fileLocation.Substring(0, index - 1);
            }
            Process.Start("explorer.exe", "/select, \"" + Convert.ToString(fileLocation) + "\"");
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
            await Task.Delay(1000);
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
            RegistryKey startupitemsstatususer = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
            RegistryKey startupitemsstatusallusers = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);
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
            locationColumn.Width = 305;
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
                    try
                    {
                        byte[] value = (byte[])startupitemsstatususer.GetValue(filename);
                        if (value[0] % 2 == 0)
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (User)",
                                Status = "Enabled"
                            });
                        }
                        else
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (User)",
                                Status = "Disabled"
                            });
                        }
                    }
                    catch
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
            }

            foreach (string file in allusersstartupfolder)
            {
                var filename = new FileInfo(file).Name;
                var filelocation = new FileInfo(file).FullName;
                if (filename == "desktop.ini") { }
                else
                {
                    try
                    {
                        byte[] value = (byte[])startupitemsstatusallusers.GetValue(filename);
                        if (value[0] % 2 == 0)
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (All Users)",
                                Status = "Enabled"
                            });
                        }
                        else
                        {
                            startupItemsList.Items.Add(new DataTemplate()
                            {
                                Name = "[SUSPICIOUS] " + filename,
                                Location = filelocation,
                                Type = "Startup Folder (All Users)",
                                Status = "Disabled"
                            });
                        }
                    }
                    catch
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
            }

            foreach (string value in run.GetValueNames())
            {
                if (operatingSystemPage.editionValue.Content.ToString().Contains("Windows 7"))
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
                if (operatingSystemPage.editionValue.Content.ToString().Contains("Windows 7"))
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
            startupItemsList.Items.SortDescriptions.Clear();
            startupItemsList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(startupItemsList.Columns[0].SortMemberPath, System.ComponentModel.ListSortDirection.Ascending));
            startupItemsList.Items.Refresh();
            await Task.Delay(200);
            await Task.Delay(2000);
            ProgressRing.IsActive = false;
            RefreshButton.IsEnabled = true;
        }

        private void OpenStartupEntryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem");
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe", Properties.Resources.regjump);
            var selectedItem = (DataTemplate)startupItemsList.SelectedItem;

            if (selectedItem.Type == "Run (HKLM)")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\" + selectedItem.Name;
                process.Start();
            }
            else if (selectedItem.Type == "Run (HKCU)")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\" + selectedItem.Name;
                process.Start();
            }
            else if (selectedItem.Type == "Shell")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\" + selectedItem.Name;
                process.Start();
            }
            else if (selectedItem.Type == "Userinit")
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Orange Group\KnowYourSystem\regjump.exe";
                process.StartInfo.Arguments = @"/accepteula HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\" + selectedItem.Name;
                process.Start();
            }
            else if (selectedItem.Type == "Startup Folder (User)")
            {
                var Name = selectedItem.Name.Replace("[SUSPICIOUS] ", "");
                Process.Start("explorer.exe", "/select, \"" + Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Name + "\"");
            }
            else if (selectedItem.Type == "Startup Folder (All Users)")
            {
                var Name = selectedItem.Name.Replace("[SUSPICIOUS] ", "");
                Process.Start("explorer.exe", "/select, \"" + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + "\\" + Name + "\"");
            }
        }
    }
}
