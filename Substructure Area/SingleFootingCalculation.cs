using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Substructure_Area
{
    class SingleFootingCalculation
    {
        private string faceInfo="";
        public string faceinfor(GeometryElement geoElem, Document doc)
        {

            FormatOptions areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            ForgeTypeId areaUnit = areaFormatOptions.GetUnitTypeId();


            int faces = 0;
            double totalArea = 0;
            foreach (GeometryObject geomObj in geoElem)
            {

                GeometryInstance geoInst = geomObj as GeometryInstance;
                if (null != geoInst)
                {

                    foreach (Solid geoSolid in geoInst.SymbolGeometry)
                    {
                        if (null != geoSolid && geoSolid.Id != -1)
                        {
                            foreach (Face geomFace in geoSolid.Faces)
                            {
                                faces++;
                                faceInfo += "Face " + faces + " area: " + UnitUtils.ConvertFromInternalUnits(geomFace.Area, areaUnit).ToString() + "\n";

                                totalArea += geomFace.Area;
                            }
                            faceInfo += "Number of faces: " + faces + "\n";
                            faceInfo += "Total area: " + UnitUtils.ConvertFromInternalUnits(totalArea, areaUnit).ToString() + "\n";
                        }
                    }

                }

            }
            TaskDialog.Show("Face", faceInfo);
            return faceInfo;
        }
    }
}
