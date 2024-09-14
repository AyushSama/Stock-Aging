using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAging.Data.Interface
{
    public class Employee
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string NetQuantity { get; set; }
        public DateOnly Sequence { get; set; }    
    }
}
