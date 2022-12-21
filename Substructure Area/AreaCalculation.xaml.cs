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
using Substructure_Area._5__Excel_Export;

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
        
        private Element ele;
        private Reference obj;
        public static double Userinput;
        public List<double> facesdata;
        public FoundationWall foot;
        private ColumnBeamCalculation col { get; set; }
        private  List<Element> FamilyinstanceList { get; set; }
        private IList<Element> wallsList { get; set; }
        private IList<Element> columnsList { get; set; }
        private IList<Element> beamsList { get; set; }
        private IList<Element> raftList { get; set; }
        private IList<Element> recFootingsList { get; set; }
        private IList<Element> stripFootingsList { get; set; }
        private static SelectionFilter SingleSelectionFilter;
        private FormatOptions areaFormatOptions { get; set; }
        private ForgeTypeId areaUnit { get; set; }
        private static FormatOptions levelFormatOptions { get; set; }
        private static ForgeTypeId levelunit { get; set; }
        public areaCalculation(UIDocument uidoc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            doc = uidoc.Document;
            areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            areaUnit = areaFormatOptions.GetUnitTypeId();
            levelFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            levelunit = levelFormatOptions.GetUnitTypeId();
            this.UserInputLevel = UserInputLevel;
            SingleSelectionFilter = new SelectionFilter();
            facesdata = new List<double>();

            
            foot = new FoundationWall();
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
            }
            catch (Exception){}


            if (obj!=null)
            {
                ele = doc.GetElement(obj.ElementId);
                double levelelement = SingleElementLevel.ElementLevelCalculation(obj, doc,levelunit);

                if (UserInputLevel > levelelement)
                {
                    
                    if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation
                        || ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
                    {
                        datagrid.ItemsSource = foot.faceinfor(ele, areaUnit).DefaultView;
                    }
                    else if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                        || ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {

                        datagrid.ItemsSource = col.Faceinfo(ele, areaUnit).DefaultView;
                    }
                }
                else
                {
                    TaskDialog.Show("Invalid Selection", "the element is above the entered level");
                }
            }

            Show();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_As_Click(object sender, RoutedEventArgs e)
        {
            List<string > ListOfSelectedItemsItems = new List<string>() ;
            
            foreach (var item in Select_Family.SelectedItems)
            {
                ListOfSelectedItemsItems.Add(item.ToString());
                ExcelData file = new ExcelData();
                file.DataTable(ListOfSelectedItemsItems,wallsList,areaUnit);

            }
            
            
        }
    }
}

