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
using Autodesk.Revit.UI.Selection;
using Substructure_Area._2_DataFilter;
using Substructure_Area._3_Calculation;

namespace Substructure_Area
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class areaCalculation : Window
    {
        public double UserInputLevel { get; set; }
        private UIDocument _uidoc;
        private Document doc;
        private Userselection userSelection;
        private Element ele;
        private Reference obj;
        public static double Userinput;
        public List<double> facesdata;
        public SingleFootingCalculation foot;
        private ColumnBeamCalculation col { get; set; }
        public List<Element> FamilyinstanceList { get; set; }
        public IList<Element> wallsList { get; set; }
        public IList<Element> columnsList { get; set; }
        public IList<Element> beamsList { get; set; }
        public IList<Element> raftList { get; set; }
        public IList<Element> recFootingsList { get; set; }
        public IList<Element> stripFootingsList { get; set; }
        private static SelectionFilter SingleSelectionFilter;
        public areaCalculation(UIDocument uidoc)
        {
            InitializeComponent();

            this.UserInputLevel = UserInputLevel;
            SingleSelectionFilter = new SelectionFilter();
            facesdata = new List<double>();
            _uidoc = uidoc;
            doc = uidoc.Document;
            userSelection = new Userselection(uidoc);
            foot = new SingleFootingCalculation();
            col = new ColumnBeamCalculation();

            FamilyinstanceList = new List<Element>();
            wallsList = new List<Element>();
            raftList = new List<Element>();
            recFootingsList = new List<Element>();
            beamsList = new List<Element>();
            stripFootingsList = new List<Element>();
            columnsList = new List<Element>();
            FamilyInstanceList famlist = new FamilyInstanceList(doc);

            wallsList = FamilyInstanceList.documentLoopWall();
            if (wallsList.Any())
            {
                Select_Family.Items.Add("Retaining Walls");
            }

            raftList = FamilyInstanceList.documentLoopRaftFoundation();
            if (raftList.Any())
            {
                Select_Family.Items.Add("Raft Foundation");
            }
            stripFootingsList = FamilyInstanceList.documentLoopWallFoundation();
            if (stripFootingsList.Any())
            {
                Select_Family.Items.Add("Strip Footings");
            }
            ColumnSplit columnSplit = new ColumnSplit(doc);
            
            FamilyinstanceList = FamilyInstanceList.documentLoopFamilyInstance();
            //columnsList = FamilyInstanceList.documentLoopcolumnList(FamilyinstanceList);
            columnsList = columnSplit.columnsListChecked();
            if (columnsList.Any())
            {
                Select_Family.Items.Add("Columns");
            }
            beamsList = FamilyInstanceList.documentLoopsemellsList(FamilyinstanceList);
            if (beamsList.Any())
            {
                Select_Family.Items.Add("Semells");
            }
            recFootingsList = FamilyInstanceList.documentLooprectangularFootings(FamilyinstanceList);
            if (recFootingsList.Any())
            {
                Select_Family.Items.Add("Rectangular Footings");
            }
            
        }

        private void Element_Selection_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            try
            {
                obj = _uidoc.Selection.PickObject(ObjectType.Element, SingleSelectionFilter);
                ele = doc.GetElement(obj.ElementId);
                //Result r = Result.Succeeded;
            }
            catch (Exception)
            {
                
                Show();
            }
            
            



            double levelelement = SingleElementLevel.ElementLevelCalculation(obj, doc);
            



            if (UserInputLevel > levelelement)
            {
                GeometryElement geoElem = null;
                geoElem = ele.GetGeometryObjectFromReference(obj) as GeometryElement;

                if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation
                    || ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
                {
                    datagrid.ItemsSource = foot.faceinfor(ele, geoElem, doc).DefaultView;
                }
                else if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                    || ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                {
                    
                    datagrid.ItemsSource = col.Faceinfo(ele,geoElem, doc).DefaultView;
                }
            }
            else
            {
                TaskDialog.Show("Invalid Selection", "the element is above the entered level");
            }

            Show();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

