using DictonaryProject.Repository.IRepository;
using DictonaryProject.Repository;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DictonaryProject
{
 
    public partial class DictionaryManagementScreen : Window
    {

        private readonly IDictionaryRepository _dictionaryRepository = new DictionariesRepository();
        public DictionaryManagementScreen()
        {
            InitializeComponent();
            LoadNotification();
            if(CurrentUser.LoggedInUser != null)
            {
                txtUsername.Text = CurrentUser.LoggedInUser.Username;

            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
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
                MessageBox.Show("Không tìm thấy kết quả nào.");
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

            var selectedItems = dgvDictionary.SelectedItems.Cast<dynamic>().ToList();

            if (!selectedItems.Any())
            {
                MessageBox.Show("Vui lòng chọn từ để xóa.", "No Word Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa từ này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                List<int> wordIds = selectedItems.Where(item => item.WordId != null).Select(items =>(int) items.WordId).ToList();

                bool isDeleted = _dictionaryRepository.DeleteWord(wordIds);
                if (isDeleted)
                {
                    MessageBox.Show("Xóa từ thành công", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ShowDictionaries();
                    LoadNotification();
                }
                else
                {
                    MessageBox.Show("Xóa từ không thành công", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                MessageBox.Show("Hãy chọn từ để sửa ", "No Word Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Từ đã được duyệt", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowDictionaries();
                LoadNotification();
            }
            else
            {
                MessageBox.Show("Duyệt từ không thành công!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.ClearUser();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryManagement management = new CategoryManagement();
            management.Show();
            this.Close();
        }
    }
}
