using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace SwimmingPool
{
    public partial class Sections
    {
        Entities entities = new Entities();
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public string sectionPhoto
        {
            get
            {
                BitmapImage stream = new BitmapImage();
                stream.BeginInit();
                stream.UriSource = new Uri(projectDirectory + this.Image);
                stream.EndInit();
                return stream.ToString();
            }
            set
            {

            }
        }

        public string CoachName
        {
            get
            {
                return (from name in entities.Coach where name.Id == this.Id_Coach select name).Single<Coach>().Surname + " " + (from name in entities.Coach where name.Id == this.Id_Coach select name).Single<Coach>().Name;
            }
            set
            {

            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public partial class Coach
    {
        public override string ToString()
        {
            return Surname + " " + Name;
        }
    }
    
    public partial class Request
    {
        Entities entities = new Entities();
        public override string ToString()
        {
            return Surname + " " + Name + " " + (from section in entities.Sections where section.Id == Id_Section select section).Single<Sections>().Name;
        }
    }
}
