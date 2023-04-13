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
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        Users User;
        Entities entities = new Entities();
        int count;
        Stopwatch time;
        Logs log = null;
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public UserPage(Users user, Stopwatch time)
        {
            InitializeComponent();
            this.User = user;
            this.time = time;
            SectionLBV.ItemsSource = Entities.GetContext().Sections.ToList();
            PriceCB.SelectedIndex = 0;
            SortCB.SelectedIndex = 0;
            count = SectionLBV.Items.Count;

            LabelCount.Content = $"Секций: {SectionLBV.Items.Count} из {count}";
        }

        private void SectionLBV_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var section = SectionLBV.SelectedItem as Sections;
            var client = User;
            this.NavigationService.Navigate(new RequestPage(section, client, time));
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SectionLBV.ItemsSource = null;
            SectionLBV.ItemsSource = Entities.GetContext().Sections.ToList();
            var selectedSalary = PriceCB.SelectedItem as Section;
            if (SearchTB.Text != null && PriceCB.SelectedIndex == 0)
            {
                for (int i = 0; i < SectionLBV.Items.Count; i++)
                {
                    if (!Entities.GetContext().Sections.ToString().ToLower().Contains(SearchTB.Text.ToLower()))
                    {
                        SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower())).ToList();
                    }
                }
            }
            else if (SearchTB.Text != null && PriceCB.SelectedIndex != 0)
            {
                for (int i = 0; i < SectionLBV.Items.Count; i++)
                {
                    if (!Entities.GetContext().Sections.ToString().ToLower().Contains(SearchTB.Text.ToLower()))
                    {
                        if (PriceCB.SelectedIndex == -1)
                        {
                            return;
                        }
                        else if (PriceCB.SelectedIndex == 0)
                        {
                            SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower())).ToList();
                        }
                        else
                        {
                            switch (PriceCB.SelectedIndex)
                            {
                                case 0:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower()) && r.Price == 0).ToList();
                                    break;
                                case 1:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower()) && r.Price > 0 && r.Price <= 1000).ToList();
                                    break;
                                case 2:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower()) && r.Price > 1000 && r.Price <= 2000).ToList();
                                    break;
                                case 3:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower()) && r.Price > 2000 && r.Price <= 5000).ToList();
                                    break;
                                case 4:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower()) && r.Price > 5000).ToList();
                                    break;
                            }
                        }
                    }
                }
            }
            LabelCount.Content = $"Секций: {SectionLBV.Items.Count} из {count}";
        }

        private void SalaryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SectionLBV.ItemsSource = null;
            SectionLBV.ItemsSource = Entities.GetContext().Sections.ToList();
            var selectedSalary = PriceCB.SelectedItem as Sections;
            for (int i = 0; i < SectionLBV.Items.Count; i++)
            {
                if (!Entities.GetContext().Sections.ToString().ToLower().Contains(PriceCB.Text.ToLower()))
                {
                    if (PriceCB.SelectedIndex == -1)
                    {
                        return;
                    }
                    else if (PriceCB.SelectedIndex == 0)
                    {
                        //VacancyLBV.ItemsSource = Entities.GetContext().Vacancy.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower())).ToList();
                        return;
                    }
                    else
                    {
                        switch (PriceCB.SelectedIndex)
                        {
                            case 0:
                                SectionLBV.ItemsSource = Entities.GetContext().Sections.ToList();
                                break;
                            case 1:
                                SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Price > 0 && r.Price <= 10000).ToList();
                                break;
                            case 2:
                                SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Price > 10000 && r.Price <= 20000).ToList();
                                break;
                            case 3:
                                SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Price > 20000 && r.Price <= 50000).ToList();
                                break;
                            case 4:
                                SectionLBV.ItemsSource = Entities.GetContext().Sections.Where(r => r.Price > 50000).ToList();
                                break;
                        }
                    }
                }
            }
            LabelCount.Content = $"Секций: {SectionLBV.Items.Count} из {count}";
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SectionLBV.ItemsSource = null;
            SectionLBV.ItemsSource = Entities.GetContext().Sections.ToList();
            for (int i = 0; i < SectionLBV.Items.Count; i++)
            {
                if (!Entities.GetContext().Sections.ToString().ToLower().Contains(SortCB.Text.ToLower()))
                {
                    {
                        if (SortCB.SelectedIndex == -1)
                        {
                            return;
                        }
                        else if (SortCB.SelectedIndex == 0)
                        {
                            //VacancyLBV.ItemsSource = Entities.GetContext().Vacancy.Where(r => r.Name.ToLower().Contains(SearchTB.Text.ToLower())).ToList();
                            return;
                        }
                        else
                        {
                            switch (SortCB.SelectedIndex)
                            {
                                case 0:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.ToList();
                                    break;
                                case 1:
                                    SectionLBV.ItemsSource = Entities.GetContext().Sections.OrderByDescending(r => r.Price).ToList();
                                    break;
                                case 2:
                                    SectionLBV.ItemsSource = (from p in entities.Sections orderby p.Price ascending select p).ToList();
                                    break;
                            }
                        }
                    }
                }
            }
            LabelCount.Content = $"Секций: {SectionLBV.Items.Count} из {count}";
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            PriceCB.SelectedIndex = 0;
            SortCB.SelectedIndex = 0;
            SearchTB.Clear();
            LabelCount.Content = $"Секций: {SectionLBV.Items.Count} из {count}";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/Authorization.xaml", UriKind.Relative));
            MainWindow SetWindow = Window.GetWindow(this) as MainWindow;
            SetWindow.usernameLabel.Content = "";
            SetWindow.userImage.ImageSource = null;
            time.Stop();

            using (var db = new Entities())
            {
                var result = db.Logs.Where(p => p.Id_User == User.Id).OrderByDescending(p => p.Id).FirstOrDefault();
                if (result != null)
                {
                    result.Work_Time = time.Elapsed;
                    db.SaveChanges();
                }
            }
            entities.SaveChanges();
        }

    }
}
