using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Application.DTOs
{
    public class BudgetResponseDto
    {
        public int Id { get; set; }
        public decimal MonthlyLimit { get; set; }
        public DateTime Month { get; set; }
        public int CategoryId { get; set; }
        //public /*string*/ ?CategoryName { get; set; }
    }
}
