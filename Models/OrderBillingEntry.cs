using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("OrdersBillingEntriesTable")]
    public class OrderBillingEntry
    {
        [Key]
        public int Id { get; set; }
        [Column("billingEntryId")]
        public string BillingEntryId { get; set; }
        [Column("occurredAt")]
        public DateTime OccurredAt { get; set; }
        [Column("typeId")]
        public string TypeId { get; set; }
        [Column("typeName")]
        public string TypeName { get; set; }
        [Column("offerId")]
        public string OfferId { get; set; }
        [Column("offerName")]
        public string OfferName { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("currency")]
        public string Currency { get; set; }
        [Column("orderId")]
        public string OrderId { get; set; }
    }
}
