using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace Substructure_Area
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class getArea : IExternalCommand
    {
        /// <summary>
        /// main entry of the application
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;


            getLevel wPF = new getLevel(uidoc);

            wPF.ShowDialog();


            return Result.Succeeded;




        }


    }
}
