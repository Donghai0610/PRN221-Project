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
    /// Interaction logic for CategoryManagement.xaml
    /// </summary>
    public partial class CategoryManagement : Window
    {
        private readonly ICategoryRepository categoryRepository = new CategoryRepository();
        private readonly IDictionaryRepository dictionaryRepository = new DictionariesRepository();


        public CategoryManagement()
        {
            InitializeComponent();
            LoadCategory();
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtCategoryName.Text))

            {

                string newCategoryName = txtCategoryName.Text.Trim();
                var existingCategory = categoryRepository.GetCategoryByName(newCategoryName);
                if (existingCategory != null)
                {
                    MessageBox.Show("Tên chủ đề đã tồn tại.");
                    return;
                }

                var category = categoryRepository.AddCategory(txtCategoryName.Text);
                if (category != null)
                {
                    MessageBox.Show("Thêm thành công chủ đề mới");
                    LoadCategory();
                }
                else
                {
                    MessageBox.Show("Lỗi khi thêm chủ đề mới");
                }
            }
            else
            {
                MessageBox.Show("Hãy nhập chủ đề bạn muốn thêm vào ô nhập");
            }
        }

        private void btnUpdateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (dgvCategory.SelectedItem is Category selectedCategory)
            {
                string newCategoryName = txtCategoryName.Text.Trim();

                // Kiểm tra nếu tên mới trống
                if (string.IsNullOrEmpty(newCategoryName))
                {
                    MessageBox.Show("Tên chủ đề không được để trống.");
                    return;
                }

                var existingCategory = categoryRepository.GetCategoryByName(newCategoryName);
                if (existingCategory != null && existingCategory.CategoryId != selectedCategory.CategoryId)
                {
                    MessageBox.Show("Tên chủ đề đã tồn tại.");
                    return;
                }

                bool isUpdated = categoryRepository.UpdateCategory(selectedCategory.CategoryId, newCategoryName);
                if (isUpdated)
                {
                    MessageBox.Show("Cập nhật chủ đề thành công.");
                    LoadCategory(); 
                }
                else
                {
                    MessageBox.Show("Cập nhật chủ đề thất bại.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn chủ đề cần cập nhật.");
            }
        }


        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.ClearUser();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var selectCategories = dgvCategory.SelectedItems.Cast<Category>().ToList();
            if (selectCategories.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn chủ đề cần xóa.");
                return;
            }
        List<int> categoryIds = selectCategories.Select(selectCategories => selectCategories.CategoryId).ToList();
            bool isDeleted = categoryRepository.DeleteCategory(categoryIds);
            if (isDeleted)
            {
                MessageBox.Show("Xóa chủ đề thành công.");
                LoadCategory();
            }
            else
            {
                MessageBox.Show("Xóa chủ đề thất bại.");
            }
        }

        private void btnSearchCategory_Click(object sender, RoutedEventArgs e)
        {
           var categories = categoryRepository.searchCategory(txtCategoryName.Text.Trim());
            if (categories != null && categories.Count > 0)
            {
                dgvCategory.ItemsSource = categories;
            }
            else
            {
                MessageBox.Show("Không tìm thấy kết quả nào.");
            }
        }
        void LoadCategory()
        {
            var categories = categoryRepository.GetAllCategories();
            if (categories != null && categories.Count > 0)
            {
                dgvCategory.ItemsSource = categories;
            }
        }

        private void dgvCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = dgvCategory.SelectedItem as dynamic;
            if (selectedCategory != null)
            {
                txtCategoryName.Text = selectedCategory.CategoryName;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            DictionaryManagementScreen dictionaryManagementScreen = new DictionaryManagementScreen();
            dictionaryManagementScreen.Show();
            this.Close();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
