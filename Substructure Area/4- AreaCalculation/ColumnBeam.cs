using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;

namespace Substructure_Area
{
    public class ColumnBeamCalculation
    {


        private List<int> instanceID { get; set; }
        private Options option { get; set; }
        private Solid solid { get; set; }
        private GeometryElement geoElem { get; set; }
        private DataTable table { get; set;  }
        private DataColumn header { get; set; }
        private DataColumn header2 { get; set; }
        private DataRow rowData { get; set; }
        private int faces { get; set; }
        private double result { get; set; }
        public ColumnBeamCalculation()
        {
            instanceID = new List<int>();
            option = new Options();
        }
        public DataTable Faceinfo(Element ele, ForgeTypeId areaUnit)
        {
            
            table = new DataTable();
            
            
            geoElem = ele.get_Geometry(option);
            header = new DataColumn("Faces");
            header2 = new DataColumn(ele.Name);
            table.Columns.Add(header);
            table.Columns.Add(header2);

            
            foreach (GeometryObject geomObj in geoElem)
            {

                solid = geomObj as Solid;
                if (null != solid && solid.Id != -1)
                {
                    faces = 0;
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
                else if (null == solid)
                {
                    GeometryInstance geoInst = geomObj as GeometryInstance;

                    if (null != geoInst)
                    {

                        foreach (Solid geoSolid in geoInst.SymbolGeometry)
                        {
                            if (null != geoSolid)
                            {
                                faces = 0;
                                foreach (Face geomFace in geoSolid.Faces)
                                {

                                    faces++;
                                    rowData = table.NewRow();
                                    rowData[header] = faces;
                                    rowData[header2] = Math.Round(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit), 2);
                                    table.Rows.Add(rowData);

                                }
                                if (geoSolid.SurfaceArea != 0)
                                {
                                    result = Math.Round(UnitUtils.ConvertFromInternalUnits(geoSolid.SurfaceArea, areaUnit), 2);
                                    rowData = table.NewRow();
                                    rowData[header] = "Total";
                                    rowData[header2] = result;
                                    table.Rows.Add(rowData);
                                }
                            }
                        }
                    }
                }
            }
            return table;
        }

        public DataTable Faceinfo(IList<Element> ele, ForgeTypeId areaUnit)
        {

            table = new DataTable();
            
            header = new DataColumn();
            
            header2 = new DataColumn();
            table.Columns.Add(header);
            table.Columns.Add(header2);

            foreach (var item in ele)
            {
                geoElem = item.get_Geometry(option);
                if (!instanceID.Contains(item.Id.IntegerValue))
                {
                    foreach (GeometryObject geomObj in geoElem)
                    {

                        solid = geomObj as Solid;
                        if (null != solid && solid.Id != -1)
                        {
                            faces = 0;
                            rowData = table.NewRow();
                            rowData[header] = "face";
                            rowData[header2] = item.Name;
                            table.Rows.Add(rowData);
                            foreach (Face geomFace in solid.Faces)
                            {
                                faces++;
                                rowData = table.NewRow();

                                rowData[header] = faces;
                                rowData[header2] = Math.Round(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit), 2);
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
                        else if (null == solid)
                        {
                            GeometryInstance geoInst = geomObj as GeometryInstance;

                            if (null != geoInst)
                            {

                                foreach (Solid geoSolid in geoInst.SymbolGeometry)
                                {
                                    if (null != geoSolid)
                                    {
                                        faces = 0;
                                        foreach (Face geomFace in geoSolid.Faces)
                                        {

                                            faces++;
                                            rowData = table.NewRow();
                                            rowData[header] = "face";
                                            rowData[header2] = item.Name;
                                            rowData[header] = faces;
                                            rowData[header2] = Math.Round(UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit), 2);
                                            table.Rows.Add(rowData);

                                        }
                                        if (geoSolid.SurfaceArea != 0)
                                        {
                                            result = Math.Round(UnitUtils.ConvertFromInternalUnits(geoSolid.SurfaceArea, areaUnit), 2);
                                            rowData = table.NewRow();
                                            rowData[header] = "Total";
                                            rowData[header2] = result;
                                            table.Rows.Add(rowData);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return table;
        }
    }
}
