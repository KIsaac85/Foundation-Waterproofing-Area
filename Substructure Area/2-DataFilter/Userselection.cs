using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Diagnostics;

namespace Substructure_Area
{
    class Userselection
    {


        private static UIDocument _uidoc;
        private Document _doc;
        private  Reference obj;
        private static SelectionFilter SingleSelectionFilter;
        public double Elevation { get; set; }
        public double UserElevation { get; set; }


        public Userselection(UIDocument uidoc)
        {
            _uidoc = uidoc;
            _doc = uidoc.Document;
            
            UserElevation = getLevel.Userinput;
            
            SingleSelectionFilter = new SelectionFilter();
           


        }
        public  Reference Object()
        {
            
            
                
            
            do
            {
                try
                {
                    obj = _uidoc.Selection.PickObject(ObjectType.Element, SingleSelectionFilter);
                }
                catch (OperationCanceledException e)
                {

                    return null;
                }
            } while (obj!=null);
            
          
            return obj;

            
                

           

            
        }
        
    }
}
