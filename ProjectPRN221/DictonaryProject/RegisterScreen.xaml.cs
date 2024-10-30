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
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : Window
    {

        private readonly IUserRepository _userRepository = new UserRepository();
        public RegisterScreen()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            // Kiểm tra điều kiện nhập liệu
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khâu không trùng khớp với nhau.");
                return;
            }

            bool isExist = _userRepository.checkUserNameExist(username);
            if (isExist)
            {
                MessageBox.Show("Tên người dùng đã tồn tại trong hệ thống.");
                return;
            }

            bool isRegistered = _userRepository.RegisterUser(username, password);

            if (isRegistered)
            {
                MessageBox.Show("Đăng ký tài khoản thành công!");
                MainWindow loginScreen = new MainWindow();
                loginScreen.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Đăng ký tài khoản thất bại vui lòng thử lại!");
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow loginScreen = new MainWindow();
            loginScreen.Show();
            this.Close();
        }
    }
}
