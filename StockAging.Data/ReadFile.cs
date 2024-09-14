using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using StockAging.Data.Interface;

public class ReadFile
{
    public static List<List<Employee>> ReadFileFromDirectory(string dirPath)
    {
        List<List<Employee>> employeesFromAllFiles = new List<List<Employee>>();

        try
        {
            // Register encoding provider to support additional encodings
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Get all Excel files (.xls and .xlsx) from the directory
            string[] filePaths = Directory.GetFiles(dirPath, "*.xlsx*");

            foreach (string filePath in filePaths)
            {
                try
                {
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // Create an Excel reader based on the file extension
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            // Convert to DataSet
                            var sequenceDate = ExtractDateFromFileName(Path.GetFileNameWithoutExtension(filePath));

                            var result = reader.AsDataSet();
                            var table = result.Tables[0]; // Get the first table

                            List<Employee> employeeList = new List<Employee>();

                            // Iterate through rows, starting from the second row to skip headers
                            for (int row = 1; row < table.Rows.Count; row++)
                            {
                                DataRow dataRow = table.Rows[row];

                                Employee employee = new Employee
                                {
                                    Id = dataRow[0].ToString(), // Assuming first column is ID
                                    Symbol = dataRow[2].ToString(), // Assuming third column is Symbol
                                    NetQuantity = int.Parse(dataRow[4].ToString()), // Assuming fifth column is NetQuantity
                                    Sequence = sequenceDate
                                };

                                if (employee.NetQuantity <= 0)
                                    continue;

                                employeeList.Add(employee);
                            }
                            employeesFromAllFiles.Add(employeeList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading files from directory: {ex.Message}");
        }

        return employeesFromAllFiles;
    }

    private static DateOnly ExtractDateFromFileName(string fileNameWithoutExtension)
    {
        // The date format in the file name is dd-MM-yy
        if (DateOnly.TryParseExact(fileNameWithoutExtension, "dd-MM-yy", null, System.Globalization.DateTimeStyles.None, out var dateOnly))
        {
            return dateOnly;
        }

        // Return a default value if parsing fails
        return DateOnly.MinValue; // Or use another appropriate default
    }


}
