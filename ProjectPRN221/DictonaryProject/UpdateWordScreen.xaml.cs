using DictonaryProject.Models;
using DictonaryProject.Repository;
using DictonaryProject.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DictonaryProject
{
    /// <summary>
    /// Interaction logic for UpdateWordScreen.xaml
    /// </summary>
    public partial class UpdateWordScreen : Window
    {

        private readonly IDictionaryRepository _dictionaryRepository = new DictionariesRepository();
        private int wordID;
        public UpdateWordScreen(int id)
        {
            InitializeComponent();
            wordID = id;
            ShowCategories();
            LoadData();

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentUser.LoggedInUser == null)
            {
                MessageBox.Show("You need to be logged in to add a new word.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Kiểm tra các trường có giá trị hợp lệ
            string englishWord = txtEnglishWord.Text;
            string type = cbTypeOfWord.Text;
            string pronunciation = txtPronunciation.Text;
            string englishMeaning = txtMeaningEnglish.Text;
            string vietnameseMeaning = txtMeaningVietnamese.Text;
            string exampleSentence = txtExampleSentence.Text;

            // Lấy danh sách tên danh mục từ ListBox
            List<string> categoryNames = lstCategories.SelectedItems
                                        .Cast<Category>()
                                        .Select(c => c.CategoryName)
                                        .ToList();

            // Kiểm tra xem các trường quan trọng đã được điền chưa
            if (string.IsNullOrEmpty(englishWord) || string.IsNullOrEmpty(englishMeaning) ||
                string.IsNullOrEmpty(vietnameseMeaning))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            // Gọi hàm UpdateWord để cập nhật từ
            bool isUpdated = _dictionaryRepository.UpdateWord(wordID, englishWord,type, categoryNames,
                                                              pronunciation, CurrentUser.LoggedInUser.UserId,
                                                              englishMeaning, vietnameseMeaning,
                                                              exampleSentence);

            if (isUpdated)
            {
                MessageBox.Show("Word updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close(); // Đóng form sau khi cập nhật thành công
            }
            else
            {
                MessageBox.Show("An error occurred while updating the word.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBackToDictionary_Click(object sender, RoutedEventArgs e)
        {
            DictionaryManagementScreen dictionaryManagementScreen = new DictionaryManagementScreen();
            dictionaryManagementScreen.Show();
            Close();
        }


        void LoadData()
        {
            var word = _dictionaryRepository.GetWordById(wordID);

            if (word != null)
            {
                txtEnglishWord.Text = word.EnglishWord;
                txtPronunciation.Text = word.Pronunciation;
                cbTypeOfWord.SelectedItem = word.TypeOfWord;

                var meaning = word.Meanings.FirstOrDefault();
                if (meaning != null)
                {
                    txtMeaningEnglish.Text = meaning.EnglishMeaning;
                    txtMeaningVietnamese.Text = meaning.VietnameseMeaning;
                    txtExampleSentence.Text = meaning.ExampleSentence;
                }

                foreach (var category in word.Categories)
                {
                    foreach (var item in lstCategories.Items)
                    {
                        if (item is Category cat && cat.CategoryName == category.CategoryName)
                        {
                            lstCategories.SelectedItems.Add(item);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy từ này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // Đóng form nếu không tìm thấy từ
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ShowCategories();
            if(CurrentUser.LoggedInUser != null)
            {
                txtUsername.Text = CurrentUser.LoggedInUser.Username;
            }
        }

        private void ShowCategories()
        {
            var categories = _dictionaryRepository.GetAllCategories();
            lstCategories.ItemsSource = categories;
            lstCategories.DisplayMemberPath = "CategoryName";

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.ClearUser();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
