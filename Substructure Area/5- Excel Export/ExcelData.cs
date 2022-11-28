using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ExcelData()
        {
            SelectedItems = new List<string>();
        }
        public void DataTable(List<String>SelectedItems, IList<Element> WallList)
        {
            using (var package = new ExcelPackage())
            {
                if (SelectedItems.Contains("Retaining Walls"))
                {
                    ExcelWorksheet RetainingWallSheet = package.Workbook.Worksheets.Add("Retaining Walls");

                    for (int i = 0; i < WallList.Count; i++)
                    {
                        RetainingWallSheet.Cells["A{0}" + i].LoadFromDataTable(;
                    }
                    
                    SaveFileDialog saveFile = new SaveFileDialog
                    {
                        FileName = "NewSheet", // Default file name
                        DefaultExt = ".xlsx", // Default file extension
                        Filter = "Excel Sheet (.xlsx)|*" // Filter files by extension
                    };
                }










                bool? result = saveFile.ShowDialog();
                switch (result)
                {
                    case true:
                        {
                            FileInfo filename = new FileInfo(saveFile.FileName);
                            package.SaveAs(filename);
                            Process.Start(Path.Combine(filename.Directory.ToString(),filename.ToString()));
                            break;
                        }

                }
            }
        }
    }
}
