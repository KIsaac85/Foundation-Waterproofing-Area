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
using Substructure_Area;
using System.Windows.Controls;

namespace Substructure_Area._5__Excel_Export
{
    /// <summary>
    /// This class is created to gather data(sheets)
    /// if the element was chosen by user
    /// and then save the file
    /// </summary>
    public class ExcelData
    {
        private ExcelPackage package { get; set; }


        /// <summary>
        /// All lists are sent to this class
        /// A sheet is created using each list
        /// Sheet shall be added to the package if the user choses it
        /// Then save as dialogue shows up
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="WallList"></param>
        /// <param name="isolatedFootingList"></param>
        /// <param name="ColumnsList"></param>
        /// <param name="BeamsList"></param>
        /// <param name="RaftList"></param>
        /// <param name="StripFootingsList"></param>
        /// <param name="areaUnit"></param>
        public void DataTable(ListBox listBox, IList<Element> WallList, IList<Element> isolatedFootingList,
            IList<Element> ColumnsList, IList<Element> BeamsList, IList<Element> RaftList, IList<Element> StripFootingsList, ForgeTypeId areaUnit)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (package = new ExcelPackage())
            {
                foreach (var item in listBox.SelectedItems)
                {
                    switch (item.ToString())
                    {
                        case "Retaining Walls":
                            package = RetainingWallsExcelSheet.retainingwallsheetcreation(package, WallList, areaUnit);
                            break;
                        case "Raft Foundation":
                            package = RaftExcelSheet.RaftFootingssheetcreation(package, RaftList, areaUnit);
                            break;
                        case "Strip Footings":
                            package = StripFootingsExcelSheet.StripFootingssheetcreation(package, StripFootingsList, areaUnit);
                            break;
                        case "Columns":
                            package = ColumnsExcelSheet.columnssheetcreation(package, ColumnsList, areaUnit);
                            break;
                        case "Semells":
                            package = SemellsExcelSheet.beamssheetcreation(package, BeamsList, areaUnit);
                            break;
                        case "Rectangular Footings":
                            package = FootingsExcelSheet.IsolatedFootingssheetcreation(package, isolatedFootingList, areaUnit);
                            break;

                    }
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
                            TaskDialog.Show(e.Message, "Error saving the file!");
                            break;
                        }

                    }
                } while (result != false && errormessage != null);

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
