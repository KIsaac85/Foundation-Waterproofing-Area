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
        public void DataTable()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("My Sheet");
                sheet.Cells["A1"].Value = "Hello World!";
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    FileName = "NewSheet", // Default file name
                    DefaultExt = ".xlsx", // Default file extension
                    Filter = "Excel Sheet (.xlsx)|*" // Filter files by extension
                };
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
                // Save to file
                package.Save();
                
             

            }


        }
        
    }
}
