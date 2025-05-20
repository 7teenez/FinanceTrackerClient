using FinanceTrackerClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FinanceTrackerClient
{
    public partial class MainWindow : Window
    {
        private int _userId = 1;
        private List<Category> _categories = new List<Category>();

        public MainWindow()
        {
            InitializeComponent();
            LoadCategories();
            LoadEntries();
        }

        private void LoadCategories()
        {
            _categories = Database.GetCategories();
            CategoryComboBox.ItemsSource = _categories;
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "ID";
        }

        private void LoadEntries()
        {
            List<Entry> entries = Database.GetEntries(_userId);
            List<EntryViewModel> entryViewModels = new List<EntryViewModel>();

            foreach (var entry in entries)
            {
                string categoryName = "Невідомо";
                var category = _categories.FirstOrDefault(c => c.CategoryID == entry.CategoryID);
                if (category != null)
                {
                    categoryName = category.Name;
                }

                entryViewModels.Add(new EntryViewModel
                {
                    Amount = entry.Amount,
                    CategoryName = categoryName,
                    Type = entry.Type,
                    Date = entry.Date.ToShortDateString(),
                    Note = entry.Note
                });
            }

            EntryListView.ItemsSource = entryViewModels;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Невірна сума.");
                return;
            }

            Category selectedCategory = CategoryComboBox.SelectedItem as Category;
            if (selectedCategory == null)
            {
                MessageBox.Show("Оберіть категорію.");
                return;
            }

            ComboBoxItem typeItem = TypeComboBox.SelectedItem as ComboBoxItem;
            if (typeItem == null)
            {
                MessageBox.Show("Оберіть тип.");
                return;
            }

            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Оберіть дату.");
                return;
            }

            DateTime selectedDate = DatePicker.SelectedDate.Value;

            Entry newEntry = new Entry
            {
                UserID = _userId,
                CategoryID = selectedCategory.CategoryID,
                Type = typeItem.Content.ToString(),
                Amount = amount,
                Date = selectedDate,
                Note = NoteTextBox.Text
            };

            Database.AddEntry(newEntry);
            LoadEntries();

            
            AmountTextBox.Text = "";
            NoteTextBox.Text = "";
            CategoryComboBox.SelectedIndex = -1;
            TypeComboBox.SelectedIndex = -1;
            DatePicker.SelectedDate = null;
        }

        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функція видалення ще не реалізована.");
        }
    }

    public class EntryViewModel
    {
        public decimal Amount { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
    }
}
