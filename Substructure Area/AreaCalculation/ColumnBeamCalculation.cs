using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Substructure_Area
{
    public class ColumnBeamCalculation
    {
        //private string faceInfo = "";

        private FormatOptions areaFormatOptions { get; set; }
        private ForgeTypeId areaUnit { get; set; }
        private DataTable table { get; set;  }
        private DataColumn header { get; set; }
        private Element ele { get; set; }

        public ColumnBeamCalculation()
        {
            
            
        }
        public DataTable Faceinfo(Element ele, GeometryElement geoElem, Document doc)
        {
            this.ele = ele;
            table = new DataTable();
            areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            areaUnit = areaFormatOptions.GetUnitTypeId();
            header = new DataColumn(ele.Name);
            table.Columns.Add(header);
            int faces = 0;
            //double totalArea = 0;
            foreach (GeometryObject geomObj in geoElem)
            {
                Solid solid = geomObj as Solid;
                if (null != solid && solid.Id != -1)
                {

                    foreach (Face geomFace in solid.Faces)
                    {

                        faces++;
                        //faceInfo += "Face " + faces + " area: " + geomFace.Area.ToString() + "\n";

                        header = new DataColumn("Face" + faces, typeof(double));
                        table.Rows.Add(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit));
                        //totalArea += geomFace.Area;

                    }
                    //faceInfo += "Number of faces: " + faces + "\n";
                    //faceInfo += "Total area: " + totalArea.ToString() + "\n";
                    var result = table.AsEnumerable().Sum(x => Convert.ToDouble(x[ele.Name]));
                    table.Rows.Add(result);
                }
            }
            
            return table;
        }
    }
}
