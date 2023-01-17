using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Substructure_Area
{
    class SingleElementLevel
    {


        private static Element Element;
        private static Level levelelement;
        private static double ElevationLevel;


        public static Double ElementLevelCalculation(Reference obj, Document doc, ForgeTypeId levelunit)
        {
            ;
            Element = doc.GetElement(obj.ElementId);

            if (Element.LevelId.IntegerValue != -1)
            {

                ElementId levelid = Element.LevelId;
                levelelement = doc.GetElement(levelid) as Level;
            }

            else if (Element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && Element.LevelId.IntegerValue == -1)
            {

                FamilyInstance beam = doc.GetElement(obj.ElementId) as FamilyInstance;
                if (beam.Host != null && beam.StructuralMaterialType == StructuralMaterialType.Concrete)
                {
                    levelelement = beam.Host as Level;
                }
                else if (beam.Host == null && beam.StructuralMaterialType == StructuralMaterialType.Concrete)
                {
                    levelelement = doc.GetElement(beam.LookupParameter(LabelUtils
                        .GetLabelFor(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM)).AsElementId()) as Level;

                }

            }

            else if (Element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation && Element.LevelId.IntegerValue == -1)
            {
                WallFoundation wallFound = doc.GetElement(obj.ElementId) as WallFoundation;
                ElementId wallFoundid = wallFound.WallId;
                Element elewalllevel = doc.GetElement(wallFoundid);
                levelelement = doc.GetElement(elewalllevel.LevelId) as Level;
            }
            ElevationLevel = UnitUtils.ConvertFromInternalUnits(levelelement.Elevation, levelunit);

            return ElevationLevel;
        }
    }
}
