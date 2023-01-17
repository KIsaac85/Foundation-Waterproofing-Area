
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using System;

namespace Substructure_Area
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class getArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;


            getLevel wPF = new getLevel(uidoc);

            wPF.ShowDialog();


            return Result.Succeeded;




        }


    }
}
