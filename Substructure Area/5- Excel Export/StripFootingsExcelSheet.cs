using Autodesk.Revit.DB;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substructure_Area._5__Excel_Export
{
    class StripFootingsExcelSheet
    {
        private static ExcelWorksheet StripFootingsSheet { get; set; }

        private static FoundationWall StripFootings { get; set; }
        private static string tableaddress { get; set; }
        private static double GrandTotal { get; set; }
        private static string lastcelladdress { get; set; }
        private static double celladdress { get; set; }

        private static IEnumerable<String> totaladdress { get; set; }

        public static ExcelPackage StripFootingssheetcreation(ExcelPackage package, IList<Element> IsolatedFootingsList, ForgeTypeId areaUnit)
        {
            StripFootingsSheet = package.Workbook.Worksheets.Add("Strip Footings");
            StripFootings = new FoundationWall();

            tableaddress = StripFootingsSheet.Cells[1, 1]
                .LoadFromDataTable(StripFootings.faceinfotype(IsolatedFootingsList, areaUnit)).Address;

            StripFootingsSheet.Cells[tableaddress].Style.WrapText = true;
            StripFootingsSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            StripFootingsSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            //getting the address of the total area for each wall
            totaladdress = StripFootingsSheet.Cells[tableaddress]
             .Where(xy => xy.Value.ToString() == "Total Per Type")
                 .Select(ax => ax.Address.Replace('A', 'B'));
            //Total area calculation 
            foreach (string add in totaladdress)
            {
                GrandTotal += double.Parse(StripFootingsSheet.Cells[add].Value.ToString());
            }
            //address of grand total value
            lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
            celladdress = double.Parse(lastcelladdress) + 1;
            lastcelladdress = "B" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total value
            StripFootingsSheet.Cells[lastcelladdress].Value = GrandTotal;
            StripFootingsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            StripFootingsSheet.Cells[lastcelladdress].Style.WrapText = true;
            StripFootingsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            StripFootingsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //address of grand total string
            lastcelladdress = "A" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total string
            StripFootingsSheet.Cells[lastcelladdress].Value = "Grand Total";
            StripFootingsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            StripFootingsSheet.Cells[lastcelladdress].Style.WrapText = true;
            StripFootingsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            StripFootingsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            double DoubleOutputBool;
            foreach (ExcelRangeBase item in StripFootingsSheet.Cells[tableaddress])
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
                    StripFootingsSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);


                }

                catch (Exception) { break; }

            }
            return package;
        }
    }
}
