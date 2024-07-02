using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Models
{
    [Table("OfferBillingEntriesTable")]
    public class OfferBillingEntry
    {
        [Key]
        public int Id { get; set; }
        [Column("offerId")]
        public string OfferId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
    }
}
