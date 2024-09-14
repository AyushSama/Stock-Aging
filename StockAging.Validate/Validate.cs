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
                .Select(g =>
                {
                    var nonZeroPeriods = GetContiguousPeriods(g.OrderBy(e => e.Sequence).ToList());
                    var latestPeriod = nonZeroPeriods.OrderByDescending(p => p.StartDate).FirstOrDefault();

                    return latestPeriod != default((DateOnly StartDate, DateOnly EndDate)) ? new
                    {
                        Employee = g.OrderByDescending(e => e.Sequence).FirstOrDefault(),
                        StartDate = latestPeriod.StartDate,
                        EndDate = latestPeriod.EndDate,
                        Days = (latestPeriod.StartDate != DateOnly.MinValue && latestPeriod.EndDate != DateOnly.MinValue)
                            ? (latestPeriod.EndDate.DayNumber - latestPeriod.StartDate.DayNumber) + 1
                            : 0
                    } : null;
                })
                .Where(x => x != null) // Filter out null values
                .ToList();

            // Add valid employees to the result list
            foreach (var group in employeeGroups)
            {
                var dataTable = new EmployeDataTable
                {
                    Id = group.Employee.Id,
                    Symbol = group.Employee.Symbol,
                    Exchange = group.Employee.Exchange,
                    NetQuantity = group.Employee.NetQuantity,
                    First_Date = group.StartDate,
                    Last_Date = group.EndDate,
                    Days = group.Days
                };
                validEmployees.Add(dataTable);
            }

            return validEmployees;
        }

        private static List<(DateOnly StartDate, DateOnly EndDate)> GetContiguousPeriods(List<Employee> employees)
        {
            var periods = new List<(DateOnly StartDate, DateOnly EndDate)>();
            var currentStartDate = DateOnly.MinValue;
            var currentEndDate = DateOnly.MinValue;
            bool inPeriod = false;

            foreach (var emp in employees)
            {
                if (emp.NetQuantity > 0)
                {
                    if (!inPeriod)
                    {
                        // Start of a new period
                        currentStartDate = emp.Sequence;
                        inPeriod = true;
                    }
                    // End of the current period
                    currentEndDate = emp.Sequence;
                }
                else
                {
                    if (inPeriod)
                    {
                        // End of the current period
                        periods.Add((currentStartDate, currentEndDate));
                        inPeriod = false;
                    }
                }
            }

            // Add the last period if the loop ended while still in a period
            if (inPeriod)
            {
                periods.Add((currentStartDate, currentEndDate));
            }

            return periods;
        }
    }
}
