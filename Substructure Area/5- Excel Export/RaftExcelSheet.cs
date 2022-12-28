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
    class RaftExcelSheet
    {
        private static ExcelWorksheet RaftFootingsSheet { get; set; }

        private static FoundationWall RaftFootings { get; set; }
        private static string tableaddress { get; set; }
        private static double GrandTotal { get; set; }
        private static string lastcelladdress { get; set; }
        private static double celladdress { get; set; }
        private static IEnumerable<String> totaladdress { get; set; }

        public static ExcelPackage RaftFootingssheetcreation(ExcelPackage package, IList<Element> RaftFootingsList, ForgeTypeId areaUnit)
        {
            RaftFootingsSheet = package.Workbook.Worksheets.Add("Raft Foundation");
            RaftFootings = new FoundationWall();

            tableaddress = RaftFootingsSheet.Cells[1, 1]
                .LoadFromDataTable(RaftFootings.faceinfor(RaftFootingsList, areaUnit)).Address;

            RaftFootingsSheet.Cells[tableaddress].Style.WrapText = true;
            RaftFootingsSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            RaftFootingsSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            //getting the address of the total area for each wall
            totaladdress = RaftFootingsSheet.Cells[tableaddress]
             .Where(xy => xy.Value.ToString() == "Total")
                 .Select(ax => ax.Address.Replace('A', 'B'));
            //Total area calculation 
            foreach (string add in totaladdress)
            {
                GrandTotal += double.Parse(RaftFootingsSheet.Cells[add].Value.ToString());
            }
            //address of grand total value
            lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
            celladdress = double.Parse(lastcelladdress) + 1;
            lastcelladdress = "B" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total value
            RaftFootingsSheet.Cells[lastcelladdress].Value = GrandTotal;
            RaftFootingsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            RaftFootingsSheet.Cells[lastcelladdress].Style.WrapText = true;
            RaftFootingsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            RaftFootingsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //address of grand total string
            lastcelladdress = "A" + celladdress.ToString();

            // setting the value and adjusting cell borders of the grand total string
            RaftFootingsSheet.Cells[lastcelladdress].Value = "Grand Total";
            RaftFootingsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            RaftFootingsSheet.Cells[lastcelladdress].Style.WrapText = true;
            RaftFootingsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            RaftFootingsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            double DoubleOutputBool;
            foreach (ExcelRangeBase item in RaftFootingsSheet.Cells[tableaddress])
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
                    RaftFootingsSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);


                }

                catch (Exception) { break; }

            }
            return package;
        }
    }
}
