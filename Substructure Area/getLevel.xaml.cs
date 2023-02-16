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
        #region Document Members
        private UIDocument _uidoc { get; set; }
        private Document doc { get; set; }
        #endregion


        #region Members
        public static double Userinput { get; set; }
        public areaCalculation areaCalculationWindow { get; set; } 
        #endregion

        /// <summary>
        /// Constructor to initialize objects
        /// </summary>
        /// <param name="uidoc"></param>
        public getLevel(UIDocument uidoc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            doc = uidoc.Document;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Userinput = double.Parse(Receive_Level.Text);
            Close();
            areaCalculationWindow = new areaCalculation(_uidoc);
            areaCalculationWindow.Show();

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }
}
