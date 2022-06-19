
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace Substructure_Area
{
    [TransactionAttribute(TransactionMode.Manual)]
    class getArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //UI doc
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            UserControl1 wPF = new UserControl1(uidoc);
            
            wPF.ShowDialog();
            
            //var x = wPF.Elements_Selection;
            return Result.Succeeded;

            
            
        }
        
       
    }
}
