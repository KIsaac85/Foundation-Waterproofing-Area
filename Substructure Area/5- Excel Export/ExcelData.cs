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
    public class ExcelData
    {
        private ExcelPackage package { get; set; }
        private ExcelWorksheet RetainingWallSheet { get; set; }
        private List<string> SelectedItems { get; set; }
        private FoundationWall foundationWall { get; set; }
        private string tableaddress { get; set; }
        private double GrandTotal { get; set; }
        private string lastcelladdress { get; set; }
        private double celladdress { get; set; }
        private IEnumerable<String> totaladdress { get; set; }

        public ExcelData()
        {
            SelectedItems = new List<string>();
            foundationWall = new FoundationWall();
        }
        public void DataTable(List<String>SelectedItems, IList<Element> WallList, ForgeTypeId areaUnit)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (package = new ExcelPackage())
            {
                if (SelectedItems.Contains("Retaining Walls"))
                {
                    //new sheet creation
                    RetainingWallSheet = package.Workbook.Worksheets.Add("Retaining Walls");

                    tableaddress= RetainingWallSheet.Cells[1 , 1]
                        .LoadFromDataTable(foundationWall.faceinfor(WallList,areaUnit)).Address;

                    RetainingWallSheet.Cells[tableaddress].Style.WrapText = true;
                    RetainingWallSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    RetainingWallSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                   
                    
                    //getting the address of the total area for each wall
                     totaladdress = RetainingWallSheet.Cells[tableaddress]
                      .Where(xy => xy.Value.ToString() == "Total")
                          .Select(ax => ax.Address.Replace('A', 'B'));
                    //Total area calculation
                    foreach (string add in totaladdress)
                    {
                        GrandTotal += double.Parse(RetainingWallSheet.Cells[add].Value.ToString());
                    }
                    lastcelladdress = totaladdress.LastOrDefault().Remove(0, 1);
                    celladdress = double.Parse(lastcelladdress) + 1;

                    lastcelladdress = "B" + celladdress.ToString();
                    RetainingWallSheet.Cells[lastcelladdress].Value = GrandTotal;
                    RetainingWallSheet.Cells[lastcelladdress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    RetainingWallSheet.Cells[lastcelladdress].Style.WrapText=true;
                    RetainingWallSheet.Cells[lastcelladdress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    RetainingWallSheet.Cells[lastcelladdress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    lastcelladdress = "A" + celladdress.ToString();
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

                            if (double.TryParse((string)item.Value, out DoubleOutputBool) ==true)
                            {
                                item.Value = double.Parse((string)item.Value);
                                item.Style.Border.BorderAround(ExcelBorderStyle.Thin);  
                            }
                            else if (double.TryParse((string)item.Value, out DoubleOutputBool) == false)
                            {
                                item.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                            }
                            RetainingWallSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                            
                            
                        }

                        catch (Exception) { break; }
                        
                    }
                    



                    SaveFileDialog saveFile = new SaveFileDialog
                    {
                        FileName = "NewSheet", // Default file name
                        DefaultExt = ".xlsx", // Default file extension
                        Filter = "Excel Sheet (.xlsx)|*" // Filter files by extension
                    };
                    bool? result = saveFile.ShowDialog();
                    string errormessage = null;
                    do
                    {
                        try
                        {
                            saveFile.OverwritePrompt = true;
                            savedialogue(package, saveFile);
                        }
                        
                        catch (Exception e)
                        {
                            if (result != false)
                            {
                                errormessage = e.Message;
                                TaskDialog.Show(e.Message, "The file can not be saved. Please close the file and try again.");
                                break;
                            }

                        }
                    } while (result!=false && errormessage!=null);
                }
            }
        }

        private void savedialogue(ExcelPackage package, SaveFileDialog saveFile)
        {
            FileInfo filename = new FileInfo(saveFile.FileName);
            package.SaveAs(filename);
            Process.Start(Path.Combine(filename.Directory.ToString(), filename.ToString()));
        }


    }
}
