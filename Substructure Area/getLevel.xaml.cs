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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Substructure_Area._2_DataFilter;

namespace Substructure_Area
{
    /// <summary>
    /// Interaction logic for getLevel.xaml
    /// </summary>
    public partial class getLevel : Window
    {
        private UIDocument _uidoc;
        private Document doc;

        public static double Userinput;
        public List<double> facesdata;
        public FoundationSurfaceAreas foot;


        public getLevel(UIDocument uidoc)


        {
            InitializeComponent();
            
            facesdata = new List<double>();
            _uidoc = uidoc;
            doc = uidoc.Document;

        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Userinput = double.Parse(Receive_Level.Text);
            Close();
            areaCalculation are = new areaCalculation(_uidoc);

            are.Show();

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }
}
