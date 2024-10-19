using DictonaryProject.Repository;
using DictonaryProject.Repository.IRepository;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DictonaryProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IUserRepository _userRepository = new UserRepository();
        public MainWindow()
        {
            InitializeComponent();
            LoadRememberedUser();
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

        private void Register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterScreen registerScreen = new RegisterScreen();
            registerScreen.Show();
           this. Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Password.Trim();
            if (chkRememberMe.IsChecked == true)
            {
                SaveUserCredentials(username, password);
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password are required.");
                return;
            }
            var user = _userRepository.Login(username, password);
            if (user == null)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }
            MessageBox.Show("Login successful.");


        }

        private void chkRememberMe_Checked(object sender, RoutedEventArgs e)
        {
            SaveUserCredentials(txtUser.Text, txtPass.Password);
        }

        private void SaveUserCredentials(string username, string password)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("D:\\Learn_FPT\\Semeter_7\\PRN221\\Project\\PRN221-Project\\ProjectPRN221\\DictonaryProject\\SettingSave.txt"))
                {
                    writer.WriteLine(username);
                    writer.WriteLine(password);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving credentials: {ex.Message}");
            }
        }
        private void ClearUserCredentials()
        {
            try
            {
                if (File.Exists("D:\\Learn_FPT\\Semeter_7\\PRN221\\Project\\PRN221-Project\\ProjectPRN221\\DictonaryProject\\SettingSave.txt"))
                {
                    File.Delete("D:\\Learn_FPT\\Semeter_7\\PRN221\\Project\\PRN221-Project\\ProjectPRN221\\DictonaryProject\\SettingSave.txt");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing credentials: {ex.Message}");
            }
        }
        private void LoadRememberedUser()
        {
            try
            {
                if (File.Exists("D:\\Learn_FPT\\Semeter_7\\PRN221\\Project\\PRN221-Project\\ProjectPRN221\\DictonaryProject\\SettingSave.txt"))
                {
                    string[] lines = File.ReadAllLines("D:\\Learn_FPT\\Semeter_7\\PRN221\\Project\\PRN221-Project\\ProjectPRN221\\DictonaryProject\\SettingSave.txt");
                    if (lines.Length >= 2)
                    {
                        txtUser.Text = lines[0];
                        txtPass.Password = lines[1];
                        chkRememberMe.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading remembered user: {ex.Message}");
            }
        }
    }
}