using StockAging.Data.Interface;
using System.Windows;
using StockAging.Validate;
using Microsoft.Win32;
using System.IO;

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

            List<List<Employee>> employees = ReadFile.ReadFileFromDirectory(dirPath);

            var validEmployees = Validate.EmployeeValidation.FindEmployeesWithSameSymbolFor5Days(employees);

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
    }
}