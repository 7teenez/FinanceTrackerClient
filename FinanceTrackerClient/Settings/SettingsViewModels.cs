using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MyApp.Helpers;

namespace MyApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        // Themes
        public ObservableCollection<string> AvailableThemes { get; }
        private string _selectedTheme;
        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme == value) return;
                _selectedTheme = value;
                OnPropertyChanged();
                ApplyTheme(_selectedTheme);
            }
        }

        // Currencies
        public ObservableCollection<string> AvailableCurrencies { get; }
        private string _selectedCurrency;
        public string SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                if (_selectedCurrency == value) return;
                _selectedCurrency = value;
                OnPropertyChanged();
            }
        }

        // Categories
        public ObservableCollection<string> Categories { get; }
        private string _newCategoryName;
        public string NewCategoryName
        {
            get => _newCategoryName;
            set { _newCategoryName = value; OnPropertyChanged(); }
        }
        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanRemoveCategory)); }
        }
        public bool CanRemoveCategory => !string.IsNullOrEmpty(SelectedCategory);

        public ICommand AddCategoryCommand { get; }
        public ICommand RemoveCategoryCommand { get; }

        public SettingsViewModel()
        {
            // Initialize
            AvailableThemes = new ObservableCollection<string> { "Light", "Dark" };
            AvailableCurrencies = new ObservableCollection<string> { "USD", "EUR", "UAH" };
            Categories = new ObservableCollection<string>(LoadCategoriesFromSettings());

            // Load persisted settings
            SelectedTheme = LoadThemeFromSettings();
            SelectedCurrency = LoadCurrencyFromSettings();

            AddCategoryCommand = new RelayCommand(_ => AddCategory(), _ => !string.IsNullOrWhiteSpace(NewCategoryName));
            RemoveCategoryCommand = new RelayCommand(_ => RemoveCategory(), _ => CanRemoveCategory);
        }

        public void SaveSettings()
        {
            SaveThemeToSettings(SelectedTheme);
            SaveCurrencyToSettings(SelectedCurrency);
            SaveCategories();
        }

        private void AddCategory()
        {
            if (!Categories.Contains(NewCategoryName))
            {
                Categories.Add(NewCategoryName);
                NewCategoryName = string.Empty;
            }
        }

        private void RemoveCategory()
        {
            if (CanRemoveCategory)
                Categories.Remove(SelectedCategory);
        }

        private void ApplyTheme(string theme)
        {
            var dict = new ResourceDictionary();
            var themeFile = theme == "Dark" ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
            dict.Source = new Uri($"pack://application:,,,/{themeFile}", UriKind.Absolute);

            var existing = Application.Current.Resources.MergedDictionaries
                                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"));
            if (existing != null)
                Application.Current.Resources.MergedDictionaries.Remove(existing);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        #region Persistence (using Properties.Settings)
        private string LoadThemeFromSettings() => Properties.Settings.Default.AppTheme;
        private void SaveThemeToSettings(string theme)
        {
            Properties.Settings.Default.AppTheme = theme;
            Properties.Settings.Default.Save();
        }

        private string LoadCurrencyFromSettings() => Properties.Settings.Default.AppCurrency;
        private void SaveCurrencyToSettings(string currency)
        {
            Properties.Settings.Default.AppCurrency = currency;
            Properties.Settings.Default.Save();
        }

        private string[] LoadCategoriesFromSettings()
        {
            var data = Properties.Settings.Default.UserCategories;
            return string.IsNullOrEmpty(data)
                ? new[] { "General", "Work", "Personal" }
                : data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void SaveCategories()
        {
            Properties.Settings.Default.UserCategories = string.Join(";", Categories);
            Properties.Settings.Default.Save();
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    // RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
