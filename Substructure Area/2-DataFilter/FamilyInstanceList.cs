using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Substructure_Area._2_DataFilter
{
    class FamilyInstanceList
    {
        private static Document doc { get; set; }
        private static IList<BuiltInCategory> categoryList { get; set; }
        private static List<Element> elementsList { get; set; }

        private static List<Element> columnsList { get; set; }

        private static List<Element> beamsList { get; set; }

        private static List<Element> rectangularfootingsList { get; set; }

        private static List<Element> raftList { get; set; }
        private static List<Element> AllraftList { get; set; }
        private static IList<Element> wallList { get; set; }
        private static IList<Element> AllwallList { get; set; }

        private static IList<Element> stripfootingsList { get; set; }
        private static IList<Element> AllstripfootingsList { get; set; }
        private static FilteredElementCollector collector { get; set; }

        private static FilteredElementCollector wallCollector { get; set; }

        private static FilteredElementCollector wallFoundationCollector { get; set; }
        private static FilteredElementCollector raftCollector { get; set; }

        private static ElementMulticategoryFilter categoryFilter { get; set; }







        private FormatOptions elevationformatoptions { get; set; }

        private static ForgeTypeId levelUnit { get; set; }

        private static double BottomColumnElevation { get; set; }
        private static ElementId ColumnTopParameterID { get; set; }
        
        private static double TopColumnElevation { get; set; }
        
        private static double NewcolumnLength { get; set; }
        public static Level beamTopLevel { get; set; }
        public FamilyInstanceList(Document docu)
        {
            doc = docu;
            //Raft is different class (Floor)
            raftCollector = new FilteredElementCollector(doc);
            //wall strip is different class (Wall Foundation)
            wallFoundationCollector = new FilteredElementCollector(doc);
            // wall is different class (Wall)
            wallCollector = new FilteredElementCollector(doc);
            collector = new FilteredElementCollector(doc);

            //for the rectangular footings, columns, beams (Family Instance)
            categoryList = new List<BuiltInCategory>();

            elementsList = new List<Element>();

            raftList = new List<Element>();
            AllraftList = new List<Element>();
            stripfootingsList = new List<Element>();
            AllstripfootingsList = new List<Element>();
            rectangularfootingsList = new List<Element>();
            columnsList = new List<Element>();
            AllwallList = new List<Element>();
            wallList = new List<Element>();
            beamsList = new List<Element>();

            elevationformatoptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            levelUnit = elevationformatoptions.GetUnitTypeId();

        }
        public static List<Element> documentLoopFamilyInstance()
        {
            categoryList.Add(BuiltInCategory.OST_StructuralFoundation);
            categoryList.Add(BuiltInCategory.OST_StructuralColumns);
            categoryList.Add(BuiltInCategory.OST_StructuralFraming);
            categoryFilter = new ElementMulticategoryFilter(categoryList);

            elementsList.AddRange(collector.WherePasses(categoryFilter)
                .OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements());
            
            return elementsList;
        }
        public static IList<Element> documentLoopWall()
        {



            AllwallList = wallCollector.OfCategory(BuiltInCategory.OST_Walls)
                .OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();

            foreach (Element item in AllwallList)
            {
                Level elementLevel = doc.GetElement(item.LevelId) as Level;
                double elElevation = UnitUtils.ConvertFromInternalUnits(elementLevel.Elevation, levelUnit);
                if (elElevation < getLevel.Userinput)
                {
                    wallList.Add(item);
                }
            }
            return wallList;
        }

        public static IList<Element> documentLoopWallFoundation()
        {
            

            AllstripfootingsList = wallFoundationCollector.OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .OfClass(typeof(WallFoundation)).WhereElementIsNotElementType().ToElements();
            foreach (Element ele in AllstripfootingsList)
            {
                WallFoundation stripelement = doc.GetElement(ele.Id) as WallFoundation;
                Element stripele = doc.GetElement(stripelement.WallId);
                Level elementLevel = doc.GetElement(stripele.LevelId) as Level;
                double elElevation = UnitUtils.ConvertFromInternalUnits(elementLevel.Elevation, levelUnit);
                if (elElevation < getLevel.Userinput)
                {
                    stripfootingsList.Add(ele);
                }
            }
            return stripfootingsList;
        }
        public static List<Element> documentLoopRaftFoundation()
        {
            

            AllraftList.AddRange(raftCollector.OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .OfClass(typeof(Floor)).WhereElementIsNotElementType().ToElements());
            foreach (Element ele in AllraftList)
            {
                
                Level elementLevel = doc.GetElement(ele.LevelId) as Level;
                double elElevation = UnitUtils.ConvertFromInternalUnits(elementLevel.Elevation, levelUnit);
                if (elElevation < getLevel.Userinput)
                {
                    raftList.Add(ele);
                }
            }
            return raftList;
        }



        public static List<Element> documentLoopsemellsList(List<Element> Listofelements)
        {
            
            elementsList = Listofelements;
            foreach (Element element in elementsList)
            {
                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
                    && element.LevelId.IntegerValue == -1)
                {
                    FamilyInstance beaminstance = doc.GetElement(element.Id) as FamilyInstance;
                    try
                    {
                        if (beaminstance.Host != null && beaminstance.StructuralMaterialType == StructuralMaterialType.Concrete)
                        {
                            beamTopLevel = beaminstance.Host as Level;

                        }
                        else if (beaminstance.Host == null && beaminstance.StructuralMaterialType == StructuralMaterialType.Concrete)
                        {
                            beamTopLevel = doc.GetElement(beaminstance.LookupParameter(LabelUtils
                                .GetLabelFor(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM)).AsElementId()) as Level;

                        }
                        else
                            break;
                        double elElevation = UnitUtils.ConvertFromInternalUnits(beamTopLevel.Elevation, levelUnit);
                        if (elElevation < getLevel.Userinput)
                        {
                            beamsList.Add(element);
                        }
                    }
                    catch (Exception)
                    {

                        
                    }
                    

                }
                
            }
            
            return beamsList;

        }
        public static List<Element> documentLooprectangularFootings(List<Element> Listofelements)
        {
            elementsList = Listofelements;
            foreach (var element in elementsList)
            {
                
                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
                {

                    Level elementLevel = doc.GetElement(element.LevelId) as Level;
                    double elElevation = UnitUtils.ConvertFromInternalUnits(elementLevel.Elevation, levelUnit);
                    if (elElevation < getLevel.Userinput)
                    {
                        rectangularfootingsList.Add(element);
                    }
                    
                    
                }
            }
            return rectangularfootingsList;

        }
    }
}
