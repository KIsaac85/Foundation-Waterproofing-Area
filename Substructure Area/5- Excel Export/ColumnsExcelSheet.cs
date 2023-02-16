using Autodesk.Revit.DB;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substructure_Area._5__Excel_Export
{
    class ColumnsExcelSheet
    {

        #region Members
        private static ExcelWorksheet columnsSheet { get; set; }
        private static ColumnBeamCalculation Columns { get; set; }
        private static DataTable datatable { get; set; }
        private static string tableaddress { get; set; }
        private static double GrandTotal { get; set; }
        private static string lastcelladdress { get; set; }
        private static double celladdress { get; set; }
        private static IEnumerable<string> totaladdress { get; set; } 
        #endregion

        /// <summary>
        /// A function is created to add columns sheet
        /// </summary>
        /// <param name="package">the same package to save the sheet in the same file</param>
        /// <param name="columnsList">the list of columns to be published in the sheet</param>
        /// <param name="areaUnit">units used in the project, wheter metric or us</param>
        /// <returns  package inculding the added sheet</returns>

        public static ExcelPackage columnssheetcreation(ExcelPackage package, IList<Element> columnsList, ForgeTypeId areaUnit)
        {
            // Sheet is created using table that was used for data grid
            columnsSheet = package.Workbook.Worksheets.Add("Columns");
            Columns = new ColumnBeamCalculation();
            datatable = Columns.Faceinfoinstances(columnsList, areaUnit);


            if (datatable.Rows.Count!=0)
            {
                tableaddress = columnsSheet.Cells[1, 1]
            .LoadFromDataTable(datatable).Address;

                columnsSheet.Cells[tableaddress].Style.WrapText = true;
                columnsSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                columnsSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                
                //getting the address of the total area for each wall
                totaladdress = columnsSheet.Cells[tableaddress]
                 .Where(xy => xy.Value.ToString() == "Total Per Type")
                     .Select(ax => ax.Address.Replace('A', 'B'));
                //Total area calculation 
                foreach (string add in totaladdress)
                {
                    GrandTotal += double.Parse(columnsSheet.Cells[add].Value.ToString());
                }
                //address of grand total value
                lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
                celladdress = double.Parse(lastcelladdress) + 1;
                lastcelladdress = "B" + celladdress.ToString();

                // setting the value and adjusting cell borders of the grand total value
                columnsSheet.Cells[lastcelladdress].Value = GrandTotal;
                columnsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                columnsSheet.Cells[lastcelladdress].Style.WrapText = true;
                columnsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                columnsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //address of grand total string
                lastcelladdress = "A" + celladdress.ToString();

                // setting the value and adjusting cell borders of the grand total string
                columnsSheet.Cells[lastcelladdress].Value = "Grand Total";
                columnsSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                columnsSheet.Cells[lastcelladdress].Style.WrapText = true;
                columnsSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                columnsSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                double DoubleOutputBool;
                foreach (ExcelRangeBase item in columnsSheet.Cells[tableaddress])
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
                        columnsSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);


                    }

                    catch (Exception) { break; } 
                }

            }
            return package;
        }
    }
}
