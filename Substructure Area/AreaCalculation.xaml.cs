﻿using System;
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
        public static double Userinput;
        public List<double> facesdata;
        public SingleFootingCalculation foot;
        public List<Element> FamilyinstanceList { get; set; }
        public IList<Element> wallsList { get; set; }
        public IList<Element> columnsList { get; set; }
        public IList<Element> beamsList { get; set; }
        public IList<Element> raftList { get; set; }
        public IList<Element> recFootingsList { get; set; }
        public IList<Element> stripFootingsList { get; set; }
        public areaCalculation(UIDocument uidoc)
        {
            InitializeComponent();

            this.UserInputLevel = UserInputLevel;
            facesdata = new List<double>();
            _uidoc = uidoc;
            doc = uidoc.Document;
            userSelection = new Userselection(uidoc);
            foot = new SingleFootingCalculation();

            FamilyinstanceList = new List<Element>();
            wallsList = new List<Element>();
            raftList = new List<Element>();
            recFootingsList = new List<Element>();
            beamsList = new List<Element>();
            stripFootingsList = new List<Element>();
            columnsList = new List<Element>();
            FamilyInstanceList famlist = new FamilyInstanceList(doc);
            wallsList = FamilyInstanceList.documentLoopWall();
            ColumnSplit spcol = new ColumnSplit(doc);

            columnsList = spcol.columnsListChecked();
            if (wallsList.Any())
            {
                Select_Family.Items.Add("Wall Structure");
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
            FamilyinstanceList = FamilyInstanceList.documentLoopFamilyInstance();
            columnsList = FamilyInstanceList.documentLoopcolumnList(FamilyinstanceList);
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

            Reference obj = userSelection.Object();
            Element ele = doc.GetElement(obj.ElementId);



            double levelelement = SingleElementLevel.ElementLevelCalculation(obj, doc);
            //Userinput = double.Parse(UserInputLevel);



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
                    ColumnBeamCalculation col = new ColumnBeamCalculation();
                    col.Faceinfo(geoElem, doc);
                }
            }
            else
            {
                TaskDialog.Show("Invalid Selection", "the element is above the entered level");
            }



            //datagrid.Items.Add(facesdata);

            
            Show();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            
        }

        private void splitcolumn_Click(object sender, RoutedEventArgs e)
        {
            
            
        }
    }
}
