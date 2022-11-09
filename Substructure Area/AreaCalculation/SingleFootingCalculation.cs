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
        public string faceInfo { get; set; }
        public Element ele { get; set; }
        public int faces { get; set; }
        public double totalArea { get; set; }
        public DataTable table { get; set; }
        public DataColumn header { get; set; }
        public static List<string> infofaces { get; set; }
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
            header = new DataColumn(ele.Name);
            
            table.Columns.Add(header);
            foreach (GeometryObject geomObj in geoElem)
            {
                
                GeometryInstance geoInst = geomObj as GeometryInstance;
                
                //isolated footing data
                if (null != geoInst)
                {

                    foreach (Solid geoSolid in geoInst.SymbolGeometry)
                    {

                        if (null != geoSolid && geoSolid.Id != -1)
                        {
                            
                            foreach (Face geomFace in geoSolid.Faces)
                            {
                                //for (int i = 0; i < faces; i++)
                                {
                                    faces++;
                                    faceInfo += "Face " + faces + " area: " + UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit).ToString() + "\n";
                                    infofaces.Add(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit).ToString());
                                    table.Rows.Add(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit));
                                    
                                    totalArea += geomFace.Area;
                                    
                                }
                                
                            }
                                var result = table.AsEnumerable().Sum(x => Convert.ToDouble(x[ele.Name]));
                                table.Rows.Add(result);
                            
                        

                            faceInfo += "Number of faces: " + faces + "\n";
                            faceInfo += "Total area: " + UnitUtils.ConvertFromInternalUnits(totalArea, areaUnit).ToString() + "\n";
                            

                        }
                    }
                }

                //strip footing/raft data
                else if(null == geoInst)
                {
                    Solid geoSolid = geomObj as Solid;
                    if (null != geoSolid)
                    {
                        foreach (Face geoface in geoSolid.Faces)
                        {
                            if (null != geoface && geoface.Id != -1)
                            {
                                infofaces.Add(UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit).ToString());
                                faces++;
                                faceInfo += "Face " + faces + " area: " + UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit).ToString() + "\n";

                                totalArea += geoface.Area;

                                //faceInfo += "Number of faces: " + faces + "\n";
                                header = new DataColumn("Face" + faces, typeof(double));
                                //table.Columns.Add(header);
                                table.Rows.Add(UnitUtils.ConvertFromInternalUnits(geoface.Area, areaUnit));
                                faceInfo += "Total area: " + UnitUtils.ConvertFromInternalUnits(totalArea, areaUnit).ToString() + "\n";
                            }
                        }
                        faceInfo += "Number of faces: " + faces + "\n";
                    }
                }
            }
            
           
            
            return table;
        }
    }
}
