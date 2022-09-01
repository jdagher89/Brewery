namespace Brewery.Models
{
    public class WholesalerQuoteResponse
    {
        public int WholesalerId { get; set; }
        public int BeerId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
