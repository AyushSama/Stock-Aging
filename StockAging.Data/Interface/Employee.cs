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
        public string UserName {  get; set; }
        public string Symbol { get; set; }
        public int NetQuantity { get; set; }
        public string Exchange {  get; set; }
        public DateOnly Sequence { get; set; }    
    }
}
