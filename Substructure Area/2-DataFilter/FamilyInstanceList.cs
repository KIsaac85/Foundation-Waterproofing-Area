using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;


namespace Substructure_Area._2_DataFilter
{
    /// <summary>
    /// This class is created to loop through the document
    /// and filters the required document
    /// which are below the user input
    /// </summary>
    class FamilyInstanceList
    {
        private static Document doc { get; set; }


        #region Collectors
        /// <summary>
        /// Family instance collector for columns beams and isolated footings
        /// A different collector is created for the rest because they are 
        /// different types
        /// </summary>
        private static FilteredElementCollector FamilyInstanceCollector { get; set; }
        private static FilteredElementCollector raftCollector { get; set; }
        private static FilteredElementCollector wallCollector { get; set; }

        private static FilteredElementCollector StripFootingCollector { get; set; }
        #endregion

        #region Units Calculation
        private FormatOptions elevationformatoptions { get; set; }

        private static ForgeTypeId levelUnit { get; set; }
        #endregion

        #region Element Level Calculation
        private static Level elementlevel { get; set; }

        private static double elementelevation { get; set; }
        #endregion


        /// <summary>
        /// Lists of elements which their levels are below the entered level from user
        /// Raft, walls and strip footings are collected in two lists first for all elements in the document
        /// then casting to get their levels first and then gather them in the second list
        /// </summary>
        /// 

        #region familyInstanceFilter
        private static IList<BuiltInCategory> FamilyInstanceCategoryList { get; set; }
        private static ElementMulticategoryFilter categoryFilter { get; set; } 
        #endregion

        #region Family instance lists Beams/Isolated Footings Lists Property
        private static FamilyInstance elementFamilyInstance { get; set; }
        private static List<Element> elementsList { get; set; }
        private static List<Element> beamsList { get; set; }

        private static List<Element> rectangularfootingsList { get; set; }
        #endregion

        #region Raft List Property
        private static List<Element> raftList { get; set; }
        private static List<Element> AllraftList { get; set; }
        #endregion

        #region Walls List Property
        private static IList<Element> wallList { get; set; }
        private static IList<Element> AllwallList { get; set; }
        private static Wall wall { get; set; }
        
        #endregion

        #region Strip Footing List Property
        private static IList<Element> wallFoundationList { get; set; }
        private static IList<Element> AllwallFoundationList { get; set; }
        private static WallFoundation wallFoundation { get; set; }
        private static Element wallFoundationElement { get; set; }
        #endregion


        /// <summary>
        /// Constructor to initialize objects for lists
        /// </summary>
        /// <param name="_doc"></param>
        public FamilyInstanceList(Document _doc)
        {
            doc = _doc;
            //Raft is different class (Floor)
            raftCollector = new FilteredElementCollector(doc);
            //wall strip is different class (Wall Foundation)
            StripFootingCollector = new FilteredElementCollector(doc);
            // wall is different class (Wall)
            wallCollector = new FilteredElementCollector(doc);
            FamilyInstanceCollector = new FilteredElementCollector(doc);

            //for the rectangular footings, beams (Family Instance)
            FamilyInstanceCategoryList = new List<BuiltInCategory>();

            elementsList = new List<Element>();

            raftList = new List<Element>();
            AllraftList = new List<Element>();
            wallFoundationList = new List<Element>();
            AllwallFoundationList = new List<Element>();
            rectangularfootingsList = new List<Element>();
            
            AllwallList = new List<Element>();
            wallList = new List<Element>();
            beamsList = new List<Element>();

            elevationformatoptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            levelUnit = elevationformatoptions.GetUnitTypeId();

        }


        /// <summary>
        /// A function to filter the elements of type family instance
        /// e.g. beams and footings
        /// </summary>
        /// <returns>list of elements that contains beams and footings</returns>
        public static List<Element> documentLoopFamilyInstance()
        {
            FamilyInstanceCategoryList.Add(BuiltInCategory.OST_StructuralFoundation);
            FamilyInstanceCategoryList.Add(BuiltInCategory.OST_StructuralFraming);
            categoryFilter = new ElementMulticategoryFilter(FamilyInstanceCategoryList);

            elementsList.AddRange(FamilyInstanceCollector.WherePasses(categoryFilter)
                .OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements());

            return elementsList;
        }


        /// <summary>
        /// A function to filter the elements of type Wall
        /// e.g. Walls
        /// </summary>
        /// <returns>List of walls</returns>
        public static IList<Element> DocumentLoopWall()
        {
            AllwallList = wallCollector.OfCategory(BuiltInCategory.OST_Walls)
                .OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElements();

            foreach (Element item in AllwallList)
            {
                wall = item as Wall;
                if (wall.StructuralUsage == StructuralWallUsage.Bearing && wall.CurtainGrid == null)
                {
                    elementlevel = doc.GetElement(item.LevelId) as Level;
                    elementelevation = UnitUtils.ConvertFromInternalUnits(elementlevel.Elevation, levelUnit);
                    if (elementelevation <= getLevel.Userinput)
                    {
                        wallList.Add(item);
                    } 
                }
            }
            return wallList;
        }

        /// <summary>
        /// A function to filter the elements of type WallFoundation
        /// e.g. Strip Footings
        /// </summary>
        /// <returns> Strip Footings List</returns>
        public static IList<Element> documentLoopWallFoundation()
        {


            AllwallFoundationList = StripFootingCollector.OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .OfClass(typeof(WallFoundation)).WhereElementIsNotElementType().ToElements();
            foreach (Element ele in AllwallFoundationList)
            {
                wallFoundation = doc.GetElement(ele.Id) as WallFoundation;
                wallFoundationElement = doc.GetElement(wallFoundation.WallId);
                elementlevel = doc.GetElement(wallFoundationElement.LevelId) as Level;
                elementelevation = UnitUtils.ConvertFromInternalUnits(elementlevel.Elevation, levelUnit);
                if (elementelevation <= getLevel.Userinput)
                {
                    wallFoundationList.Add(ele);
                }
            }
            return wallFoundationList;
        }

        /// <summary>
        /// A function to filter the elements of type Floor
        /// e.g. Raft
        /// </summary>
        /// <returns> list of raft elements</returns>
        public static List<Element> documentLoopRaftFoundation()
        {


            AllraftList.AddRange(raftCollector.OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .OfClass(typeof(Floor)).WhereElementIsNotElementType().ToElements());
            foreach (Element ele in AllraftList)
            {

                elementlevel = doc.GetElement(ele.LevelId) as Level;
                elementelevation = UnitUtils.ConvertFromInternalUnits(elementlevel.Elevation, levelUnit);
                if (elementelevation <= getLevel.Userinput)
                {
                    raftList.Add(ele);
                }
            }
            return raftList;
        }


        /// <summary>
        /// The list of family instance contains beams/semells and footings
        /// this functions filters the concrete beams/semells
        /// </summary>
        /// <param name="Listofelements"></param>
        /// <returns>beams list</returns>
        public static List<Element> documentLoopsemellsList(List<Element> Listofelements)
        {

            elementsList = Listofelements;
            foreach (Element element in elementsList)
            {
                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
                    && element.LevelId.IntegerValue == -1)
                {
                    elementFamilyInstance = doc.GetElement(element.Id) as FamilyInstance;
                    if (elementFamilyInstance.SuperComponent == null&& elementFamilyInstance.StructuralMaterialType==StructuralMaterialType.Concrete)
                    {
                        try
                        {
                            if (elementFamilyInstance.Host != null && elementFamilyInstance.StructuralMaterialType == StructuralMaterialType.Concrete)
                            {
                                elementlevel = elementFamilyInstance.Host as Level;

                            }
                            else if (elementFamilyInstance.Host == null && elementFamilyInstance.StructuralMaterialType == StructuralMaterialType.Concrete)
                            {
                                elementlevel = doc.GetElement(elementFamilyInstance.LookupParameter(LabelUtils
                                    .GetLabelFor(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM)).AsElementId()) as Level;

                            }
                            else
                                break;
                            elementelevation = UnitUtils.ConvertFromInternalUnits(elementlevel.Elevation, levelUnit);
                            if (elementelevation <= getLevel.Userinput)
                            {

                                beamsList.Add(element);
                            }
                        }
                        catch (Exception) { }
                    }



                }

            }

            return beamsList;

        }

        /// <summary>
        /// The list of family instance contains beams/semells and footings
        /// this functions filters the concrete footings
        /// </summary>
        /// <param name="Listofelements"></param>
        /// <returns>Footings List</returns>
        public static List<Element> documentLooprectangularFootings(List<Element> Listofelements)
        {
            elementsList = Listofelements;
            foreach (var element in elementsList)
            {

                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
                {

                    elementlevel = doc.GetElement(element.LevelId) as Level;
                    elementelevation = UnitUtils.ConvertFromInternalUnits(elementlevel.Elevation, levelUnit);
                    if (elementelevation <= getLevel.Userinput)
                    {
                        elementFamilyInstance = element as FamilyInstance;
                        if (elementFamilyInstance.SuperComponent == null)
                        {
                            rectangularfootingsList.Add(element);
                        }


                    }


                }
            }
            return rectangularfootingsList;

        }
    }
}
