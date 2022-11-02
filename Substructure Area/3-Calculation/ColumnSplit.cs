using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Substructure_Area._3_Calculation
{
    class ColumnSplit
    {
        private Document doc { get; set; }
        private IList<Element> columnsList { get; set; }
        private List<Element> modifiedColumnsList { get; set; }
        private FormatOptions elevationformatoptions { get; set; }
        private double elementTopElevationValue;


        private static FilteredElementCollector ColumnsElementCollector { get; set; }
        private List<double> TopelementElevationList { get; set; }
        private ForgeTypeId levelUnit { get; set; }

        private List<ElementId> ElementTopParameterID { get; set; }
        private List<double> ElementTopOffsetValues { get; set; }
        private List<double> ElementbottomoffsetValues { get; set; }
        private ICollection<ElementId> listofAlllevelsID { get; set; }
        private List<ElementId> listofLevelsBelowUserInput { get; set; }
        private Level bottomElementLevel { get; set; }
        private double bottomElementElevation { get; set; }
        private double topElementElevation { get; set; }
        
        private int count { get; set; }

        public ColumnSplit(Document doc)
        {
            this.doc = doc;
            
            elevationformatoptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            TopelementElevationList = new List<double>();
            modifiedColumnsList = new List<Element>();
            levelUnit = elevationformatoptions.GetUnitTypeId();
            ColumnsElementCollector = new FilteredElementCollector(doc);
            columnsList = new List<Element>() ;
            ElementTopParameterID = new List<ElementId>();
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

            //Level ID for the levels below User Input
            //List<ElementId> listoflevels = new List<ElementId>();
            foreach (var item in listofAlllevelsID)
            {
                bottomElementLevel = doc.GetElement(item) as Level;
                bottomElementElevation = UnitUtils.ConvertFromInternalUnits(bottomElementLevel.Elevation, levelUnit);
                
                if (bottomElementElevation < getLevel.Userinput)
                {

                    listofLevelsBelowUserInput.Add(bottomElementLevel.Id) ;
                }
            }

            columnsList = ColumnsElementCollector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                .OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();



            foreach (var item in columnsList)
            {
                foreach (var lev in listofLevelsBelowUserInput)
                {
                    if (item.LevelId.IntegerValue == lev.IntegerValue)
                    {
                        modifiedColumnsList.Add(item);
                    }
                   
                }
            }

            ElementTopParameterID.AddRange(modifiedColumnsList.Select(x =>
                 x.LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM))
                 .AsElementId()));


            ElementTopOffsetValues.AddRange(modifiedColumnsList.Select(x =>
            x.LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM))
            .AsDouble()));

            ElementbottomoffsetValues.AddRange(modifiedColumnsList.Select(x =>
            x.LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM))
            .AsDouble()));

            count = 0;
            foreach (var item in ElementTopParameterID)
            {
                
                Level toplevel = doc.GetElement(item) as Level;
                topElementElevation = UnitUtils.ConvertFromInternalUnits(toplevel.Elevation, levelUnit);
                TopelementElevationList.Add(topElementElevation + ElementTopOffsetValues.ElementAt(count));
                count++;
            }

            if (TopelementElevationList.Where(x => x > getLevel.Userinput).Any())
            {
                switch (MessageBox.Show("Top level for some columns are above the entered level. " +
                    "Would you Like to split them at the entered level?", "Columns Top Level",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))

                {
                    case MessageBoxResult.Yes:
                        ElementTopParameterID.Clear();
                        count = 0;
                        foreach (var ele in modifiedColumnsList)
                        {
                            
                            FamilyInstance column = ele as FamilyInstance;

                            ElementTopParameterID.Add(column
                                .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM))
                                .AsElementId());
                            Level tolevel = doc.GetElement(ElementTopParameterID.ElementAt(count)) as Level;
                            elementTopElevationValue = UnitUtils.ConvertFromInternalUnits(tolevel.Elevation, levelUnit);

                            ElementId bottomlevelpara = column
                            .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM))
                            .AsElementId();
                            Level bottomlevel = doc.GetElement(bottomlevelpara) as Level;
                            bottomElementElevation = UnitUtils.ConvertFromInternalUnits(bottomlevel.Elevation, levelUnit);

                            double topoffsetvalue = column
                            .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM))
                            .AsDouble();


                            double splitRatio = (getLevel.Userinput - elebottom)
                                                    / (eletop + topoffsetvalue - elebottom);
                            if (eletop + topoffsetvalue > getLevel.Userinput)
                            {
                                using (Transaction tran = new Transaction(doc, "Split Columns"))
                                {
                                    tran.Start();
                                    column.Split(splitRatio);
                                    tran.Commit();
                                }
                            }
                            count++;
                        }
                        break;
                }
            }
            else if (TopelementElevationList.Where(x => x < getLevel.Userinput).Any())
            {
                TaskDialog.Show("Split Columns", "Columns top level is equal to the top elevation level");
            }
     
            return modifiedColumnsList;
            
        }
   
    }
}    


