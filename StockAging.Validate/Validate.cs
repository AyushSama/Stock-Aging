using StockAging.Data.Interface;
using System;
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
                .Where(g => g.Count() >= 2) // && g.All(e => int.Parse(e.NetQuantity) > 0))
                .Select(g =>
                {
                    var dates = g.Select(e => e.Sequence).OrderBy(d => d).ToList();
                    var (start, end) = GetContiguousSequenceRange(dates);
                    return new
                    {
                        Employee = g.OrderByDescending(e => e.Sequence).FirstOrDefault(),
                        StartDate = start,
                        EndDate = end,
                        Days = (start != DateOnly.MinValue && end != DateOnly.MinValue) ? (end.DayNumber - start.DayNumber) + 1 : 0 // Calculate the number of days
                    };
                })
                .Where(x => x.StartDate != DateOnly.MinValue && x.EndDate != DateOnly.MinValue) // Filter out invalid ranges
                .ToList();

            // Add valid employees to the result list
            foreach (var group in employeeGroups)
            {
                var dataTable = new EmployeDataTable
                {
                    Id = group.Employee.Id,
                    Symbol = group.Employee.Symbol,
                    NetQuantity = group.Employee.NetQuantity,
                    First_Date = group.StartDate,
                    Last_Date = group.EndDate,
                    Days = group.Days
                };
                validEmployees.Add(dataTable);
            }

            return validEmployees;
        }

        private static (DateOnly, DateOnly) GetContiguousSequenceRange(List<DateOnly> sequences)
        {
            if (sequences.Count < 5 || !IsContiguous(sequences))
                return (DateOnly.MinValue, DateOnly.MinValue);

            // The first date in the sorted list
            var start = sequences.First();
            // The last date in the sorted list
            var end = sequences.Last();

            return (start, end);
        }

        private static bool IsContiguous(List<DateOnly> sequences)
        {
            for (int i = 0; i < sequences.Count - 1; i++)
            {
                // Check if the next date is exactly one day after the current date
                if (sequences[i].AddDays(1) != sequences[i + 1])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
