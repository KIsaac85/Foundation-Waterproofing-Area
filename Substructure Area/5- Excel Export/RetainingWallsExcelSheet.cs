using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Substructure_Area._5__Excel_Export
{
    class RetainingWallsExcelSheet
    {

        private static ExcelWorksheet RetainingWallSheet { get; set; }

        private static FoundationWall foundationWall { get; set; }
        private static string tableaddress { get; set; }
        private static double GrandTotal { get; set; }
        private static string lastcelladdress { get; set; }
        private static double celladdress { get; set; }
        private static IEnumerable<String> totaladdress { get; set; }

        public static ExcelPackage retainingwallsheetcreation(ExcelPackage package, IList<Element> WallList, ForgeTypeId areaUnit)
        {
            RetainingWallSheet = package.Workbook.Worksheets.Add("Retaining Walls");
            foundationWall = new FoundationWall();

            tableaddress = RetainingWallSheet.Cells[1, 1]
                .LoadFromDataTable(foundationWall.faceinfotype(WallList, areaUnit)).Address;

            RetainingWallSheet.Cells[tableaddress].Style.WrapText = true;
            RetainingWallSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            RetainingWallSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            //getting the address of the total area for each wall
            totaladdress = RetainingWallSheet.Cells[tableaddress]
             .Where(xy => xy.Value.ToString() == "Total Per Type")
                 .Select(ax => ax.Address.Replace('A', 'B'));
            //Total area calculation 
            foreach (string add in totaladdress)
            {
                GrandTotal += double.Parse(RetainingWallSheet.Cells[add].Value.ToString());
            }
            //address of grand total value
            lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
            celladdress = double.Parse(lastcelladdress) + 1;
            lastcelladdress = "B" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total value
            RetainingWallSheet.Cells[lastcelladdress].Value = GrandTotal;
            RetainingWallSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            RetainingWallSheet.Cells[lastcelladdress].Style.WrapText = true;
            RetainingWallSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            RetainingWallSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //address of grand total string
            lastcelladdress = "A" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total string
            RetainingWallSheet.Cells[lastcelladdress].Value = "Grand Total";
            RetainingWallSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            RetainingWallSheet.Cells[lastcelladdress].Style.WrapText = true;
            RetainingWallSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            RetainingWallSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            double DoubleOutputBool;
            foreach (ExcelRangeBase item in RetainingWallSheet.Cells[tableaddress])
            {
                try
                {
                    //converting text in cells to doubles
                    //first check if it was possible and adjusting the borders
                    if (double.TryParse((string)item.Value, out DoubleOutputBool) == true)
                    {
                        item.Value = double.Parse((string)item.Value);
                        item.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    //if not keep it as it is but adjusting borders only
                    else if (double.TryParse((string)item.Value, out DoubleOutputBool) == false)
                    {
                        item.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    }
                    RetainingWallSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);


                }

                catch (Exception) { break; }

            }
            return package;
        }
    }
}
