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
                MessageBox.Show("Bạn cần phải đăng nhập để thêm từ mới vào!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string englishWord = txtEnglishWord.Text;
            string type = cbTypeOfWord.Text;
            string pronunciation = txtPronunciation.Text;
            string englishMeaning = txtMeaningEnglish.Text;
            string vietnameseMeaning = txtMeaningVietnamese.Text;
            string exampleSentence = txtExampleSentence.Text;
            int createdByUserId = CurrentUser.LoggedInUser.UserId; 
            bool isUser = CurrentUser.LoggedInUser.Roles.Any(r => r.RoleName == "User");

          
            List<string> categoryNames = lstCategories.SelectedItems.Cast<Category>()
                                        .Select(c => c.CategoryName).ToList();

           



            // Thực hiện thêm từ mới
            bool isAdded = _dictionaryRepository.AddNewWord(englishWord,type, categoryNames, pronunciation, createdByUserId,
                                      englishMeaning, vietnameseMeaning, exampleSentence,
                                      isUser);

            if (isAdded)
            {
                if (isUser)
                {
                    MessageBox.Show("Từ đã được thêm thành công.Hãy đợi quản trị viên phê duyệt từ của bạn mới thêm vào", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    DictionaryMainScreen dictionaryMainScreen = new DictionaryMainScreen();
                    dictionaryMainScreen.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Từ đã được thêm thành công.", "Thông báo",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DictionaryManagementScreen dictionaryManagementScreen = new DictionaryManagementScreen();
                    dictionaryManagementScreen.Show();
                    this.Close();
                }
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
            if(CurrentUser.LoggedInUser != null)
            {
                txtUsername.Text = CurrentUser.LoggedInUser.Username;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.ClearUser();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
