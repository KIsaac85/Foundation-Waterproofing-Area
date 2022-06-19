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

namespace Substructure_Area
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        private UIDocument uidoc;
        private Document doc;
        private Userselection userSelection;
        public UserControl1(UIDocument uidoc)
        {
            InitializeComponent();
            
            this.uidoc = uidoc;
            doc = uidoc.Document;
            userSelection = new Userselection(uidoc);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Elements_Selection_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            GeometryElement obj = userSelection.objec();
            SingleFootingCalculation footarea = new SingleFootingCalculation();
            footarea.faceinfor(obj, doc);
            Show();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
