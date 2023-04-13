using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;

namespace SwimmingPool.Pages
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        string imageUserExt;
        string SelectedUserImagePath;
        bool isNewUserImage = false;
        Entities entities = new Entities();
        private static Random random = new Random();
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void imageViewer(Image Img, string Path)
        {
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(projectDirectory + Path);
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            Img.Source = myBitmapImage;
        }

        public Registration()
        {
            InitializeComponent();
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(projectDirectory + "\\images\\Unknown.png");
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            UserImage.Source = myBitmapImage;
            RegistrationBut.IsEnabled = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/Authorization.xaml", UriKind.Relative));
        }

        private void RegistrationBut_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new Users();

            if (PassTB.Password != RePassTB.Password)
            {
                System.Windows.MessageBox.Show("Пароли не совпадают! Проверьте правильность введенных данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                newUser.Login = LoginTB.Text;
                newUser.Name = NameTB.Text;
                newUser.Password = Encrypt(PassTB.Password);
                if (isNewUserImage == true)
                {
                    string rndString = RandomString(16);
                    string newPath = projectDirectory + @"\images\" + rndString + "." + imageUserExt;
                    File.Copy(SelectedUserImagePath, newPath);
                    newUser.Image = @"\images\" + rndString + "." + imageUserExt;
                    isNewUserImage = false;
                }
                else
                {
                    newUser.Image = @"\images\Unknown.png";
                }
                entities.Users.Add(newUser);
                entities.SaveChanges();
                System.Windows.MessageBox.Show("Вы успешно зарегистрировались!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = projectDirectory + @"\images\";
            dlg.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog().ToString().Equals("OK"))
            {
                string selectedFileName = dlg.FileName;
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(selectedFileName);
                myBitmapImage.EndInit();

                UserImage.Source = myBitmapImage;
                SelectedUserImagePath = dlg.FileName;
                imageUserExt = dlg.SafeFileName.Split('.')[1];
                isNewUserImage = true;
            }
        }

        private void LoginTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoginTB.Text == "" || PassTB.Password == "" || RePassTB.Password == "" || NameTB.Text == "")
            {
                RegistrationBut.IsEnabled = false;
            }
            else
            {
                RegistrationBut.IsEnabled = true;
            }
        }

        private void PassTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (LoginTB.Text == "" || PassTB.Password == "" || RePassTB.Password == "" || NameTB.Text == "")
            {
                RegistrationBut.IsEnabled = false;
            }
            else
            {
                RegistrationBut.IsEnabled = true;
            }
        }

        private void RePassTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (LoginTB.Text == "" || PassTB.Password == "" || RePassTB.Password == "" || NameTB.Text == "")
            {
                RegistrationBut.IsEnabled = false;
            }
            else
            {
                RegistrationBut.IsEnabled = true;
            }
        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoginTB.Text == "" || PassTB.Password == "" || RePassTB.Password == "" || NameTB.Text == "")
            {
                RegistrationBut.IsEnabled = false;
            }
            else
            {
                RegistrationBut.IsEnabled = true;
            }
        }

        public static string Encrypt(string str)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            return Convert.ToBase64String(hash);
        }
    }
}
