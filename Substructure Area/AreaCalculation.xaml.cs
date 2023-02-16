using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Substructure_Area._2_DataFilter;
using Substructure_Area._3_Calculation;
using Substructure_Area._5__Excel_Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Substructure_Area
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class areaCalculation : Window
    {

        #region Document
        private UIDocument _uidoc { get; set; }
        private Document doc { get; set; } 
        #endregion

        #region Single Element Selection Members
        private Element singleElement { get; set; }
        private Reference objectReference { get; set; }
        private static SelectionFilter SingleSelectionFilter { get; set; }
        private static double levelelement { get; set; }
        #endregion

        #region Elements Surface Area Calculation Members
        private FoundationSurfaceAreas footings { get; set; }
        private ColumnBeamSurfaceArea columnbeam { get; set; } 
        #endregion

        #region List of elements 
        private List<Element> FamilyinstanceList { get; set; }
        private IList<Element> wallsList { get; set; }
        private IList<Element> columnsList { get; set; }
        private IList<Element> beamsList { get; set; }
        private IList<Element> raftList { get; set; }
        private IList<Element> recFootingsList { get; set; }
        private IList<Element> stripFootingsList { get; set; } 
        #endregion

        #region Units Calculation Members
        private FormatOptions areaFormatOptions { get; set; }
        private ForgeTypeId areaUnit { get; set; }
        private static FormatOptions levelFormatOptions { get; set; }
        private static ForgeTypeId levelunit { get; set; } 
        #endregion

        /// <summary>
        /// Constructor to initialize component
        /// </summary>
        /// <param name="uidoc"></param>
        public areaCalculation(UIDocument uidoc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            doc = uidoc.Document;
            areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            areaUnit = areaFormatOptions.GetUnitTypeId();
            levelFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            levelunit = levelFormatOptions.GetUnitTypeId();

            SingleSelectionFilter = new SelectionFilter();
            



            footings = new FoundationSurfaceAreas();
            columnbeam = new ColumnBeamSurfaceArea();

            FamilyinstanceList = new List<Element>();
            wallsList = new List<Element>();
            raftList = new List<Element>();
            recFootingsList = new List<Element>();
            beamsList = new List<Element>();
            stripFootingsList = new List<Element>();
            columnsList = new List<Element>();
            FamilyInstanceList famlist = new FamilyInstanceList(doc);

            wallsList = FamilyInstanceList.DocumentLoopWall();
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

        /// <summary>
        /// Select Element Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Element_Selection_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            /*
             * Element shall be below the user input level
             * if it was the data shall be published 
             * in the datagrid
            */
            try
            {
                objectReference = _uidoc.Selection.PickObject(ObjectType.Element, SingleSelectionFilter);
            }
            catch (Exception) {  }


            if (objectReference != null)
            {
                singleElement = doc.GetElement(objectReference.ElementId);
                levelelement = SingleElementLevel.ElementLevelCalculation(objectReference, doc, levelunit);

                if (getLevel.Userinput > levelelement)
                {

                    if (singleElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation
                        || singleElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
                    {

                        datagrid.ItemsSource = footings.faceinfor(singleElement, areaUnit).DefaultView;
                    }
                    else if (singleElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                        || singleElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {

                        datagrid.ItemsSource = columnbeam.Faceinfo(singleElement, areaUnit).DefaultView;
                    }
                }
                else
                {
                    TaskDialog.Show("Invalid Selection", "the element is above the entered level");
                }
            }

            Show();

        }


        /// <summary>
        /// Save As Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_As_Click(object sender, RoutedEventArgs e)
        {
            List<string> ListOfSelectedItemsItems = new List<string>();

            foreach (var item in Select_Family.SelectedItems)
            {
                ListOfSelectedItemsItems.Add(item.ToString());
            }
            ExcelData file = new ExcelData();
            file.DataTable(Select_Family, wallsList, recFootingsList, columnsList, beamsList, raftList, stripFootingsList, areaUnit);

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// A button to go back to previous WPF 
        /// "Get level"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
            getLevel getLevel = new getLevel(_uidoc);
            getLevel.ShowDialog();
        }
    }
}

