namespace Brewery.Models
{
    public class BeerRequest
    {
        public int BrewerId { get; set; }
        public string Name { get; set; }
        public string AlcoholContent { get; set; }
        public double Price { get; set; }
    }
}
