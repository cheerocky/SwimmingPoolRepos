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
using System.Windows.Forms;

namespace SwimmingPool.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        Entities entities = new Entities();
        string imageVacancyExt;
        string SelectedVacancyImagePath;
        bool isNewVacancyImage = false;

        string imageCoachExt;
        string SelectedCoachImagePath;
        bool isNewCoachImage = false;
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
        public AdminPage()
        {
            InitializeComponent();
            foreach (var vacancy in entities.Sections)
            {
                VacancyLB.Items.Add(vacancy).ToString();
            }
            foreach (var company in entities.Coach)
            {
                TypeWorkCB.Items.Add(company).ToString();
                CompanyLB.Items.Add(company).ToString();
            }

            foreach (var request in entities.Request)
                RequestsLB.Items.Add(request).ToString();


            imageViewer(VacancyImage, "\\images\\Unknown.png");
            imageViewer(CoachImage, "\\images\\Unknown.png");

            foreach (var users in entities.Logs)
            {
                var query = (from data in entities.Logs

                             join user in entities.Users on data.Id_User equals user.Id
                             select new
                             {
                                 username = user.Login,
                                 Auth_Date = data.Auth_Date,
                                 Work_Time = data.Work_Time
                             }).Distinct().ToList();
                LogsDG.ItemsSource = query;


            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_item = VacancyLB.SelectedItem as Sections;
            if (selected_item != null)
            {
                NameTB.Text = selected_item.Name;
                SalaryTB.Text = selected_item.Price.ToString();
                TypeWorkCB.SelectedItem = (from type in entities.Coach where type.Id == selected_item.Id_Coach select type).Single<Coach>();

                imageViewer(VacancyImage, selected_item.Image);

                imageVacancyExt = null;
                SelectedVacancyImagePath = null;
                isNewVacancyImage = false;
            }
            else
            {
                NameTB.Text = "";
                SalaryTB.Text = "";
                TypeWorkCB.SelectedIndex = -1;
            }
        }

        private void LoadVacancy_Click(object sender, RoutedEventArgs e)
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

                VacancyImage.Source = myBitmapImage;
                SelectedVacancyImagePath = dlg.FileName;
                imageVacancyExt = dlg.SafeFileName.Split('.')[1];
                isNewVacancyImage = true;
            }
        }

        private void DeleteVacancy_Click(object sender, RoutedEventArgs e)
        {
            var rezult = System.Windows.MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rezult == MessageBoxResult.No)
            {
                return;
            }
            var delete_vacancy = VacancyLB.SelectedItem as Sections;
            if (delete_vacancy != null)
            {
                try
                {
                    var exist_ = (from section in entities.Request where section.Id_Section == delete_vacancy.Id select section).First();
                    System.Windows.MessageBox.Show("На секцию записаны люди!", "Ошибка!", MessageBoxButton.OK);
                }
                catch
                {
                    entities.Sections.Remove(delete_vacancy);
                    entities.SaveChanges();
                    NameTB.Text = "";
                    SalaryTB.Text = "";
                    TypeWorkCB.SelectedIndex = -1;
                    VacancyLB.Items.Remove(delete_vacancy);

                    BitmapImage myBitmapImage = new BitmapImage();
                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = new Uri(projectDirectory + "\\images\\Unknown.png");
                    myBitmapImage.DecodePixelWidth = 200;
                    myBitmapImage.EndInit();
                    VacancyImage.Source = myBitmapImage;
                }
                
            }
            else
            {
                System.Windows.MessageBox.Show("Нет удаляемых объектов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearVacancy_Click(object sender, RoutedEventArgs e)
        {
            NameTB.Text = "";
            SalaryTB.Text = "";
            TypeWorkCB.SelectedIndex = -1;
            VacancyLB.SelectedIndex = -1;

            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(projectDirectory + "\\images\\Unknown.png");
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            VacancyImage.Source = myBitmapImage;
        }

        private void SaveVacancy_Click(object sender, RoutedEventArgs e)
        {
            var save_vacancy = VacancyLB.SelectedItem as Sections;
            if (NameTB.Text == "" || SalaryTB.Text == "" || TypeWorkCB.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (save_vacancy == null)
                {
                    save_vacancy = new Sections();
                    entities.Sections.Add(save_vacancy);
                    VacancyLB.Items.Add(save_vacancy);
                }
                else
                {
                    isNewVacancyImage = true;
                }
                save_vacancy.Name = NameTB.Text;
                save_vacancy.Price = Convert.ToInt32(SalaryTB.Text);
                save_vacancy.Id_Coach = (TypeWorkCB.SelectedItem as Coach).Id;

                if (isNewVacancyImage == true)
                {
                    string rndString = RandomString(16);
                    string newPath = projectDirectory + @"\images\" + rndString + "." + imageVacancyExt;
                    File.Copy(SelectedVacancyImagePath, newPath);
                    save_vacancy.Image = @"\images\" + rndString + "." + imageVacancyExt;
                    isNewVacancyImage = false;
                }
                else
                {
                    save_vacancy.Image = @"\images\Unknown.png";
                }

                VacancyLB.Items.Refresh();
                entities.SaveChanges();
            }

        }

        private void CompanyLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_item = CompanyLB.SelectedItem as Coach;
            if (selected_item != null)
            {
                NameCompanyTB.Text = selected_item.Name;
                AdressTB.Text = selected_item.Surname;
                imageViewer(VacancyImage, selected_item.Image);
                CoachBirthdayDP.SelectedDate = selected_item.Birthday;
                imageVacancyExt = null;
                SelectedVacancyImagePath = null;
                isNewVacancyImage = false;
            }
            else
            {
                NameCompanyTB.Text = "";
                AdressTB.Text = "";
                CoachBirthdayDP.SelectedDate = null;

            }
        }

        private void DeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            var rezult = System.Windows.MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rezult == MessageBoxResult.No)
            {
                return;
            }
            var delete = CompanyLB.SelectedItem as Coach;
            if (delete != null)
            {
                try
                {
                    var exist_ = (from coach in entities.Sections where coach.Id_Coach == delete.Id select coach).First();
                    System.Windows.MessageBox.Show("Тренер прикреплен к секции!", "Ошибка!", MessageBoxButton.OK);
                }
                catch
                {
                    entities.Coach.Remove(delete);
                    entities.SaveChanges();
                    NameCompanyTB.Text = "";
                    AdressTB.Text = "";
                    CoachBirthdayDP.SelectedDate = null;
                    CompanyLB.Items.Remove(delete);
                }
                
            }
            else
            {
                System.Windows.MessageBox.Show("Нет удаляемых объектов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearCompany_Click(object sender, RoutedEventArgs e)
        {
            NameCompanyTB.Text = "";
            AdressTB.Text = "";
            CoachBirthdayDP.SelectedDate = null;
            CompanyLB.SelectedItem = -1;
        }

        private void SaveCompany_Click(object sender, RoutedEventArgs e)
        {
            var save = CompanyLB.SelectedItem as Coach;
            if (NameCompanyTB.Text == "" || AdressTB.Text == "" || CoachBirthdayDP.SelectedDate == null)
            {
                System.Windows.MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (save == null)
                {
                    save = new Coach();
                    entities.Coach.Add(save);
                    CompanyLB.Items.Add(save);
                }
                else
                {
                    isNewCoachImage = true;
                }

                save.Name = NameCompanyTB.Text;
                save.Surname = AdressTB.Text;
                save.Birthday = CoachBirthdayDP.SelectedDate;

                if (isNewCoachImage == true)
                {
                    string rndString = RandomString(16);
                    string newPath = projectDirectory + @"\images\" + rndString + "." + imageVacancyExt;
                    File.Copy(SelectedVacancyImagePath, newPath);
                    save.Image = @"\images\" + rndString + "." + imageVacancyExt;
                    isNewCoachImage = false;
                }
                else
                {
                    save.Image = @"\images\Unknown.png";
                }
                CompanyLB.Items.Refresh();
                entities.SaveChanges();
            }

        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/Authorization.xaml", UriKind.Relative));
        }

        private void RequestsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_item = RequestsLB.SelectedItem as Request;
            if (selected_item != null)
            {
                SurnameTB.Text = selected_item.Surname;
                NameRequestTB.Text = selected_item.Name;
                PhoneNumberTB.Text = selected_item.Phone_Number;
                BirthdayReqDP.SelectedDate = selected_item.Birthday;
                if (selected_item.Sex == "Мужчина")
                {
                    SexReqCB.SelectedIndex = 0;
                }
                else
                {
                    SexReqCB.SelectedIndex = 1;
                }
                NameVacancyReqTB.Text = (from name in entities.Sections where name.Id == selected_item.Id_Section select name).Single<Sections>().Name;
            }
            else
            {
                SurnameTB.Text = "";
                NameRequestTB.Text = "";
                LastnameTB.Text = "";
                PhoneNumberTB.Text = "";
                NameVacancyReqTB.Text = "";
                BirthdayReqDP.Text = null;
                SexReqCB.SelectedIndex = -1;
            }
        }
    }
}
