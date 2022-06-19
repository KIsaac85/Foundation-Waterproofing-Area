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
    class singleSelectionFilter : ISelectionFilter
    {
        private Document doc;
        public singleSelectionFilter(Document doc)
        {
            this.doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            //return false;
            RevitLinkInstance revitlinkinstance = doc.GetElement(reference) as RevitLinkInstance;
            Document docLink = revitlinkinstance.GetLinkDocument();
            Element eFootingLink = docLink.GetElement(reference.LinkedElementId);
            if (eFootingLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
            {
                return true;
            }
            return false;
        }
    }


}
