using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

namespace SwimmingPool.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        Entities entities = new Entities();
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public void imageViewer(ImageBrush Img, string Path)
        {
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(projectDirectory + Path);
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            Img.ImageSource = myBitmapImage;
        }
        public Authorization()
        {
            InitializeComponent();
            
        }

        private void EnterBut_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTB.Text == "" || PassTB.Password == "")
            {
                MessageBox.Show("Логин или пароль не найдены. Проверьте правильность введенных данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {

                var profile = entities.Users.Where(x => x.Login == LoginTB.Text);
                if (profile.Count() == 1)
                {
                    Users user = profile.First();
                    if (user.Login == "Admin" && user.Password == PassTB.Password)
                    {
                        this.NavigationService.Navigate(new AdminPage());
                    }
                    else
                    {
                        if(user.Password == Decrypt(PassTB.Password))
                        {
                            Stopwatch time = new Stopwatch();
                            time.Start();
                            Logs log = new Logs();
                            entities.Logs.Add(log);
                            log.Id_User = user.Id;
                            log.Auth_Date = DateTime.Now;
                            entities.SaveChanges();
                            this.NavigationService.Navigate(new UserPage(user, time));
                            MainWindow SetWindow = Window.GetWindow(this) as MainWindow;
                            SetWindow.usernameLabel.Content = user.Name;
                            imageViewer(SetWindow.userImage, user.Image);
                        }
                        else
                        {
                            LoginTB.Visibility = Visibility.Hidden;
                            PassTB.Visibility = Visibility.Hidden;
                            EnterBut.Visibility = Visibility.Hidden;
                            RegBut.Visibility = Visibility.Hidden;
                            LoginLabel.Visibility = Visibility.Hidden;
                            PassLabel.Visibility = Visibility.Hidden;
                            TitleLabel.Content = "Вы не правильно ввели пароль. Введите капчу";
                            TitleLabel.FontSize = 17;
                            
                            CaptchaLabel.Visibility = Visibility.Visible;
                            CaptchaTB.Visibility = Visibility.Visible;
                            CaptchaButton.Visibility = Visibility.Visible;

                            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                            var stringChars = new char[5];
                            var random = new Random();

                            for (int i = 0; i < stringChars.Length; i++)
                            {
                                stringChars[i] = chars[random.Next(chars.Length)];
                            }

                            var finalString = new String(stringChars);
                            CaptchaLabel.Content = finalString;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Логин или пароль не найдены. Проверьте правильность введенных данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);


                }
            }
        }

        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/Registration.xaml", UriKind.Relative));
        }

        public static string Decrypt(string str)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            return Convert.ToBase64String(hash);
        }

        private void CaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaLabel.Content.ToString() == CaptchaTB.Text)
            {
                MessageBox.Show("Код введён верно!\nПопробуйте авторизоваться ещё раз через 10 секунд", "Успех", MessageBoxButton.OK);
                this.StartTimer();
                LoginTB.Visibility = Visibility.Visible;
                PassTB.Visibility = Visibility.Visible;
                EnterBut.Visibility = Visibility.Visible;
                RegBut.Visibility = Visibility.Visible;
                LoginLabel.Visibility = Visibility.Visible;
                PassLabel.Visibility = Visibility.Visible;
                TitleLabel.Content = "Войдите или зарегистрируйтесь";
                TitleLabel.FontSize = 26;
                CaptchaLabel.Visibility = Visibility.Hidden;
                CaptchaTB.Visibility = Visibility.Hidden;
                CaptchaButton.Visibility = Visibility.Hidden;
                CaptchaTB.Clear();
                EnterBut.IsEnabled = false;
            }
            else
            {
                CaptchaTB.Clear();
                MessageBox.Show("Код введён не верно!\nПопробуйте ещё раз!", "Успех", MessageBoxButton.OK);
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[5];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);
                CaptchaLabel.Content = finalString;
            }
        }

        private void StartTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            EnterBut.IsEnabled = true;
            
            MessageBox.Show("10 секунд прошли.Попробуйте ещё раз.", "Внимание!", MessageBoxButton.OK);
        }
    }
}
