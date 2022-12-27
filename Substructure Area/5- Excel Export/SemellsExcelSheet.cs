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
    class SemellsExcelSheet
    {
        private static ExcelWorksheet beamsSheet { get; set; }

        private static ColumnBeamCalculation Beams { get; set; }
        private static string tableaddress { get; set; }
        private static double GrandTotal { get; set; }
        private static string lastcelladdress { get; set; }
        private static double celladdress { get; set; }
        private static IEnumerable<String> totaladdress { get; set; }

        public static ExcelPackage beamssheetcreation(ExcelPackage package, IList<Element> columnsList, ForgeTypeId areaUnit)
        {
            beamsSheet = package.Workbook.Worksheets.Add("Columns");
            Beams = new ColumnBeamCalculation();

            tableaddress = beamsSheet.Cells[1, 1]
                .LoadFromDataTable(Beams.Faceinfo(columnsList, areaUnit)).Address;

            beamsSheet.Cells[tableaddress].Style.WrapText = true;
            beamsSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            beamsSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            //getting the address of the total area for each wall
            totaladdress = beamsSheet.Cells[tableaddress]
             .Where(xy => xy.Value.ToString() == "Total")
                 .Select(ax => ax.Address.Replace('A', 'B'));
            //Total area calculation 
            foreach (string add in totaladdress)
            {
                GrandTotal += double.Parse(beamsSheet.Cells[add].Value.ToString());
            }
            //address of grand total value
            lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
            celladdress = double.Parse(lastcelladdress) + 1;
            lastcelladdress = "B" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total value
            beamsSheet.Cells[lastcelladdress].Value = GrandTotal;
            beamsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            beamsSheet.Cells[lastcelladdress].Style.WrapText = true;
            beamsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            beamsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //address of grand total string
            lastcelladdress = "A" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total string
            beamsSheet.Cells[lastcelladdress].Value = "Grand Total";
            beamsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            beamsSheet.Cells[lastcelladdress].Style.WrapText = true;
            beamsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            beamsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            double DoubleOutputBool;
            foreach (ExcelRangeBase item in beamsSheet.Cells[tableaddress])
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
                    beamsSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);


                }

                catch (Exception) { break; }

            }
            return package;
        }
    }
}
