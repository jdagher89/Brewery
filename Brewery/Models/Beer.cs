using System.ComponentModel.DataAnnotations;

namespace Brewery.Models
{
    public class Beer
    {
        [Key]
        public int Id { get; set; }
        public int BrewerId { get; set; }
        public string Name { get; set; }
        public string AlcoholContent { get; set; }
        public double Price { get; set; }
    }
}
