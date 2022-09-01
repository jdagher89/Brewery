using Brewery.Context;
using Brewery.Controllers;
using Brewery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Xunit;

namespace BreweryUnitTesting
{
    public class BeertTest : DbContext
    {
        private BreweryController _BreweryController;
        private readonly BreweryDb _breweryDB;
        IConfiguration _configuration;
        public BeertTest()
        {
            try
            {
                _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();
                var BreweryConnectionString = _configuration.GetConnectionString("BreweryConn");
                var options = new DbContextOptionsBuilder<BreweryDb>().UseSqlServer(BreweryConnectionString).Options;
                _breweryDB = new BreweryDb(options);
                _BreweryController = new BreweryController(_breweryDB, _configuration);
            }
            catch (Exception ex)
            {

            }
        }
        [Fact]
        public async Task BeerList()
        {
            try
            {

             var resp =  await _BreweryController.GetBrewerBeers(1);
            var result = Assert.IsType<OkObjectResult>(resp);
            Assert.Equal(200,result.StatusCode);
             }
            catch (Exception ex)
            {

            }
        }
        [Fact]
        public async Task AddBeer()
        {
            try
            {
                BeerRequest B = new BeerRequest
                {
                    AlcoholContent = "2.2%",
                    BrewerId = 1,
                    Name = "Leffe Blond 3 From Unit Test",
                    Price = 6.5
                }
            ;
                var resp = await _BreweryController.AddBeer(B);
                var result = Assert.IsType<OkObjectResult>(resp);
                Assert.Equal(200, result.StatusCode);
            }
            catch (Exception ex)
            {

            }
        }
        [Fact]
        public async Task AddBeerInvalidBrewer()
        {
            try
            {
                BeerRequest B = new BeerRequest
                {
                    AlcoholContent = "2.2%",
                    BrewerId = -99,
                    Name = "Leffe Blond 3 From Unit Test",
                    Price = 6.5
                }
            ;
                var resp = await _BreweryController.AddBeer(B);
                var result = Assert.IsType<BadRequestResult>(resp);
                Assert.Equal(400, result.StatusCode);
            }
            catch (Exception ex)
            {

            }
        }
        [Fact]
        public async Task DeleteBeer()
        {
            try
            {

                var resp = await _BreweryController.GetAllBeers();
                var respDelete = await _BreweryController.DeleteBeer(resp[resp.Count-1].Id);
                var result = Assert.IsType<OkObjectResult>(respDelete);
                Assert.Equal(200, result.StatusCode);
            }
            catch (Exception ex)
            {

            }

        }
        [Fact]
        public async Task AddBeerSale()
        {
            try
            {
                BrewerSalesRequest B = new BrewerSalesRequest
                {
                    BeerId = 5,
                    WholesalerId=1,
                    Quantity = 5,
                    SaleDate = DateTime.Now,
                    Price = 6.5
                }
            ;
                var resp = await _BreweryController.AddBeerSale(B);
                var result = Assert.IsType<OkObjectResult>(resp);
                Assert.Equal(200, result.StatusCode);
            }
            catch (Exception ex)
            {

            }
        }
        [Fact]
        public async Task UpdateBeerStock()
        {
            try
            {
                WholesalerStockRequest B = new WholesalerStockRequest
                {
                    BeerId = 5,
                    WholesalerId = 1,
                    Quantity = 55
                }
            ;
                var resp = await _BreweryController.UpdateWholesalerStock(B);
                var result = Assert.IsType<OkObjectResult>(resp);
                Assert.Equal(200, result.StatusCode);
            }
            catch (Exception ex)
            {

            }
        }
        [Fact]
        public async Task RequestWholesalerQuote()
        {
            try
            { List<WholesalerStockRequest> QuoteList = new List<WholesalerStockRequest>();
                WholesalerStockRequest Quote = new WholesalerStockRequest
                {
                    BeerId = 5,
                    WholesalerId = 1,
                    Quantity = 55
                };
                QuoteList.Add(Quote);
            ;
                var resp = await _BreweryController.RequestWholesalerQuote(QuoteList);
                var result = Assert.IsType<OkObjectResult>(resp);
                var model = result.Value as WholesalerQuoteResponse;
                Assert.Equal(55*10*0.8, model.Price);
            }
            catch (Exception ex)
            {

            }
        }
    }
}