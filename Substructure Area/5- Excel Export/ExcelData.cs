using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Substructure_Area._5__Excel_Export
{
    public class ExcelData
    {
        private List<string> SelectedItems { get; set; }
        private FoundationWall foundationWall { get; set; }
        public ExcelData()
        {
            SelectedItems = new List<string>();
            foundationWall = new FoundationWall();
        }
        public void DataTable(List<String>SelectedItems, IList<Element> WallList, ForgeTypeId areaUnit)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                if (SelectedItems.Contains("Retaining Walls"))
                {
                    
                    ExcelWorksheet RetainingWallSheet = package.Workbook.Worksheets.Add("Retaining Walls");

                    string tableaddress= RetainingWallSheet.Cells[1 , 1]
                        .LoadFromDataTable(foundationWall.faceinfor(WallList,areaUnit)).Address;

                    RetainingWallSheet.Cells[tableaddress].Style.WrapText = true;
                    RetainingWallSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    RetainingWallSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                   
                    double x;
                    double GrandTotal = 0;
                    var cel = RetainingWallSheet.Cells[tableaddress]
                      .Where(xy => xy.Value.ToString() == "Total")
                          .Select(ax => ax.Address.Replace('A', 'B'));
                    string lastcelladdress = cel.LastOrDefault().Remove(0, 1);
                    double celladdres = double.Parse(lastcelladdress) + 1;
                    lastcelladdress = "B" + celladdres.ToString();
                    //TaskDialog.Show("total", lastcelladdress);
                    RetainingWallSheet.Cells[lastcelladdress].Value = GrandTotal;
                    lastcelladdress = "A" + celladdres.ToString();
                    RetainingWallSheet.Cells[lastcelladdress].Value = "Grand Total";
                    
                    foreach (string add in cel)
                    {
                        //TaskDialog.Show("", add);
                        GrandTotal += double.Parse(RetainingWallSheet.Cells[add].Value.ToString());
                    }
                    
                    foreach (ExcelRangeBase item in RetainingWallSheet.Cells[tableaddress])
                    {
                        try
                        {

                            if (double.TryParse((string)item.Value, out x)==true)
                            {
                                item.Value = double.Parse((string)item.Value);
                                item.Style.Border.BorderAround(ExcelBorderStyle.Thin);  
                            }
                            else if (double.TryParse((string)item.Value, out x) == false)
                            {
                                item.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                            }
                            RetainingWallSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                            
                            
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
