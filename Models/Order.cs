using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("OrderTable")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Column("orderId")]
        public string OrderId { get; set; }

        [Column("erpOrderId")]
        public int? ErpOrderId { get; set; }

        [Column("invoiceId")]
        public int? InvoiceId { get; set; }

        [Column("storeId")]
        public int StoreId { get; set; }
    }
}
