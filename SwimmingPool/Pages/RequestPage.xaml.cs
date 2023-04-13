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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace SwimmingPool.Pages
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        Users User;
        Sections req;
        Stopwatch time;
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        Entities entities = new Entities();
        public RequestPage(Sections section, Users user, Stopwatch time)
        {
            InitializeComponent();
            this.User = user;
            this.req = section;
            this.time = time;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(projectDirectory + req.Image);
            bitmap.EndInit();
            SectionsImage.Source = bitmap;

            BitmapImage stream1 = new BitmapImage();
            stream1.BeginInit();
            stream1.UriSource = new Uri(projectDirectory + (from name in entities.Coach where name.Id == req.Id_Coach select name).Single<Coach>().Image);
            stream1.EndInit();
            CoachImage.Source = stream1;

            VacancyName.Content = req.Name;
            CompanyName.Content = (from name in entities.Coach where name.Id == req.Id_Coach select name).Single<Coach>().Surname + " " + (from name in entities.Coach where name.Id == req.Id_Coach select name).Single<Coach>().Name;
            SalaryLabel.Content = req.Price + " " + "руб/мес.";
        }

        private void RequestBut_Click(object sender, RoutedEventArgs e)
        {
            Request request = new Request();
            entities.Request.Add(request);
            request.Id_User = User.Id;
            request.Id_Section = req.Id;
            request.Name = NameTB.Text;
            request.Surname = SurnameTB.Text;
            request.Phone_Number = PhoneTB.Text;
            request.Birthday = Convert.ToDateTime(BirthdayDP.SelectedDate);
            if (SexCB.SelectedIndex == 0)
            {
                request.Sex = "Мужчина";
            }
            else
            {
                request.Sex = "Женщина";
            }
            entities.SaveChanges();
            MessageBox.Show($"Вы успешно оставили заявку по вакансии!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new UserPage(User, time));
        }
    }
}
