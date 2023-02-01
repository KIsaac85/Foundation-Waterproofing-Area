using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Substructure_Area._3_Calculation
{
    class ColumnSplit
    {
        private Document doc { get; set; }
        private IList<Element> columnsList { get; set; }
        private List<Element> modifiedColumnsList { get; set; }
        private FormatOptions elevationformatoptions { get; set; }
        private ForgeTypeId levelUnit { get; set; }


        private static FilteredElementCollector ColumnsElementCollector { get; set; }

        private FamilyInstance Column { get; set; }

        private List<ElementId> ElementTopParameterID { get; set; }
        private List<ElementId> ElementBottomParameterID { get; set; }
        private ICollection<ElementId> listofAlllevelsID { get; set; }
        private List<ElementId> listofLevelsBelowUserInput { get; set; }
        private List<double> ElementTopOffsetValues { get; set; }
        private List<double> ElementbottomoffsetValues { get; set; }
        private List<double> TopelementElevationList { get; set; }
        private Level bottomElementLevel { get; set; }
        private Level topElementLevel { get; set; }
        private double bottomElementElevation { get; set; }
        private double topElementElevation { get; set; }
        private double elementTopElevationValue { get; set; }
        private double splitRatio { get; set; }
        private int count { get; set; }

        public ColumnSplit(Document doc)
        {
            this.doc = doc;

            elevationformatoptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            TopelementElevationList = new List<double>();
            modifiedColumnsList = new List<Element>();
            levelUnit = elevationformatoptions.GetUnitTypeId();
            ColumnsElementCollector = new FilteredElementCollector(doc);
            columnsList = new List<Element>();
            ElementTopParameterID = new List<ElementId>();
            ElementBottomParameterID = new List<ElementId>();
            listofAlllevelsID = new List<ElementId>();
            listofLevelsBelowUserInput = new List<ElementId>();
            ElementTopOffsetValues = new List<double>();
            ElementbottomoffsetValues = new List<double>();
        }
        public List<Element> columnsListChecked()
        {
            //Collector for all levels
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            listofAlllevelsID = collector.OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level)).ToElementIds();

            //List of Level ID for the levels below User Input

            foreach (var item in listofAlllevelsID)
            {
                bottomElementLevel = doc.GetElement(item) as Level;
                bottomElementElevation = UnitUtils.ConvertFromInternalUnits(bottomElementLevel.Elevation, levelUnit);

                if (bottomElementElevation <= getLevel.Userinput)
                {

                    listofLevelsBelowUserInput.Add(bottomElementLevel.Id);
                }
            }
            //list of all columns in the project
            columnsList = ColumnsElementCollector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                .OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();


            //add columns which their lower level is below the user input
            foreach (var item in columnsList)
            {
                foreach (ElementId lev in listofLevelsBelowUserInput)
                {
                    if (item.LevelId.IntegerValue == lev.IntegerValue)
                    {
                        FamilyInstance column = item as FamilyInstance;
                        if (column.StructuralMaterialType== StructuralMaterialType.Concrete)
                        {
                            modifiedColumnsList.Add(item);
                        }
                        
                    }

                }
            }
            //getting the top levelid for modified columns which are supposed to be split
            ElementTopParameterID.AddRange(modifiedColumnsList.Select(x =>
                 x.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM)
                 .AsElementId()));

            //getting the top offset value for modified columns which are supposed to be split
            ElementTopOffsetValues.AddRange(modifiedColumnsList.Select(x =>
            x.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM)
            .AsDouble()));
            //getting the bottom offset value for modified columns which are supposed to be split
            ElementbottomoffsetValues.AddRange(modifiedColumnsList.Select(x =>
            x.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM)
            .AsDouble()));

            count = 0;
            foreach (var item in ElementTopParameterID)
            {
                //calculating the elevation of element top
                topElementLevel = doc.GetElement(item) as Level;
                topElementElevation = UnitUtils
                    .ConvertFromInternalUnits(topElementLevel.Elevation, levelUnit);

                TopelementElevationList.Add(topElementElevation + ElementTopOffsetValues.ElementAt(count));
                count++;
            }
            ElementTopParameterID.Clear();
            ElementTopOffsetValues.Clear();
            ElementbottomoffsetValues.Clear();
            count = 0;
            if (TopelementElevationList.Where(x => x > getLevel.Userinput).Any())
            {

                switch (MessageBox.Show("Top level for some columns are above the entered level. " +
                    "Would you Like to split them at the entered level?", "Columns Top Level",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))

                {
                    case MessageBoxResult.Yes:
                        splittingcolumns(modifiedColumnsList);
                        break;

                    case MessageBoxResult.No:
                        
                        switch (MessageBox.Show(
                            "Calculated areas may not be accurate based on the entered elevation. Are you sure you want to continue? " +
                            "Press yes to continue without split or no to split columns and continue" , "Split Columns", MessageBoxButton.YesNo,MessageBoxImage.Warning))
                        {
                            case MessageBoxResult.Yes:
                                break;
                            case MessageBoxResult.No:
                                splittingcolumns(modifiedColumnsList);
                                break;
                        }
                        break;
                }
            }
            return modifiedColumnsList;
        }
        public List<Element> splittingcolumns(List<Element> ColumnslistSplit)
        {
            foreach (var ele in modifiedColumnsList)
            {

                Column = ele as FamilyInstance;

                ElementTopParameterID.Add(Column
                    .get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM)
                    .AsElementId());
                topElementLevel = doc.GetElement(ElementTopParameterID.ElementAt(count)) as Level;
                elementTopElevationValue = UnitUtils.ConvertFromInternalUnits(topElementLevel.Elevation, levelUnit);

                ElementBottomParameterID.Add(Column
                .get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM)
                .AsElementId());
                bottomElementLevel = doc.GetElement(ElementBottomParameterID.ElementAt(count)) as Level;
                bottomElementElevation = UnitUtils.ConvertFromInternalUnits(bottomElementLevel.Elevation, levelUnit);

                ElementTopOffsetValues.Add(UnitUtils.ConvertFromInternalUnits(Column
                .get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM)
                .AsDouble(), levelUnit));

                ElementbottomoffsetValues.Add(UnitUtils.ConvertFromInternalUnits(Column
                .get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM)
                .AsDouble(), levelUnit));
                splitRatio = (getLevel.Userinput - bottomElementElevation - ElementbottomoffsetValues.ElementAt(count))
                                        / (elementTopElevationValue - bottomElementElevation
                                        + ElementTopOffsetValues.ElementAt(count) - ElementbottomoffsetValues.ElementAt(count));

                if (Math.Round(elementTopElevationValue, 2) + Math.Round(ElementTopOffsetValues.ElementAt(count), 2) > getLevel.Userinput && splitRatio != 0)
                {
                    using (Transaction tran = new Transaction(doc, "Split Columns"))
                    {
                        try
                        {
                            tran.Start();
                            Column.Split(splitRatio);
                            tran.Commit();
                        }
                        catch (Exception)
                        {

                            TaskDialog.Show("The Input is invalid", "Columns Can not be split based on your input");
                            break;
                        }

                    }
                }
                count++;

            }


            return ColumnslistSplit;
        }
    }
}


