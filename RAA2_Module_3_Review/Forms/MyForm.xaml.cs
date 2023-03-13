using Autodesk.Revit.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RAA2_Module_3_Review
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        ObservableCollection<SheetData> SheetList { get; set; }
        ObservableCollection<Element> TBlockData { get; set; }
        ObservableCollection<View> ViewData { get; set; }
        public MyForm(List<Element> TblockList, List<View> ViewList)
        {
            InitializeComponent();

            SheetList = new ObservableCollection<SheetData>();
            TBlockData = new ObservableCollection<Element>(TblockList);
            ViewData = new ObservableCollection<View>(ViewList);

            SheetGrid.ItemsSource = SheetList;
            cmbTitleblock.ItemsSource= TBlockData;
            cmbViews.ItemsSource = ViewData;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SheetList.Add(new SheetData());
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (SheetData curRow in SheetList)
                {
                    if (SheetGrid.SelectedItem == curRow)
                        SheetList.Remove(curRow);
                }
            }
            catch (Exception)
            {}
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public List<SheetData> GetSheetData()
        {
            return SheetList.ToList();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            SheetList.Clear();

            OpenFileDialog selectFile = new OpenFileDialog();
            selectFile.Multiselect = false;
            selectFile.RestoreDirectory = true;
            selectFile.Filter = "*csv file (*.csv)|*.csv";

            if(selectFile.ShowDialog() == true)
            {
                // read csv file
                string[] sheetArray = System.IO.File.ReadAllLines(selectFile.FileName);

                foreach(string sheetString in sheetArray)
                {
                    string[] cellData = sheetString.Split(',');

                    SheetData curSD = new SheetData();
                    curSD.SheetNumber = cellData[0];
                    curSD.SheetName = cellData[1];

                    if (cellData[2] == "true")
                        curSD.IsPlaceholder = true;
                    else
                        curSD.IsPlaceholder = false;

                    // add method to get view by name

                    // add method to get titleblock by name

                    SheetList.Add(curSD);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.RootFolder = Environment.SpecialFolder.MyDocuments;

            if(folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                folderPath= folderDialog.SelectedPath;
                string csvFilePath = folderPath + "\\sheet list export.csv";

                using(StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    foreach(SheetData curSheet in SheetList)
                    {
                        string sheetNum = "";
                        string sheetName = "";
                        string view = "";
                        string titleBlock = "";

                        if (curSheet.SheetName != null)
                            sheetName = curSheet.SheetName;
                        if (curSheet.SheetNumber != null)
                            sheetNum = curSheet.SheetNumber;
                        if (curSheet.SelectedView != null)
                            view = curSheet.SelectedView.Name;
                        if (curSheet.Titleblock != null)
                            titleBlock = curSheet.Titleblock.Name;

                        writer.WriteLine(sheetNum + "," + sheetName + "," + curSheet.IsPlaceholder
                            + "," + view + "," + titleBlock);
                    }
                }
            }
        }
    }
}
