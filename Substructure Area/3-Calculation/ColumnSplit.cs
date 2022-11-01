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
        private double eletop;


        private static FilteredElementCollector ColumnsElementCollector { get; set; }
        private List<double> elElevation { get; set; }
        private ForgeTypeId levelUnit { get; set; }

        private List<ElementId> ElementTopParameterID { get; set; }
        private List<double> ElementTopOffsetID { get; set; }
        private List<int> elementsindexbottom { get; set; }

        public ColumnSplit(Document doc)
        {
            this.doc = doc;
            
            elevationformatoptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            elElevation = new List<double>();
            modifiedColumnsList = new List<Element>();
            levelUnit = elevationformatoptions.GetUnitTypeId();
            ColumnsElementCollector = new FilteredElementCollector(doc);
            columnsList = new List<Element>() ;
            ElementTopParameterID = new List<ElementId>();
            ElementTopOffsetID = new List<double>();
        }
        public List<Element> columnsListChecked()
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> levels = collector.OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level)).ToElementIds();

            List<ElementId> listoflevels = new List<ElementId>();
            foreach (var item in levels)
            {
                Level lev = doc.GetElement(item) as Level;
                double levelelv = UnitUtils.ConvertFromInternalUnits(lev.Elevation, levelUnit);
                if (levelelv < getLevel.Userinput)
                {
                    listoflevels.Add(lev.Id) ;
                }
            }

            columnsList = ColumnsElementCollector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                .OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();



            foreach (var item in columnsList)
            {
                foreach (var lev in listoflevels)
                {
                    if (item.LevelId.IntegerValue == lev.IntegerValue)
                    {
                        modifiedColumnsList.Add(item);
                    }
                   
                }
            }

            ElementTopParameterID.AddRange(modifiedColumnsList.Select(x =>
                 x.LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM)).AsElementId()));

            
            ElementTopOffsetID.AddRange(modifiedColumnsList.Select(x =>
            x.LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM)).AsDouble()));
            int count = 0;
            foreach (var item in ElementTopParameterID)
            {
                
                Level toplevel = doc.GetElement(item) as Level;
                double elementtoplevel = UnitUtils.ConvertFromInternalUnits(toplevel.Elevation, levelUnit);
                elElevation.Add(elementtoplevel + ElementTopOffsetID.ElementAt(count));
                count++;
            }

            if (elElevation.Where(x => x > getLevel.Userinput).Any())
            {
                switch (MessageBox.Show("Top level for some columns are above the entered level. " +
                    "Would you Like to split them at the entered level?", "Columns Top Level",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))

                {
                    case MessageBoxResult.Yes:

                        foreach (var ele in modifiedColumnsList)
                        {

                            FamilyInstance column = ele as FamilyInstance;

                            ElementId topLevelParameter = column
                                .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM))
                                .AsElementId();
                            Level tolevel = doc.GetElement(topLevelParameter) as Level;
                            eletop = UnitUtils.ConvertFromInternalUnits(tolevel.Elevation, levelUnit);

                            ElementId bottomlevelpara = column
                            .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM))
                            .AsElementId();
                            Level bottomlevel = doc.GetElement(bottomlevelpara) as Level;
                            double elebottom = UnitUtils.ConvertFromInternalUnits(bottomlevel.Elevation, levelUnit);

                            double topoffsetvalue = column
                            .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM))
                            .AsDouble();


                            double elementlength = (getLevel.Userinput - elebottom)
                                                    / (eletop + topoffsetvalue - elebottom);
                            if (eletop + topoffsetvalue > getLevel.Userinput)
                            {
                                using (Transaction tran = new Transaction(doc, "Split Columns"))
                                {
                                    tran.Start();
                                    column.Split(elementlength);
                                    tran.Commit();
                                }
                            }
                            


                            //modifiedColumnsList.Add(column);




                        }
                        break;
                }

            }
            else if (elElevation.Where(x => x < getLevel.Userinput).Any())
            {
                TaskDialog.Show("Split Columns", "Columns top level is equal to the top elevation level");
            }
            return modifiedColumnsList;
        }
   
    }
}            






        //MessageBoxResult result = (MessageBox.Show("Top level for some columns are above the entered level. " +
        //            "Would you Like to split them at the entered level?", "Columns Top Level",
        //            MessageBoxButton.YesNo, MessageBoxImage.Question));
        //        if (result == MessageBoxResult.OK)
        //        {
        //            foreach (Element ele in columnsList)
        //            {
        //                FamilyInstance column = ele as FamilyInstance;
        //                ElementId topLevelParameter = column
        //                    .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM))
        //                    .AsElementId();
        //                Level toplevel = doc.GetElement(topLevelParameter) as Level;
        //                elElevation = UnitUtils.ConvertFromInternalUnits(toplevel.Elevation, levelUnit);
        //                ElementId bottomlevelpara = column
        //                 .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM))
        //                 .AsElementId();
        //        Level bottomlevel = doc.GetElement(bottomlevelpara) as Level;
        //        double elementlength = (getLevel.Userinput - bottomlevel.Elevation)
        //                                / (toplevel.Elevation - bottomlevel.Elevation);

        //        using (Transaction tran = new Transaction(doc, "splitcolumn"))
        //        {
        //            tran.Start();
        //            column.Split(elementlength);
        //            tran.Commit();
        //        }
        //        modifiedColumnsList.Add(column);
        //break;



        //    FamilyInstance column = ele as FamilyInstance;
        //    ElementId topLevelParameter = column
        //        .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM))
        //        .AsElementId();
        //    Level toplevel = doc.GetElement(topLevelParameter) as Level;
        //    elElevation = UnitUtils.ConvertFromInternalUnits(toplevel.Elevation, levelUnit);
        //    if (elElevation > getLevel.Userinput)
        //    {
        //        switch (MessageBox.Show("Top level for some columns are above the entered level. " +
        //            "Would you Like to split them at the entered level?", "Columns Top Level",
        //            MessageBoxButton.YesNo, MessageBoxImage.Question))
        //        {
        //            case MessageBoxResult.Yes:
        //                //splitcolumn(column, ele, toplevel);
        //                ElementId bottomlevelpara = column
        //                .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM))
        //                .AsElementId();
        //                Level bottomlevel = doc.GetElement(bottomlevelpara) as Level;
        //                double elementlength = (getLevel.Userinput - bottomlevel.Elevation)
        //                                        / (toplevel.Elevation - bottomlevel.Elevation);

        //                using (Transaction tran = new Transaction(doc, "splitcolumn"))
        //                {
        //                    tran.Start();
        //                    column.Split(elementlength);
        //                    tran.Commit();
        //                }
        //                modifiedColumnsList.Add(column);


        //                break;
        //            case MessageBoxResult.No:
        //                break;



        //            }

        //        }
        //        return modifiedColumnsList;
        //    }

        //}
        //public void splitcolumn(FamilyInstance column, Element ele, Level toplevel)
        //{
        //    ElementId bottomlevelpara = column
        //    .LookupParameter(LabelUtils.GetLabelFor(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM))
        //    .AsElementId();
        //    Level bottomlevel = doc.GetElement(bottomlevelpara) as Level;
        //    double elementlength = (getLevel.Userinput - bottomlevel.Elevation)
        //                            / (toplevel.Elevation - bottomlevel.Elevation);

        //    using (Transaction tran = new Transaction(doc, "splitcolumn"))
        //    {
        //        tran.Start();
        //        column.Split(elementlength);
        //        tran.Commit();
        //    }
        //    modifiedColumnsList.Add(column);


