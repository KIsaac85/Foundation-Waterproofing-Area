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
    class FootingsExcelSheet
    {
        private static ExcelWorksheet IsolatedFootingsSheet { get; set; }

        private static FoundationWall IsolatedFootings { get; set; }
        private static string tableaddress { get; set; }
        private static double GrandTotal { get; set; }
        private static string lastcelladdress { get; set; }
        private static double celladdress { get; set; }
        private static IEnumerable<String> totaladdress { get; set; }


        public static ExcelPackage IsolatedFootingssheetcreation(ExcelPackage package, IList<Element> IsolatedFootingsList, ForgeTypeId areaUnit)
        {
            IsolatedFootingsSheet = package.Workbook.Worksheets.Add("Isolated Footings");
            IsolatedFootings = new FoundationWall();

            tableaddress = IsolatedFootingsSheet.Cells[1, 1]
                .LoadFromDataTable(IsolatedFootings.faceinfoinstances(IsolatedFootingsList, areaUnit)).Address;

            IsolatedFootingsSheet.Cells[tableaddress].Style.WrapText = true;
            IsolatedFootingsSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            IsolatedFootingsSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            //getting the address of the total area for each wall
            totaladdress = IsolatedFootingsSheet.Cells[tableaddress]
             .Where(xy => xy.Value.ToString() == "Total Per Type")
                 .Select(ax => ax.Address.Replace('A', 'B'));
            //Total area calculation 
            foreach (string add in totaladdress)
            {
                GrandTotal += double.Parse(IsolatedFootingsSheet.Cells[add].Value.ToString());
            }
            //address of grand total value
            lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
            celladdress = double.Parse(lastcelladdress) + 1;
            lastcelladdress = "B" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total value
            IsolatedFootingsSheet.Cells[lastcelladdress].Value = GrandTotal;
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.WrapText = true;
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //address of grand total string
            lastcelladdress = "A" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total string
            IsolatedFootingsSheet.Cells[lastcelladdress].Value = "Grand Total";
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.WrapText = true;
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            IsolatedFootingsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            foreach (ExcelRangeBase item in IsolatedFootingsSheet.Cells[tableaddress])
            {
                try
                {
                    //converting text in cells to doubles
                    //first check if it was possible and adjusting the borders
                    if (double.TryParse((string)item.Value, out double DoubleOutputBool) == true)
                    {
                        item.Value = double.Parse((string)item.Value);
                        item.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    //if not keep it as it is but adjusting borders only
                    else if (double.TryParse((string)item.Value, out DoubleOutputBool) == false)
                    {
                        item.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    }
                    IsolatedFootingsSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);


                }

                catch (Exception) { break; }

            }
            return package;
        }
    }
}
