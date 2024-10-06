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

namespace KnowMySystem.Pages.Hardware_Specifications_Pages
{
    /// <summary>
    /// Interaction logic for CPUPage.xaml
    /// </summary>
    public partial class CPUPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        public CPUPage()
        {
            InitializeComponent();

            // CPU Name
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                cpuName.Content = (string)mo["Name"];
            }

            // CPU Architecture
            string architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
            switch (architecture)
            {
                case "X86":
                    architecture = "i386 (32-bit)";
                    break;
                
                case "X64":
                    architecture = "AMD64 (64-bit)";
                    break;

                case "ARM":
                    architecture = "ARM32 (32-bit)";
                    break;

                case "ARM64":
                    architecture = "ARM64 (64-bit)";
                    break;

                default:
                    architecture = "Unknown";
                    break;
            }
            cpuArchitecture.Content = architecture;

            // CPU Cores and Threads
            ManagementObjectSearcher coreSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject managementObject in coreSearcher.Get())
            {
                cpuCores.Content = managementObject["NumberOfCores"];
                cpuThreads.Content = managementObject["NumberOfLogicalProcessors"];
            }

            // CPU Clock Speed
            ManagementObjectSearcher clockSpeedSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject managementObject in clockSpeedSearcher.Get())
            {
                cpuBaseClock.Content = managementObject["MaxClockSpeed"];
                cpuBoostClock.Content = managementObject["CurrentClockSpeed"];
            }

        }
    }
}
