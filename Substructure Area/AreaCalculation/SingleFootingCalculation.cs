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
            foreach (GeometryObject geomObj in geoElem)
            {
                geoInst = geomObj as GeometryInstance;
                //isolated footing data
                if (null != geoInst)
                {
                    foreach (Solid geoSolid in geoInst.SymbolGeometry)
                    {
                        if (null != geoSolid && geoSolid.Id != -1)
                        {
                            faces = 0;
                            foreach (Face geomFace in geoSolid.Faces)
                            {
                                //for (int i = 0; i < faces; i++)
                                {
                                    
                                    faces++;
                                    rowData = table.NewRow();
                                    rowData[header] = faces;
                                    rowData[header2] = UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit);
                                    table.Rows.Add(rowData);
                                    //faceInfo += "Face " + faces + " area: " + UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit).ToString() + "\n";
                                    //infofaces.Add(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit).ToString());
                                    //table.Rows.Add(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit));
                                    
                                    //totalArea += geomFace.Area;
                                }
                                
                            }

                            double result = table.AsEnumerable().Sum(x => Convert.ToDouble(x[ele.Name]));
                            rowData = table.NewRow();
                            rowData[header] = "Total";
                            rowData[header2] = result;
                            table.Rows.Add(rowData);



                            //faceInfo += "Number of faces: " + faces + "\n";
                            //faceInfo += "Total area: " + UnitUtils.ConvertFromInternalUnits(totalArea, areaUnit).ToString() + "\n";
                            

                        }
                    }
                }

                //strip footing/raft data
                else if(null == geoInst)
                {
                    Solid geoSolid = geomObj as Solid;
                    if (null != geoSolid)
                    {
                        faces = 0;
                        foreach (Face geoface in geoSolid.Faces)
                        {
                            if (null != geoface && geoface.Id != -1)
                            {
                                //infofaces.Add(UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit).ToString());
                                
                                faces++;
                                
                                rowData = table.NewRow();
                                rowData[header] = faces;
                                rowData[header2] = UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit);
                                table.Rows.Add(rowData);
                                //faceInfo += "Face " + faces + " area: " + UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit).ToString() + "\n";

                                //totalArea += geoface.Area;

                                //faceInfo += "Number of faces: " + faces + "\n";
                                //header = new DataColumn("Face" + faces, typeof(double));
                                //table.Columns.Add(header);
                                //table.Rows.Add(UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit));
                                //faceInfo += "Total area: " + UnitUtils.ConvertFromInternalUnits(totalArea, areaUnit).ToString() + "\n";
                            }
                        }
                        //faceInfo += "Number of faces: " + faces + "\n";
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
