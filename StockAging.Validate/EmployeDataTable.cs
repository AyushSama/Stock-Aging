using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAging.Validate
{
    public class EmployeDataTable
    {   
        public string Id {  get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public int NetQuantity { get; set; }
        public DateOnly First_Date { get; set; }
        public DateOnly Last_Date { get; set; }
        public int Days { get; set; }
    }
}
