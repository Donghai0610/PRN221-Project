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
    /// Interaction logic for DictionaryMainScreen.xaml
    /// </summary>
    public partial class DictionaryMainScreen : Window
    {
        private readonly IDictionaryRepository _dictionaryRepository = new DictionariesRepository();
        public DictionaryMainScreen()
        {
            InitializeComponent();
        }

        

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowCategories();
            ShowDictionaries();
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
            var dictionaries = _dictionaryRepository.dictionaries();
            dgvDictionary.ItemsSource = dictionaries;
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            var results = _dictionaryRepository.SearchByWordAndCategory(txtSearchWord.Text.Trim(), cboCategories.Text.Trim());
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

       

     

       

        private void btnAddWord_Click(object sender, RoutedEventArgs e)
        {
            AddNewWordScreen addNewWordScreen = new AddNewWordScreen();
            addNewWordScreen.Show();
            this.Close();
        }
    }
}
