using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Domain.Entities
{
    public class Budget
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        //added required keyword to Category,UserId to ensure they are not null
        public   Category ? Category { get; set; }
        public required string UserId { get; set; }
        public  AppUser ?User { get; set; }
        public decimal MonthlyLimit { get; set; }
        public DateTime Month { get; set; }
    }
}
