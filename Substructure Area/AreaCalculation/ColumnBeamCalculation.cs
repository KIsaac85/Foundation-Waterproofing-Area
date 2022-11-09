﻿using System;
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
        private string faceInfo = "";

        private FormatOptions areaFormatOptions { get; set; }
        private ForgeTypeId areaUnit { get; set; }
        private DataTable table { get; set;  }

        public ColumnBeamCalculation()
        {
            table = new DataTable();
        }
        public string Faceinfo(GeometryElement geoElem, Document doc)
        {

            areaFormatOptions = doc.GetUnits().GetFormatOptions(SpecTypeId.Area);
            areaUnit = areaFormatOptions.GetUnitTypeId();


            int faces = 0;
            double totalArea = 0;
            foreach (GeometryObject geomObj in geoElem)
            {
                Solid solid = geomObj as Solid;
                if (null != solid && solid.Id != -1)
                {

                    foreach (Face geomFace in solid.Faces)
                    {

                        faces++;
                        faceInfo += "Face " + faces + " area: " + geomFace.Area.ToString() + "\n";

                        totalArea += geomFace.Area;

                    }
                    faceInfo += "Number of faces: " + faces + "\n";
                    faceInfo += "Total area: " + totalArea.ToString() + "\n";
                }
            }
            
            return faceInfo;
        }
    }
}
