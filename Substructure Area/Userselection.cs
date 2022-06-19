using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;

namespace Substructure_Area
{
    class Userselection
    {
        private UIDocument uidoc;
        private Document doc;
        private Reference obj;
        private singleSelectionFilter SingleSelectionFilter;
        public Userselection(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            doc = uidoc.Document;
            SingleSelectionFilter = new singleSelectionFilter(doc);
        }
        public GeometryElement objec()
        {

            obj = uidoc.Selection.PickObject(ObjectType.Element, SingleSelectionFilter, "Select Footing");
           
            Element ele = doc.GetElement(obj.ElementId);
            GeometryElement geoElem = ele.GetGeometryObjectFromReference(obj) as GeometryElement;
            return geoElem;
        }
        
    }
}
