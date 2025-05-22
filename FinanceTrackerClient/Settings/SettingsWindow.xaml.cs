using System.ComponentModel;
using System.Windows;
using MyApp.ViewModels;

namespace MyApp.Views
{
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveSettings();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            // Optional: prompt if unsaved changes exist
        }
    }
}
