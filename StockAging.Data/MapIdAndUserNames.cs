namespace StockAging.Data
{
    public class MapIdAndUserNames
    {
        public static Dictionary<string,string> ReadUserListFromDirectory(string dirPath)
        {
            var idToNameMap = new Dictionary<string, string>();

            try
            {
                // Get all CSV files in the directory
                string[] csvFiles = Directory.GetFiles(dirPath, "*.csv", SearchOption.AllDirectories);

                // Find the UserList.csv file
                string userListFile = Array.Find(csvFiles, file => Path.GetFileName(file).Equals("UserList.csv", StringComparison.OrdinalIgnoreCase));

                if (userListFile != null)
                {
                    // Read and parse UserList.csv
                    var csvLines = File.ReadAllLines(userListFile);

                    // Start from 1 to skip the header (assuming there's a header)
                    for (int i = 1; i < csvLines.Length; i++)
                    {
                        var line = csvLines[i];

                        // Split the line based on the CSV delimiter (assuming comma)
                        var values = line.Split(',');

                        // Assuming ID is in the first column (index 0) and Name in the second (index 1)
                        string id = values[1].Trim().Trim('"');
                        string name = values[2].Trim().Trim('"'); 

                        id = id.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
                        name = name.Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim();

                        // Add ID and Name to the dictionary
                        idToNameMap[id] = name;
                    }
                }
                else
                {
                    Console.WriteLine("UserList.csv not found in the directory.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading UserList.csv: {ex.Message}");
            }

            return idToNameMap;
        }
    }
}
