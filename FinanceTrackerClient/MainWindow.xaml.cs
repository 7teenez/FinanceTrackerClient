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

            foreach (var entry in entries)
            {
                var category = _categories.FirstOrDefault(c => c.CategoryID == entry.CategoryID);
                if (category != null)
                {
                    entry.Note = $"[{category.Name}] {entry.Note}";
                }
            }
            EntryListView.ItemsSource = entries;
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
            Entry selectedEntry = EntryListView.SelectedItem as Entry;
            if (selectedEntry == null)
            {
                MessageBox.Show("Оберіть запис для видалення.");
                return;
            }
            var result = MessageBox.Show("Ви впевнені, що хочете видалити цей запис?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Database.DeleteEntry(selectedEntry.EntryID);
                LoadEntries();
            }
        }
    }
}