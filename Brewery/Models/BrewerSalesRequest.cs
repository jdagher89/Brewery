namespace Brewery.Models
{
    public class BrewerSalesRequest
    {
        public int BeerId { get; set; }
        public int WholesalerId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
