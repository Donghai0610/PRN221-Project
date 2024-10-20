using DictonaryProject.Models;
using DictonaryProject.Repository;
using DictonaryProject.Repository.IRepository;
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

namespace DictonaryProject
{
    /// <summary>
    /// Interaction logic for AddNewWordScreen.xaml
    /// </summary>
    public partial class AddNewWordScreen : Window
    {
        private readonly IDictionaryRepository _dictionaryRepository = new DictionariesRepository();
        private readonly IUserRepository _userRepository = new UserRepository();

        public AddNewWordScreen()
        {
            InitializeComponent();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.LoggedInUser == null)
            {
                MessageBox.Show("You need to be logged in to add a new word.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string englishWord = txtEnglishWord.Text;
            string pronunciation = txtPronunciation.Text;
            string englishMeaning = txtMeaningEnglish.Text;
            string vietnameseMeaning = txtMeaningVietnamese.Text;
            string exampleSentence = txtExampleSentence.Text;
            int createdByUserId = CurrentUser.LoggedInUser.UserId; // Sử dụng UserID từ người dùng hiện tại
            bool isUser = CurrentUser.LoggedInUser.Roles.Any(r => r.RoleName == "User");

            // Lấy danh sách tên danh mục từ ListBox
            List<string> categoryNames = lstCategories.SelectedItems.Cast<Category>()
                                        .Select(c => c.CategoryName).ToList();
            bool addCategoryIfNotExist = false;

            if (!string.IsNullOrEmpty(txtNewCategory.Text))
            {
                addCategoryIfNotExist = MessageBox.Show("Bạn có muốn thêm danh mục mới nếu chưa có?",
                                                        "Xác nhận", MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question) == MessageBoxResult.Yes;

                if (addCategoryIfNotExist && !categoryNames.Contains(txtNewCategory.Text))
                {
                    categoryNames.Add(txtNewCategory.Text);
                }
            }



            // Thực hiện thêm từ mới
            bool isAdded = _dictionaryRepository.AddNewWord(englishWord, categoryNames, pronunciation, createdByUserId,
                                      englishMeaning, vietnameseMeaning, exampleSentence,
                                      isUser, addCategoryIfNotExist);

            if (isAdded)
            {
                MessageBox.Show("Từ đã được thêm thành công.", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnBackToDictionary_Click(object sender, RoutedEventArgs e)
        {
            bool isUser = CurrentUser.LoggedInUser.Roles.Any(r => r.RoleName == "User");
            if (isUser)
            {
                DictionaryMainScreen dictionaryMainScreen = new DictionaryMainScreen();
                dictionaryMainScreen.Show();
                this.Close();
            }
            else
            {
                DictionaryManagementScreen dictionaryManagementScreen = new DictionaryManagementScreen();
                dictionaryManagementScreen.Show();
                this.Close();
            }
           
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void ShowCategories()
        {
            var categories = _dictionaryRepository.GetAllCategories();
            lstCategories.ItemsSource = categories;
            lstCategories.SelectionChanged += lstCategories_SelectionChanged;
            lstCategories.DisplayMemberPath = "CategoryName";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowCategories();
        }
    }
}
