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
                   
                    RetainingWallSheet.Cells[tableaddress].Worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    RetainingWallSheet.Cells[tableaddress].Style.WrapText = true;
                    RetainingWallSheet.Cells[tableaddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    RetainingWallSheet.Cells[tableaddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //RetainingWallSheet.Cells[tableaddress].Value = Convert.ToDouble(RetainingWallSheet.Cells[tableaddress]);
                    double x;
                    foreach (ExcelRangeBase item in RetainingWallSheet.Cells[tableaddress].Worksheet.Cells)
                    {
                        
                        try
                        {
                            //Convert.ToDouble(item.Worksheet.Cells.Style.Numberformat.Format = format) ;
                            if (double.TryParse((string)item.Value, out x)==true)
                            {
                                item.Value = double.Parse((string)item.Value);
                            }
                            
                            //RetainingWallSheet.Cells[tableaddress].Style.Numberformat.Format= double.TryParse(item.Worksheet.Cells.Value.ToString(), out x);
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
                            errormessage = e.Message;
                            TaskDialog.Show(e.Message, "The file can not be saved. Please close the file and try again");
                            break;
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
