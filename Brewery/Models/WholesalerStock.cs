using System.ComponentModel.DataAnnotations;

namespace Brewery.Models
{
    public class WholesalerStock
    {
        [Key]
        public int Id { get; set; } 
        public int WholesalerId { get; set; }
        public int BeerId { get; set; }
        public int Quantity { get; set; }
        public double ImposedBeerPrice { get; set; }
    }
}
