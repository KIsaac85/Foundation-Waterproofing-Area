using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;


namespace Substructure_Area
{
    /// <summary>
    /// This class is created to get the level
    /// of selected element
    /// Selected level shall be below the entered
    /// level by user
    /// </summary>
    class SingleElementLevel
    {


        #region Members
        private static Element Element;
        private static Level levelElement;
        private static double ElevationLevel; 
        #endregion



        /// <summary>
        /// A function to get the level of 
        /// the selected element by user
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="doc"></param>
        /// <param name="levelunit"></param>
        /// <returns>elevationLevel</returns>
        public static double ElementLevelCalculation(Reference obj, Document doc, ForgeTypeId levelunit)
        {
            Element = doc.GetElement(obj.ElementId);

            if (Element.LevelId.IntegerValue != -1)
            {

                ElementId levelid = Element.LevelId;
                levelElement = doc.GetElement(levelid) as Level;
            }

            else if (Element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && Element.LevelId.IntegerValue == -1)
            {

                FamilyInstance beam = doc.GetElement(obj.ElementId) as FamilyInstance;
                if (beam.Host != null && beam.StructuralMaterialType == StructuralMaterialType.Concrete)
                {
                    levelElement = beam.Host as Level;
                }
                else if (beam.Host == null && beam.StructuralMaterialType == StructuralMaterialType.Concrete)
                {
                    levelElement = doc.GetElement(beam.LookupParameter(LabelUtils
                        .GetLabelFor(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM)).AsElementId()) as Level;

                }

            }

            else if (Element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation && Element.LevelId.IntegerValue == -1)
            {
                WallFoundation wallFound = doc.GetElement(obj.ElementId) as WallFoundation;
                ElementId wallFoundid = wallFound.WallId;
                Element elewalllevel = doc.GetElement(wallFoundid);
                levelElement = doc.GetElement(elewalllevel.LevelId) as Level;
            }
            ElevationLevel = UnitUtils.ConvertFromInternalUnits(levelElement.Elevation, levelunit);

            return ElevationLevel;
        }
    }
}
