using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyConsoleApp.Models
{
    [Table("BillingEntriesTable")]
    public class BillingEntry
    {
        [Key]
        public int Id { get; set; }

        [Column("orderId")]
        public string OrderId { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
    }
}
