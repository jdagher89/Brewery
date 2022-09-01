namespace Brewery.Models
{
    public class WholesalerStockRequest
    {
        public int WholesalerId { get; set; }
        public int BeerId { get; set; }
        public int Quantity { get; set; }
    }
}
