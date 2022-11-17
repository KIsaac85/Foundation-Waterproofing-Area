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
        

        private FormatOptions areaFormatOptions { get; set; }
        private ForgeTypeId areaUnit { get; set; }
        private DataTable table { get; set;  }
        private DataColumn header { get; set; }
        private DataColumn header2 { get; set; }
        private DataRow rowData { get; set; }
        private Element ele { get; set; }


        public DataTable Faceinfo(Element ele, GeometryElement geoElem, Document doc)
        {
            this.ele = ele;
            table = new DataTable();
            areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            areaUnit = areaFormatOptions.GetUnitTypeId();

            header = new DataColumn("Faces");
            header2 = new DataColumn(ele.Name);
            table.Columns.Add(header);
            table.Columns.Add(header2);

            int faces = 0;
            foreach (GeometryObject geomObj in geoElem)
            {
                Solid solid = geomObj as Solid;
                if (null != solid && solid.Id != -1)
                {

                    foreach (Face geomFace in solid.Faces)
                    {
                        faces++;
                        rowData = table.NewRow();
                        rowData[header] = faces;
                        rowData[header2] = Math.Round(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit),2);
                        table.Rows.Add(rowData);
                    }
                    double result = table.AsEnumerable().Sum(x => Convert.ToDouble(x[ele.Name]));
                    rowData = table.NewRow();
                    rowData[header] = "Total";
                    rowData[header2] = result;
                    table.Rows.Add(rowData);
                }
            }
            return table;
        }
    }
}
