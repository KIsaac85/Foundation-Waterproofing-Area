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
        private Solid solid { get; set; }
        private DataTable table { get; set;  }
        private DataColumn header { get; set; }
        private DataColumn header2 { get; set; }
        private DataRow rowData { get; set; }
        private int faces { get; set; }
        private double result { get; set; }
        public DataTable Faceinfo(Element ele, GeometryElement geoElem, Document doc)
        {
            
            table = new DataTable();
            areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            areaUnit = areaFormatOptions.GetUnitTypeId();

            header = new DataColumn("Faces");
            header2 = new DataColumn(ele.Name);
            table.Columns.Add(header);
            table.Columns.Add(header2);

            
            foreach (GeometryObject geomObj in geoElem)
            {
                solid = geomObj as Solid;
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
                    if (solid.SurfaceArea != 0)
                    {
                        result = Math.Round(UnitUtils.ConvertFromInternalUnits(solid.SurfaceArea, areaUnit), 2);
                        rowData = table.NewRow();
                        rowData[header] = "Total";
                        rowData[header2] = result;
                        table.Rows.Add(rowData);
                    }
                    
                }
            }
            return table;
        }
    }
}
