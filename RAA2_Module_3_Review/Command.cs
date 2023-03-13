#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace RAA2_Module_3_Review
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here
            FilteredElementCollector tblockCollector = new FilteredElementCollector(doc);
            tblockCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            tblockCollector.WhereElementIsElementType();

            // open form
            MyForm currentForm = new MyForm(tblockCollector.ToList(), GetViews(doc))
            {
                Width = 800,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            if(currentForm.DialogResult == true)
            {
                int counter = 0;

                using(Transaction t = new Transaction(doc))
                {
                    t.Start("Create new sheets");
                    // get form data and do something
                    foreach (SheetData curData in currentForm.GetSheetData())
                    {
                        try
                        {
                            ViewSheet newSheet;

                            if (curData.IsPlaceholder == true)
                            {
                                newSheet = ViewSheet.CreatePlaceholder(doc);
                            }
                            else
                            {
                                newSheet = ViewSheet.Create(doc, curData.Titleblock.Id);
                            }

                            newSheet.SheetNumber = curData.SheetNumber.ToUpper();
                            newSheet.Name = curData.SheetName.ToUpper();

                            if(curData.SelectedView != null)
                            {
                                Viewport curVP = Viewport.Create(doc, newSheet.Id, curData.SelectedView.Id, new XYZ());
                            }
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("ERROR", "An error occured: " + ex.Message);
                            
                        }
                        

                    }
                    t.Commit();
                }

            }

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
        private List<View> GetViews(Document doc)
        {
            List<View> returnList = new List<View>();

            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Views);

            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc);
            sheetCollector.OfCategory(BuiltInCategory.OST_Sheets);

            foreach (View curView in viewCollector)
            {
                if (curView.IsTemplate == false)
                {
                    if (Viewport.CanAddViewToSheet(doc, sheetCollector.FirstElementId(), curView.Id) == true)
                        returnList.Add(curView);
                }
            }

            return returnList;
        }
    }
}
