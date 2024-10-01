using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
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

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for HardwareSpecificationsPage.xaml
    /// </summary>
    public partial class HardwareSpecificationsPage : Page
    {
        public HardwareSpecificationsPage()
        {
            InitializeComponent();
            // Computer Name
            compName.Content = Environment.MachineName;
        }

        public async void RetrieveCPUInfo()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                cpu.Content = "CPU: " + (string)mo["Name"];
            }
            await Task.Delay(200);
        }

        public async void RetrieveGPUInfo()
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
            await Task.Delay(200);
        }

        public async void RetrieveRAMInfo()
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
                        if (memoryType2 == "34")
                        {
                            newMemoryType = "DDR5";
                        }
                        else if (memoryType2 == "20")
                        {
                            newMemoryType = "DDR";
                        }
                        else if (memoryType2 == "21")
                        {
                            newMemoryType = "DDR2";
                        }
                        else if (memoryType2 == "24")
                        {
                            newMemoryType = "DDR3";
                        }
                        else if (memoryType2 == "26")
                        {
                            newMemoryType = "DDR4";
                        }
                        else
                        {
                            newMemoryType = "Unknown";
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
            await Task.Delay(200);
        }
    }
}
