using System;

namespace FinanceTrackerClient.Models
{
    public class Entry
    {
        public int EntryID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; } //"Income" or "Expense"
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
    }
}