using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPilot.Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string ?Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        //Added required keyword to Category,UserId, User to ensure they are not null
        public required string UserId { get; set; }

        // set to nullable to avoid initialization issues
        public AppUser ?User { get; set; }
        }
    }
