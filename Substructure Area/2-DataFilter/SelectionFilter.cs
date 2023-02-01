using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace Substructure_Area
{
    class SelectionFilter : ISelectionFilter
    {
        private Document _doc;

        public Document Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }




        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                || elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
            {
                FamilyInstance familyInstance = elem as FamilyInstance;
                if (familyInstance.StructuralMaterialType == StructuralMaterialType.Concrete)
                {
                    return true & null != elem; 
                }
            }
            else if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
            {
                Wall wall = elem as Wall;
                if (wall.CurtainGrid==null&&wall.StructuralUsage==StructuralWallUsage.Bearing)
                {

                    return true & null != elem;
                }
            }
            else if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
            {
                return true & null != elem;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            //return false;
            RevitLinkInstance revitlinkinstance = _doc.GetElement(reference) as RevitLinkInstance;
            Document docLink = revitlinkinstance.GetLinkDocument();
            Element eFootingLink = docLink.GetElement(reference.LinkedElementId);
            if (eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                || eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
            {
                FamilyInstance familyInstance = eFootingLink as FamilyInstance;
                if (familyInstance.StructuralMaterialType == StructuralMaterialType.Concrete)
                {
                    return true && null != eFootingLink;
                }
                
            }
            else if (eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
            {

                Wall wall = eFootingLink as Wall;
                if (wall.CurtainGrid == null && wall.StructuralUsage == StructuralWallUsage.Bearing)
                {

                    return true & null != eFootingLink;
                }

            }
            else if (eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
            {
                return true & null != eFootingLink;
            }
            return false;
        }
    }


}
