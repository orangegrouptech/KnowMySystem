using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace KnowMySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            if (version.Foreground == Brushes.Red)
            {
                license.Content = "Orange Group Confidential";
                internalBuildText.Visibility = Visibility.Visible;
                internalBuildText2.Visibility = Visibility.Visible;
                internalBuildText3.Visibility = Visibility.Visible;
                internalBuildText4.Visibility = Visibility.Visible;
                credits.Visibility = Visibility.Collapsed;
                creditTesters.Visibility = Visibility.Collapsed;
            }
            else { }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
