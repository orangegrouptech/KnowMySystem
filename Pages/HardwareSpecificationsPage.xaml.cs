using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Input;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for HardwareSpecificationsPage.xaml
    /// </summary>
    public partial class HardwareSpecificationsPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        public HardwareSpecificationsPage()
        {
            InitializeComponent();
            // Computer Name
            compName.Content = Environment.MachineName;
        }

        public void RetrieveCPUInfo()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                cpu.Content = "CPU: " + (string)mo["Name"];
            }
        }

        public void RetrieveGPUInfo()
        {
            bool hasiGPU = false;
            string iGPUName = "";
            using (var searcher1 = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher1.Get())
                {
                    if (obj["Name"].ToString() == "Microsoft Basic Display Adapter")
                    {
                        gpu.Content = "GPU: Install display drivers to detect";
                        break;
                    }
                    //Improved iGPU detector - should work theoretically though this requires testing
                    else if (obj["Name"].ToString() == "AMD Radeon(TM) Graphics" || obj["Name"].ToString().Contains("Intel") && !obj["Name"].ToString().Contains("Intel Arc"))
                    {
                        hasiGPU = true;
                        iGPUName = obj["Name"].ToString();
                    }
                    else
                    {
                        gpu.Content = "GPU: " + obj["Name"];
                        break;
                    }
                }
                if (gpu.Content.ToString() == "GPU: " && hasiGPU == true)
                {
                    gpu.Content = "GPU: " + iGPUName;
                }

            }
        }

        public void RetrieveRAMInfo()
        {
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(objectQuery);

            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("Select * from Win32_PhysicalMemory");
            var ramspeed = "";
            var newram = 0;
            var newMemoryType = "";
            foreach (ManagementObject obj in searcher2.Get())
            {
                try
                {
                    ramspeed = Convert.ToString(obj["ConfiguredClockSpeed"]);
                }
                catch { }
            }
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            {
                newram = Convert.ToInt32(managementObject["TotalVisibleMemorySize"]) / 1000 / 1000;
            }
            foreach (ManagementObject managementObject in searcher2.Get())
            {
                string memoryType = managementObject["MemoryType"].ToString();
                switch (memoryType)
                {
                    case "20":
                        newMemoryType = "DDR";
                        break;

                    case "21":
                        newMemoryType = "DDR2";
                        break;

                    case "24":
                        newMemoryType = "DDR3";
                        break;

                    case "26":
                        newMemoryType = "DDR4";
                        break;

                    case "34":
                        newMemoryType = "DDR5";
                        break;

                    case "0":
                        string memoryType2 = managementObject["SMBIOSMemoryType"]?.ToString() ?? "0";

                        switch (memoryType2)
                        {
                            case "20":
                                newMemoryType = "DDR";
                                break;

                            case "21":
                                newMemoryType = "DDR2";
                                break;

                            case "24":
                                newMemoryType = "DDR3";
                                break;

                            case "26":
                                newMemoryType = "DDR4";
                                break;

                            case "34":
                                newMemoryType = "DDR5";
                                break;

                            default:
                                newMemoryType = "Unknown";
                                break;
                        }
                        break;

                    default:
                        newMemoryType = "Unknown";
                        break;
                }
            }
            if (ramspeed == null || ramspeed == "" || ramspeed == "0")
            {
                ramspeed = "Unknown ";
            }
            else if (newMemoryType == "Unknown")
            {
                //Last last resort RAM type check
                //Banking on nobody being able to reach 4800 MT/s on DDR4 (DDR5 JEDEC = 4800 MT/s)
                //Also not considering the LPDDR5/LPDDR5x users
                if (Convert.ToInt32(ramspeed) >= 4800)
                {
                    newMemoryType = "DDR5";
                }
            }

            ram.Content = "RAM: " + newram + "GB " + ramspeed + "MT/s " + newMemoryType;
        }

        public void RetrieveStorageInfo()
        {
            DriveInfo mainDrive = new DriveInfo(System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
            var totalsize = mainDrive.TotalSize / 1024 / 1024 / 1024;
            storage.Content = "Storage on Windows drive: " + totalsize + "GB";
        }

        public void RetrieveCPUArchitectureInfo()
        {
            bool is64 = System.Environment.Is64BitOperatingSystem;
            if (is64 == true)
            {
                cpuArchitecture.Content = "CPU Architecture: 64-bit";
            }
            else
            {
                cpuArchitecture.Content = "CPU Architecture: 32-bit";
            }
        }

        public void RetrieveBIOSModeInfo()
        {
            Process retrieveBIOSModeInfo = new Process();
            retrieveBIOSModeInfo.StartInfo.UseShellExecute = false;
            retrieveBIOSModeInfo.StartInfo.RedirectStandardOutput = true;
            retrieveBIOSModeInfo.StartInfo.FileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\WindowsPowerShell\v1.0\powershell.exe";
            retrieveBIOSModeInfo.StartInfo.Arguments = "$env:firmware_type";
            retrieveBIOSModeInfo.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            retrieveBIOSModeInfo.StartInfo.CreateNoWindow = true;
            retrieveBIOSModeInfo.StartInfo.Verb = "runas";
            retrieveBIOSModeInfo.Start();
            string result = retrieveBIOSModeInfo.StandardOutput.ReadToEnd();
            retrieveBIOSModeInfo.WaitForExit();

            switch (result.Trim())
            {
                case "UEFI":
                    biosMode.Content = "BIOS Mode: UEFI";
                    break;
                
                case "Legacy":
                    biosMode.Content = "BIOS Mode: Legacy";
                    break;
                
                default:
                    biosMode.Content = "BIOS Mode: Unknown";
                    break;
            }
        }

        public void RetrieveSecureBootInfo()
        {
            try
            {
                RegistryKey securebootstatuskey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SecureBoot\State");
                var securebootstatus = securebootstatuskey?.GetValue("UEFISecureBootEnabled") ?? 0;
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
        }

        public void RetrieveTPMInfo()
        {
            Process wmicTPMVersionProcess = new Process();
            wmicTPMVersionProcess.StartInfo.UseShellExecute = false;
            wmicTPMVersionProcess.StartInfo.RedirectStandardOutput = true;
            wmicTPMVersionProcess.StartInfo.FileName = Path.GetPathRoot(Environment.SystemDirectory) + @"\Windows\System32\wbem\wmic.exe";
            wmicTPMVersionProcess.StartInfo.Arguments = @"/namespace:\\root\CIMV2\Security\MicrosoftTpm path Win32_Tpm get /value";
            wmicTPMVersionProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            wmicTPMVersionProcess.StartInfo.CreateNoWindow = true;
            wmicTPMVersionProcess.StartInfo.Verb = "runas";
            wmicTPMVersionProcess.Start();
            string result = wmicTPMVersionProcess.StandardOutput.ReadToEnd();
            wmicTPMVersionProcess.WaitForExit();

            bool tpmEnabled = false;
            bool tpmActivated = false;
            bool tpmOwned = false;
            Version tpmVersion = null;

            if (result.Contains("IsEnabled_InitialValue=TRUE")) tpmEnabled = true;
            else if (result.Contains("IsActivated_InitialValue=TRUE")) tpmActivated = true;
            else if (result.Contains("IsOwned_InitialValue=TRUE")) tpmOwned = true;

            if (result.Contains("SpecVersion="))
            {
                int specVersionIndex = result.IndexOf("SpecVersion=");
                string line = result.Substring(specVersionIndex);
                tpmVersion = new Version(line.Replace("SpecVersion=", string.Empty).Split(',')[0].Trim());
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

        public void RetrieveMotherboardInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    motherboard.Content = "Motherboard: " + obj["Product"];
            }
            searcher.Dispose();
        }

        private void renamePCButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Will fix this later
            /*if (editionValue.Content.ToString().Contains("Windows 10") || editionValue.Content.ToString().Contains("Windows 11"))
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
            }
            else
            {
                Process.Start("sysdm.cpl");
            }*/
        }
    }
}
