using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
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
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation
                || elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                || elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
                || elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
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
            if (eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation
                || eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
                || eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
                || eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
            {
                return true && null != eFootingLink;
            }
            return false;
        }
    }


}
