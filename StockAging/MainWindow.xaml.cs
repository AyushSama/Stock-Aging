using StockAging.Data.Interface;
using System.Windows;
using StockAging.Validate;
using Microsoft.Win32;
using System.IO;
using StockAging.Data;

namespace StockAging { 
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

            List<List<Employee>> employees = ReadNetPositionFile.ReadFileFromDirectory(dirPath,userNames);

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

        }
    }
}