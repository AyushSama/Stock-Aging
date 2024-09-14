using StockAging.Data.Interface;
using System.Windows;
using StockAging.Validate;

namespace StockAging { 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
    
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            string dirPath = Directory_Path.Text;
            List<List<Employee>> employees = ReadFile.ReadFileFromDirectory(dirPath);

            var validEmployees = Validate.EmployeeValidation.FindEmployeesWithSameSymbolFor5Days(employees);

            foreach (var employee in validEmployees)
            {
                Console.WriteLine($"Employee ID: {employee.Id}, Symbol: {employee.Symbol}");
            }


            EmployeeDataGrid.ItemsSource = validEmployees;

        }
    }
}