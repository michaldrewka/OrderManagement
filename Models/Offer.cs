using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("OfferTable")]
    public class Offer
    {
        [Key]
        public int Id { get; set; }
        [Column("offerId")]
        public string OfferId { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}