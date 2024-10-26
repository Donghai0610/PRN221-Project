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
    /// Interaction logic for WordDetail.xaml
    /// </summary>
    public partial class WordDetail : Window
    {

        private readonly IDictionaryRepository _dictionaryRepository = new DictionariesRepository();
        private int wordid;
        public WordDetail(int wordID)
        {
            InitializeComponent();
            wordid = wordID;
            LoadData();
            //ShowCategories();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        void LoadData()
        {

            if (CurrentUser.LoggedInUser != null)
            {
                txtUsername.Text = CurrentUser.LoggedInUser.Username; // Giả sử User có thuộc tính Username
            }
            var word = _dictionaryRepository.GetWordById(wordid);

            if (word != null)
            {
                txtEnglishWord.Text = word.EnglishWord;
                txtPronunciation.Text = word.Pronunciation;

                // Load meanings
                var meaning = word.Meanings.FirstOrDefault();
                if (meaning != null)
                {
                    txtMeaningEnglish.Text = meaning.EnglishMeaning;
                    txtMeaningVietnamese.Text = meaning.VietnameseMeaning;
                    txtExampleSentence.Text = meaning.ExampleSentence;
                }

                // Load categories as a comma-separated string
                txtCategories.Text = string.Join(", ", word.Categories.Select(c => c.CategoryName));

            }
            else
            {
                MessageBox.Show("Không tìm thấy từ này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // Close the form if the word is not found
            }
        }

        //private void ShowCategories()
        //{
        //    var categories = _dictionaryRepository.GetAllCategories();
        //    tẽ.ItemsSource = categories;
        //    lstCategories.DisplayMemberPath = "CategoryName";

        //}
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            
                
                CurrentUser.ClearUser();
                var loginWindow = new MainWindow(); 
                loginWindow.Show();
                this.Close(); 
            
        }

        private void btnBackToDictionary_Click(object sender, RoutedEventArgs e)
        {
            DictionaryMainScreen dictionaryMainScreen = new DictionaryMainScreen();
            dictionaryMainScreen.Show();
            Close();
        }

    }
}
