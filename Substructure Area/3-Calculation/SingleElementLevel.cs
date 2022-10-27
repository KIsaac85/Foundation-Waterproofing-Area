﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Substructure_Area
{
    class SingleElementLevel
    {


        private static Element Element;
		private static Level levelelement;
		private static double ElevationLevel;
        private static FormatOptions levelFormatOptions;
        private static ForgeTypeId levelunit;
		
        public static Double ElementLevelCalculation(Reference obj, Document doc)
        {
            levelFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
            levelunit = levelFormatOptions.GetUnitTypeId();
			Element = doc.GetElement(obj.ElementId);

			if (Element.LevelId.IntegerValue != -1)
			{

				ElementId levelid = Element.LevelId;
				levelelement = doc.GetElement(levelid) as Level;
			}

			else if (Element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && Element.LevelId.IntegerValue == -1)
			{
				FamilyInstance beam = doc.GetElement(obj.ElementId) as FamilyInstance;
				levelelement = beam.Host as Level;
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
