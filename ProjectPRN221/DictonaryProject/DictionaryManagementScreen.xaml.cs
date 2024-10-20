using DictonaryProject.Repository.IRepository;
using DictonaryProject.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DictonaryProject.Models;

namespace DictonaryProject
{
    /// <summary>
    /// Interaction logic for DictionaryManagementScreen.xaml
    /// </summary>
    public partial class DictionaryManagementScreen : Window
    {

        private readonly IDictionaryRepository _dictionaryRepository = new DictionariesRepository();
        public DictionaryManagementScreen()
        {
            InitializeComponent();
            LoadNotification();
        }

        private void btnAddWord_Click(object sender, RoutedEventArgs e)
        {
            AddNewWordScreen addNewWordScreen = new AddNewWordScreen();
            addNewWordScreen.Show();
            this.Close();
        }

        void LoadNotification()
        {
            int unapprovedWordCount = _dictionaryRepository.GetUnapprovedWordCount();
            txtUnapprovedWordCount.Text = $"Unapproved words: {unapprovedWordCount}";
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void cboCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            var results = _dictionaryRepository.SearchByWordAndCategoryForAdmin(txtSearchWord.Text.Trim(), cboCategories.Text.Trim());
            if (results != null && results.Count > 0)
            {
                dgvDictionary.ItemsSource = results;
            }
            else
            {
                MessageBox.Show("No results found.");
                dgvDictionary.ItemsSource = null;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearchWord.Text = string.Empty;
            cboCategories.Text = string.Empty;
            ShowDictionaries();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            if (dgvDictionary.SelectedItem != null)
            {
                dynamic selectedItem = dgvDictionary.SelectedItem;

                int wordId = selectedItem.WordId;

                var result = MessageBox.Show("Are you sure you want to delete this word?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool isDeleted = _dictionaryRepository.DeleteWord(wordId);
                    if (isDeleted)
                    {
                        MessageBox.Show("Word deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ShowDictionaries();
                        LoadNotification();
                    }
                    else
                    {
                        MessageBox.Show("Word deletion failed.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a word to delete.", "No Word Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowCategories()
        {
            var category = _dictionaryRepository.GetAllCategories();
            cboCategories.ItemsSource = category;
            cboCategories.DisplayMemberPath = "CategoryName";
            cboCategories.SelectedValuePath = "CategoryId";

        }

        private void ShowDictionaries()
        {
            var dictionaries = _dictionaryRepository.GetAllWordsOfAdmin();
            dgvDictionary.ItemsSource = dictionaries;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowCategories();
            ShowDictionaries();

        }

        private void dgvDictionary_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgvDictionary.SelectedItem != null)
            {
                dynamic selectedItem = dgvDictionary.SelectedItem;

                int wordId = selectedItem.WordId;

                UpdateWordScreen updateWordScreen = new UpdateWordScreen(wordId);
                updateWordScreen.Closed += (s, args) => ShowDictionaries(); // Gắn sự kiện Closed
                updateWordScreen.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a word to update.", "No Word Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgvDictionary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDictionary.SelectedItem != null)
            {
                dynamic selectedItem = dgvDictionary.SelectedItem;
                string isApproved = selectedItem.IsApproved;
                if (isApproved == "Đợi duyệt") // Nếu từ chưa được duyệt
                {
                    // Hiển thị form xác nhận
                    var result = MessageBox.Show("Từ này chưa được duyệt. Bạn có muốn phê duyệt không?",
                                                 "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Gọi hàm phê duyệt từ
                        ApprovedWord(selectedItem.WordId);
                    }
                }
            }
        }

        void ApprovedWord(int wordID)
        {
            bool isApproved = _dictionaryRepository.ApproveWord(wordID);
            if (isApproved)
            {
                MessageBox.Show("Word approved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowDictionaries();
                LoadNotification();
            }
            else
            {
                MessageBox.Show("Word approval failed.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
