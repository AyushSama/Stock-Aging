using StockAging.Data.Interface;
using System.Windows;
using StockAging.Validate;
using Microsoft.Win32;
using System.IO;
using StockAging.Data;
using ClosedXML.Excel;

namespace StockAging
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load the last saved directory path
            string lastPath = Properties.Settings.Default.LastDirectoryPath;
            if (!string.IsNullOrWhiteSpace(lastPath) && Directory.Exists(lastPath))
            {
                Directory_Path.Text = lastPath;
            }

        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = Directory_Path.Text;

            Properties.Settings.Default.LastDirectoryPath = dirPath;
            Properties.Settings.Default.Save();

            Dictionary<string, string> userNames = MapIdAndUserNames.ReadUserListFromDirectory(dirPath);

            List<List<Employee>> employees = ReadNetPositionFile.ReadFileFromDirectory(dirPath, userNames);

            var validEmployees = EmployeeValidation.FindEmployeesWithSameSymbolFor5Days(employees);

            EmployeeDataGrid.ItemsSource = validEmployees;
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Directory_Path.Text = Path.GetDirectoryName(openFileDialog.FileName);
            }
        }

        public void NetQuantityFilter_Click(object sender, RoutedEventArgs e)
        {
            var employeeDetails = EmployeeDataGrid.ItemsSource as List<EmployeDataTable>;

            if (employeeDetails == null)
                return; // Handle cases where ItemsSource is not set or is of a different type

            // Sort the list based on NetQuantity and days (Sequence) in descending order
            var sortedEmployees = employeeDetails
            .OrderByDescending(e => e.Days)  // Sort by Days first
            .ThenByDescending(e => e.NetQuantity) // Then by NetQuantity
            .ToList();

            // Rebind the sorted list to the DataGrid
            EmployeeDataGrid.ItemsSource = sortedEmployees;

        }

        private void ExchangeFilter_Click(object sender, RoutedEventArgs e)
        {
            var employeeDetails = EmployeeDataGrid.ItemsSource as List<EmployeDataTable>;

            if (employeeDetails == null)
                return; // Handle cases where ItemsSource is not set or is of a different type

            // Sort the list based on NetQuantity and days (Sequence) in descending order
            var sortedEmployees = employeeDetails
            .OrderByDescending(e => e.Days)  // Sort by Days first (descending)
            .ThenBy(e => e.Exchange)  // Then by Exchange (ascending)
            .ToList();

            // Rebind the sorted list to the DataGrid
            EmployeeDataGrid.ItemsSource = sortedEmployees;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the data from the DataGrid's ItemsSource
            var employeeDetails = EmployeeDataGrid.ItemsSource as List<EmployeDataTable>;

            if (employeeDetails == null || !employeeDetails.Any())
            {
                MessageBox.Show("No data to export.");
                return;
            }

            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employee Data");

                // Add column headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "UserName";
                worksheet.Cell(1, 3).Value = "Symbol";
                worksheet.Cell(1, 4).Value = "Exchange";
                worksheet.Cell(1, 5).Value = "NetQuantity";
                worksheet.Cell(1, 6).Value = "Days";

                // Add employee data to the worksheet
                for (int i = 0; i < employeeDetails.Count; i++)
                {
                    var employee = employeeDetails[i];
                    worksheet.Cell(i + 2, 1).Value = employee.Id;
                    worksheet.Cell(i + 2, 2).Value = employee.Name;
                    worksheet.Cell(i + 2, 3).Value = employee.Symbol;
                    worksheet.Cell(i + 2, 4).Value = employee.Exchange;
                    worksheet.Cell(i + 2, 5).Value = employee.NetQuantity;
                    worksheet.Cell(i + 2, 6).Value = employee.Days;
                }

                worksheet.Columns().AdjustToContents(); // Adjust column widths

                // Get the path to the Downloads folder
                string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                string filePath = Path.Combine(downloadsFolder, "EmployeeData.xlsx");

                try
                {
                    // Save the file to the Downloads folder
                    workbook.SaveAs(filePath);
                    MessageBox.Show($"File saved successfully to Downloads!!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                }
            }
        }

    }
}