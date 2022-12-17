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

                    
                    
                    string tableaddress= RetainingWallSheet.Cells[1 , 1].LoadFromDataTable(foundationWall.faceinfor(WallList,areaUnit)).Address;
                   
                    
                    RetainingWallSheet.Cells[tableaddress].Style.WrapText = true;
                    RetainingWallSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    RetainingWallSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                   
                    double x;
                    double totalvalue=0;
                    StringBuilder sb = new StringBuilder();
                    foreach (ExcelRangeBase item in RetainingWallSheet.Cells[tableaddress])
                    {
                        try
                        {
                            if (double.TryParse((string)item.Value, out x)==true)
                            {
                                item.Value = double.Parse((string)item.Value);
                                item.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                sb.AppendLine(item.Address);
                                if (true)
                                {

                                }
                                totalvalue ++;
                            }
                            else if (double.TryParse((string)item.Value, out x) == false)
                            {
                                item.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                            }
                            RetainingWallSheet.Cells[tableaddress].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                        }
                        catch (Exception) { break; }
                        
                    }
                    //TaskDialog.Show("tableaddress", tableaddress);
                    TaskDialog.Show("total", sb.ToString());
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
