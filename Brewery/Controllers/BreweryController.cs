using Brewery.Context;
using Brewery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Brewery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreweryController : ControllerBase
    {
        private BreweryDb _breweryDB;
        private IConfiguration _configuration;
        public BreweryController(BreweryDb breweryDB,IConfiguration configuration)
        {
            _breweryDB = breweryDB;
            _configuration = configuration;
        }
        [HttpGet("GetAllBeers")]
        public async Task<List<Beer>> GetAllBeers()
        {
            try
            {
                var list = _breweryDB.Beer.AsQueryable();
                List<Beer> BeerList = list.ToList();
                return BeerList;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        [HttpGet("GetBrewerBeers/{Id}")]
        public async Task<IActionResult> GetBrewerBeers(int Id)
        {
            try
            {
                var list = _breweryDB.Beer.AsQueryable();
                List<Beer> BeerList = list.Where(m => m.BrewerId == Id).ToList();
                return Ok(BeerList);
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        [HttpPost("AddBeer")]
        public async Task<IActionResult> AddBeer([FromBody] BeerRequest Request)
        {
            try
            {
                    Beer B = new Beer
                    {
                        BrewerId = Request.BrewerId,
                        AlcoholContent = Request.AlcoholContent,
                        Name = Request.Name,
                        Price = Request.Price
                    }
                    ;
                    _breweryDB.Add(B);
                    _breweryDB.SaveChanges();
                    return Ok(B);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }

        }
        [HttpGet("DeleteBeer/{Id}")]
        public async Task<IActionResult> DeleteBeer([FromRoute]int Id)
        {
            try
            {
                var Beer = _breweryDB.Beer.SingleOrDefault(x=>x.Id == Id);
                if (Beer == null){
                    return NotFound();
                }
                else {
                    _breweryDB.Beer.Remove(Beer);
                    _breweryDB.SaveChanges();

                    return Ok(Beer);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500,"An Error has Occured");
            }

        }
        [HttpPost("AddBeerSale")]
        public async Task<IActionResult> AddBeerSale([FromBody] BrewerSalesRequest Request)
        {
            try
            {
                BrewerSales B = new BrewerSales
                {
                    WholesalerId = Request.WholesalerId,
                    BeerId = Request.BeerId,
                    Quantity = Request.Quantity,
                    Price = Request.Price,
                    SaleDate = Request.SaleDate
                }
                ;
                _breweryDB.Add(B);
                _breweryDB.SaveChanges();

                var WholesalerStock= _breweryDB.WholesalerStock.FirstOrDefault(m => m.WholesalerId == Request.WholesalerId && (m.BeerId == Request.BeerId));


                if (WholesalerStock==null)
                {

                    WholesalerStock WS= new WholesalerStock
                    {
                        WholesalerId = Request.WholesalerId,
                        BeerId = Request.BeerId,
                        Quantity = Request.Quantity,
                        ImposedBeerPrice = Request.Price
                    }
               ;
                    _breweryDB.Add(WS);
                    _breweryDB.SaveChanges();

                }
                else
                {
                    WholesalerStock.Quantity = WholesalerStock.Quantity+Request.Quantity;
                    _breweryDB.Entry(WholesalerStock).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _breweryDB.SaveChanges();

                }

                return Ok(B);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An Error has Occured");
            }

        }
        [HttpPut("UpdateWholesalerStock")]
        public async Task<IActionResult> UpdateWholesalerStock([FromBody] WholesalerStockRequest Request)
        {
            try
            {

                var WholesalerStock = _breweryDB.WholesalerStock.FirstOrDefault(m => m.WholesalerId == Request.WholesalerId && (m.BeerId == Request.BeerId));
                if (WholesalerStock != null)
                {
                    WholesalerStock.Quantity = Request.Quantity;
                    _breweryDB.Entry(WholesalerStock).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _breweryDB.SaveChanges();

                    return Ok(WholesalerStock);

                }
                else
                {
                    return StatusCode(500, "Wholesaler Beer Stock not found.");

                }

            }
            catch (Exception ex)
            {

                return StatusCode(500, "An Error has Occured");
            }

        }
        [HttpPost("RequestWholesalerQuote")]
        public async Task<IActionResult> RequestWholesalerQuote([FromBody] List<WholesalerStockRequest> Request)
        {
            WholesalerQuoteResponse response = new WholesalerQuoteResponse();  
            try
            {
                if (Request.Any())
                {
                    foreach (var item in Request)
                    {
                        //var DuplicatesList = 
                        List<WholesalerStockRequest> DuplicatesList = Request.Where(m => m.WholesalerId == item.WholesalerId && (m.BeerId == item.BeerId)).ToList();
                        if (DuplicatesList.Any())
                        {
                            if (DuplicatesList.Count() > 1)
                            {
                                return BadRequest("Order cannot contain duplicates.");
                            }

                            else
                            {
                                WholesalerStock WholesalerStock = _breweryDB.WholesalerStock.Where(m => m.WholesalerId == item.WholesalerId && (m.BeerId == item.BeerId)).Select(m => new WholesalerStock
                                {
                                    Quantity = m.Quantity,
                                    BeerId = m.BeerId,
                                    WholesalerId = m.WholesalerId,
                                    Id = m.Id,
                                    ImposedBeerPrice = m.ImposedBeerPrice
                                }).First();
                                if (WholesalerStock != null)
                                {
                                    if (item.Quantity > WholesalerStock.Quantity)
                                    {
                                        return BadRequest("Order quanity is bigger than wholesaler stock.");
                                    }
                                    else
                                    {
                                        WholesalerQuoteResponse resp = new WholesalerQuoteResponse
                                        {
                                            Quantity = item.Quantity,
                                            BeerId = item.BeerId,
                                            WholesalerId = item.WholesalerId,
                                            Price = (item.Quantity > 10 && item.Quantity <= 20) ? WholesalerStock.ImposedBeerPrice * (Convert.ToDouble(_configuration["Above10Discount"])) * item.Quantity : item.Quantity > 20 ? WholesalerStock.ImposedBeerPrice * (Convert.ToDouble(_configuration["Above20Discount"]))* item.Quantity : WholesalerStock.ImposedBeerPrice* item.Quantity
                                        };
                                        response = resp;
                                    }
                                }
                                else
                                {
                                    return BadRequest("Wholesaler Beer Stock not found.");

                                }
                            }
                        }
                    }
                }
                else
                {
                    return BadRequest("Empty Order.");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An Error has Occured");
            }

        }
    }
}
