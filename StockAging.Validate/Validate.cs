using StockAging.Data.Interface;

namespace StockAging.Validate
{
    public class EmployeeValidation
    {
        public static List<Employee> FindEmployeesWithSameSymbolFor5Days(List<List<Employee>> employeesFromAllFiles)
        {
            var validEmployees = new List<Employee>();

            // Flatten the nested lists into one
            var allEmployees = employeesFromAllFiles.SelectMany(x => x).ToList();

            // Group by Employee ID and Symbol
            var employeeGroups = allEmployees
                .GroupBy(e => new { e.Id, e.Symbol })
                .Where(g => g.Count() == 5 && g.All(e => int.Parse(e.NetQuantity) > 0)) // Ensure 5 days with NetQuantity > 0
                .Select(g => g.OrderByDescending(e => e.Sequence).FirstOrDefault())
                .ToList();

            // Add valid employees to the result list
            foreach (var group in employeeGroups)
            {
                validEmployees.Add(group);
            }

            return validEmployees;
        }
    }
}
