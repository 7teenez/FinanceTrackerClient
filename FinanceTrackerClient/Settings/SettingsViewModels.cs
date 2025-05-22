using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FinanceTrackerClient.Settings
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
                // Save to settings or apply currency format logic
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
            // Initialize themes
            AvailableThemes = new ObservableCollection<string> { "Light", "Dark" };
            SelectedTheme = LoadThemeFromSettings();
            // Initialize currencies
            AvailableCurrencies = new ObservableCollection<string> { "USD", "EUR", "UAH" };
            SelectedCurrency = LoadCurrencyFromSettings();
            // Initialize categories
            Categories = new ObservableCollection<string>(LoadCategoriesFromSettings());
            AddCategoryCommand = new RelayCommand(_ => AddCategory(), _ => !string.IsNullOrWhiteSpace(NewCategoryName));
            RemoveCategoryCommand = new RelayCommand(_ => RemoveCategory(), _ => CanRemoveCategory);
        }

        private void AddCategory()
        {
            if (!Categories.Contains(NewCategoryName))
            {
                Categories.Add(NewCategoryName);
                SaveCategories();
                NewCategoryName = string.Empty;
            }
        }

        private void RemoveCategory()
        {
            if (CanRemoveCategory)
            {
                Categories.Remove(SelectedCategory);
                SaveCategories();
            }
        }

        private void ApplyTheme(string theme)
        {
            var dict = new ResourceDictionary();
            var themeFile = theme == "Dark" ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
            dict.Source = new Uri($"pack://application:,,,/{themeFile}", UriKind.Absolute);
            // Remove existing theme dictionaries
            var existing = Application.Current.Resources.MergedDictionaries
                            .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"));
            if (existing != null)
                Application.Current.Resources.MergedDictionaries.Remove(existing);

            Application.Current.Resources.MergedDictionaries.Add(dict);
            SaveThemeToSettings(theme);
        }

        private string LoadThemeFromSettings()
        {
            // TODO: Read from persistent storage
            return "Light";
        }
        private void SaveThemeToSettings(string theme)
        {
            // TODO: Write to persistent storage
        }

        private string LoadCurrencyFromSettings()
        {
            // TODO: Read from persistent storage
            return "USD";
        }

        private void SaveCurrencyToSettings()
        {
            // TODO: Write to persistent storage
        }

        private string[] LoadCategoriesFromSettings()
        {
            // TODO: Read from persistent storage
            return new[] { "General", "Work", "Personal" };
        }

        private void SaveCategories()
        {
            // TODO: Write to persistent storage
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Simple RelayCommand implementation
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