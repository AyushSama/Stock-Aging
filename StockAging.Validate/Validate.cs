using StockAging.Data.Interface;
using System.Collections.Generic;
using System.Linq;

namespace StockAging.Validate
{
    public class EmployeeValidation
    {
        public static List<EmployeDataTable> FindEmployeesWithSameSymbolFor5Days(List<List<Employee>> employeesFromAllFiles)
        {
            var validEmployees = new List<EmployeDataTable>();

            // Flatten the nested lists into one
            var allEmployees = employeesFromAllFiles.SelectMany(x => x).ToList();

            // Group by Employee ID and Symbol
            var employeeGroups = allEmployees
                .GroupBy(e => new { e.Id, e.Symbol })
                .Where(g => g.Count() >= 5 && g.All(e => int.Parse(e.NetQuantity) > 0) && HasSequentialSequence(g))
                .Select(g => g.OrderByDescending(e => e.Sequence).FirstOrDefault())
                .ToList();

            // Add valid employees to the result list
            foreach (var group in employeeGroups)
            {
                var dataTable = new EmployeDataTable();
                dataTable.Id = group.Id;
                dataTable.Symbol = group.Symbol;
                dataTable.NetQuantity = group.NetQuantity;
                validEmployees.Add(dataTable);
            }

            return validEmployees;
        }

        private static bool HasSequentialSequence(IGrouping<object, Employee> group)
        {
            var sequences = group.Select(e => e.Sequence).OrderBy(s => s).ToList();
            return sequences.Count >= 5 && IsContiguous(sequences);
        }

        private static bool IsContiguous(List<int> sequences)
        {
            for (int i = 0; i < sequences.Count - 1; i++)
            {
                if (sequences[i] + 1 != sequences[i + 1])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
