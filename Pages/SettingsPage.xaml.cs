using iNKORE.UI.WPF.Modern;
using Microsoft.Win32;
using System.Windows;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        public SettingsPage()
        {
            InitializeComponent();
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
