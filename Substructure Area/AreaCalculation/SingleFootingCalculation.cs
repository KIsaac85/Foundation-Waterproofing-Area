using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Caliburn.Micro;



namespace Substructure_Area
{
    public class SingleFootingCalculation
    {
        private string faceInfo { get; set; }
        private Element ele { get; set; }
        private int faces { get; set; }
        private double totalArea { get; set; }
        private DataTable table { get; set; }
        private DataColumn header { get; set; }
        private DataColumn header2 { get; set; }
        private DataRow rowData { get; set; }
        private static List<string> infofaces { get; set; }
        private GeometryInstance geoInst { get; set; }
        public SingleFootingCalculation()

        {
            infofaces = new List<string>();
        }
        
        public DataTable faceinfor(Element ele, GeometryElement geoElem, Document doc)
        {

            FormatOptions areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            ForgeTypeId areaUnit = areaFormatOptions.GetUnitTypeId();
            table = new DataTable();

            this.ele = ele;
            header = new DataColumn("Faces");
            header2 = new DataColumn(ele.Name);
            table.Columns.Add(header);
            table.Columns.Add(header2);
            int count=0;
            //isolated footing data
            foreach (GeometryObject geomObj in geoElem)
            {
                
                geoInst = geomObj as GeometryInstance;
               
                if (null != geoInst)
                {
                    count++;
                    foreach (Solid geoSolid in geoInst.SymbolGeometry)
                    {
                        if (null != geoSolid && geoSolid.Id != -1)
                        {
                            faces = 0;
                            foreach (Face geomFace in geoSolid.Faces)
                            {
                                {
                                    faces++;
                                    rowData = table.NewRow();
                                    rowData[header] = faces;
                                    rowData[header2] = Math.Round(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit),2);
                                    table.Rows.Add(rowData);
                                }
                            }
                            double result = table.AsEnumerable().Sum(x => Convert.ToDouble(x[ele.Name]));
                            rowData = table.NewRow();
                            rowData[header] = "Total";
                            rowData[header2] = result;
                            table.Rows.Add(rowData);
                        }
                    }
                }

                //strip footing/raft data
                else if(null == geoInst)
                {
                    Solid geoSolid = geomObj as Solid;
                    if (null != geoSolid&&geoSolid.Id!=-1)
                    {
                        faces = 0;
                        foreach (Face geoface in geoSolid.Faces)
                        {
                            if (null != geoface && geoface.Id != -1)
                            {
                                faces++;
                                rowData = table.NewRow();
                                rowData[header] = faces;
                                rowData[header2] = Math.Round(UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit), 2);
                                table.Rows.Add(rowData);
                            }
                        }
                        double result = table.AsEnumerable().Sum(x => Convert.ToDouble(x[ele.Name]));
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
