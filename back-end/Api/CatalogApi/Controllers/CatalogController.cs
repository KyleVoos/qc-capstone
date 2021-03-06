﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogApi.Infrastructure;
using CatalogApi.Model;
using CatalogApi.ViewModel;
using System.Net;

namespace CatalogApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private CatalogContext _catalogContext;

        public CatalogController(CatalogContext context)
        {
            _catalogContext = context;
        }

        // GET api/products/{id}
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Products), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Products>> ItemByIdAsync(string id)
        {

            if (id == null)
                return BadRequest();

            var item = await _catalogContext.products.SingleOrDefaultAsync(a => a.Id == id);

            if (item != null)
                return Ok(item);

            return NotFound();
        }

        /*
         * {sort?} is an option route parameter.
         * ascending = sorted by price in ascending order
         * descending = sorted by price in descending order
         * reverse = reverse alphabetical order for product names
         * needed way and if no sorting is needed this parameter can be omitted from the route
         * and it will be sorted based on the product name.
         * GET (CatalogApi) http://localhost:7001/api/products/page/sort[?pageSize=3&pageIndex=10]
         * GET (APIGateway) http://localhost:7000/catalog-api/products/page/sort[?pageSize=3&pageIndex=10]
         */
        [HttpGet, Route("page/{sort?}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<PageView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Items(string sort, [FromQuery]int pageSize = 10,
                                               [FromQuery]int pageIndex = 0)
        {
            if (pageSize == 0)
                return BadRequest();

            // return in alphabetical order by default
            var totalItems = await _catalogContext.products.LongCountAsync();
            var newT = (from pt in _catalogContext.products
                        join ot in _catalogContext.offerings on pt.Id equals ot.Product_key
                        orderby ot.Unit_retail ascending
                        select new PageView
                        {
                            Id = pt.Id, // in future change to product_key
                            Product_name = pt.Product_name,
                            Unit_retail = Math.Round(ot.Unit_retail, 2),
                            Offering_key = ot.Id
                        }).OrderBy(p => p.Product_name);

            // if the optional route parameter equals 'ascending' sort results in ascending price
            if (sort == "ascending")
                newT = newT.OrderBy(p => p.Unit_retail);

            // if the optional route parameter equals 'descending' sort results in descending price
            else if (sort == "descending")
                newT = newT.OrderByDescending(p => p.Unit_retail);

            // if the optional route parameter equals 'reverse' sort results in reverse alphabetical order
            else if (sort == "reverse")
                newT = newT.OrderByDescending(p => p.Product_name);

            var itemsOnPage = await newT
                                   .GroupBy(p => p.Product_name)
                                   .Select(g => g.First())
                                   .Skip(pageSize * pageIndex)
                                   .Take(pageSize)
                                   .ToListAsync();

            // too the JSON being returned this is adding the fields pageIndex, pageSize, totalItems and itemsOnPage
            var model = new PaginatedItemsViewModel<PageView>(
                    pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        /*
         * This returns a list of all the offerings for a single product based on the product ID in the route.
         * If information is needed about a specific offering of a product this route should not be used.
         * GET (CatalogApi) http://localhost:7001/api/products/offerings/{productId}
         * GET (APIGateway) http://localhost:7000/catalog-api/products/offerings/{productId}
         */
        [HttpGet, Route("offerings/{productId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> OfferingsByIdAsync(string productId)
        {
            /*
             * Not really ever used, this was just a test to see if when querying for products
             * they could also be filtered by if the active_date was greater than the Unix 
             * timestamp of the current time current 
             */
            DateTime dateTime = DateTime.Now;
            DateTimeOffset dto = DateTimeOffset.Now;
            Int64 currentUnixTimestamp = dto.ToUnixTimeSeconds();

            // make sure that the product ID is received from the frontend in the URL
            if (productId == null)
                return BadRequest();

            /*
             * The SQL query, this can be optimized to use a view instead of joining all three tables.
             * The query is joining the products table with the offerings table on the product_key
             * and then joining the offerings table with the suppliers table on the supplier_key.
             */
            var newTable = (from pt in _catalogContext.products
                            join ot in _catalogContext.offerings on pt.Id equals ot.Product_key
                            join st in _catalogContext.suppliers on ot.Supplier_key equals st.Id
                            into temp
                            from rt2 in temp.DefaultIfEmpty()
                            where pt.Id == productId
                            where pt.Active_date < currentUnixTimestamp
                            orderby ot.Unit_retail
                            select new
                            {
                                pt.Id,
                                pt.Long_description,
                                pt.Product_name,
                                Offering_key = ot.Id,
                                Unit_retail = Math.Round(ot.Unit_retail, 2).ToString(),
                                ot.Uom,
                                ot.Supplier_key,
                                rt2.supplier_name
                            });

            var items = await newTable
                                .ToListAsync();

            // make sure that the number of elements in the array is not zero, indicates something went wrong.
            if (items.Count != 0)
                return Ok(items);

            return NotFound();
        }

        /*
         * use this route if you want to get the information about a single offering of a product
         * the route needs to be passed the offering ID of what you want information about
         * GET (CatalogApi) http://localhost:7001/api/products/offerings/single/{offeringId}
         * GET (APIGateway) http://localhost:7000/catalog-api/products/offerings/single/{offeringId}
         */
        [HttpGet, Route("offerings/single/{offeringId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SingleOfferingByIdAsync(string offeringId)
        {
            if (offeringId == null) return BadRequest();

            var result = await (from ot in _catalogContext.offerings
                                join pt in _catalogContext.products on ot.Product_key equals pt.Id
                                join st in _catalogContext.suppliers on ot.Supplier_key equals st.Id
                                into temp
                                from rt2 in temp.DefaultIfEmpty()
                                where ot.Id == offeringId
                                select new
                                {
                                    pt.Product_name,
                                    pt.Long_description,
                                    ot.Product_key,
                                    Offering_key = ot.Id,
                                    Unit_retail = Math.Round(ot.Unit_retail, 2),
                                    ot.Uom,
                                    ot.Supplier_key,
                                    rt2.supplier_name
                                }).ToListAsync();

            Console.WriteLine(result.Count());

            if (result.Count() != 0) return Ok(result);

            return NotFound();
        }

        /*
         * Route for the homepage to grab 15 randomly selected products
         * from the database.
         * GET (CatalogApi) http://localhost:7001/api/products/home
         * GET (APIGateway) http://localhost:7000/catalog-api/home
         */
        // GET /api/products/home
        [HttpGet, Route("home")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RandomHomePageProducts()
        {

            Random rnd = new Random();
            var randomResults = await (from pt in _catalogContext.products
                                       join ot in _catalogContext.offerings on pt.Id equals ot.Product_key
                                       join st in _catalogContext.suppliers on ot.Supplier_key equals st.Id
                                       orderby rnd.Next()
                                       select new
                                       {
                                           pt.Product_name,
                                           pt.Long_description,
                                           Offering_key = ot.Id,
                                           ot.Product_key,
                                           ot.Supplier_key,
                                           ot.Unit_retail,
                                           ot.Uom,
                                           st.supplier_name
                                       })
                                       .Take(15)
                                       .OrderBy(i => i.Unit_retail)
                                       .ToListAsync();

            if (randomResults.Count > 0)
                return Ok(randomResults);

            return NotFound();
        }

        /*
         * This route retrieves all of the offerings that a specific supplier has available.
         * Does the lookup based on the Supplier_key
         * GET (CatalogApi) http://localhost:7001/api/products/supplier/{supplierId}
         * GET (APIGateway) http://localhost:7000/catalog-api/products/supplier/{supplierId}
         */
        [HttpGet, Route("supplier/{supplierID}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<SupplierView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> supplierOfferings(string supplierId, [FromQuery]int pageSize = 10,
                                               [FromQuery]int pageIndex = 0)
        {
            if (supplierId == null)
                return NotFound();

            if (pageSize == 0)
                return BadRequest();

            var result = (from st in _catalogContext.suppliers
                                join ot in _catalogContext.offerings on st.Id equals ot.Supplier_key
                                join pt in _catalogContext.products on ot.Product_key equals pt.Id
                                into temp
                                from rt2 in temp.DefaultIfEmpty()
                                where st.Id == supplierId
                                orderby rt2.Product_name
                                select new SupplierView
                                {
                                    Product_name = rt2.Product_name,
                                    Long_description = rt2.Long_description,
                                    Product_key = ot.Product_key,
                                    Offering_key = ot.Id,
                                    Unit_retail = Math.Round(ot.Unit_retail, 2).ToString(),
                                    Uom = ot.Uom,
                                    Supplier_name = st.supplier_name,
                                    Supplier_key = st.Id
                                });

            var totalItems = result.Count();

            var itemsOnPage = await result
                                   .Skip(pageSize * pageIndex)
                                   .Take(pageSize)
                                   .ToListAsync();

            var model = new PaginatedItemsViewModel<SupplierView>(
                    pageIndex, pageSize, totalItems, itemsOnPage);

            return 
                Ok(model);
        }

        /*
         * This was a test when the discount information normalized and put into an SQL
         * table. Just leaving it here as an example of another way the discount information
         * could be handled if using a different NoSQL database for it is not wanted.
         */
        [HttpGet, Route("discounts/{offeringId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> offeringsWithDiscounts(string offeringId)
        {
            if (offeringId == null)
                return NotFound();

            var results = await (from d in _catalogContext.disc
                           where d.Product_key == offeringId
                           select new Disc
                           {
                               Product_key = d.Product_key,
                               Product_name = d.Product_name,
                               Long_description = d.Long_description,
                               Supplier_key = d.Supplier_key,
                               Supplier_name = d.Supplier_name,
                               Uom = d.Uom,
                               Offering_key = d.Offering_key,
                               Discount_key = d.Discount_key,
                               Unit_retail = d.Unit_retail,
                               tiers = d.tiers,
                               Type = d.Type,
                               Discount_price = 0
                           }).ToListAsync();

            for (int ii = 0; ii < results.Count(); ii++)
            {
                if (((results[ii].Type == "SUPPLIER_DISCOUNT") || (results[ii].Type == "PRODUCT_DISCOUNT")) && (results[ii].tiers[0].MinQty == 1))
                {
                    results[ii].Discount_price = results[ii].Unit_retail * (1 - results[ii].tiers[0].DiscountPercentage);
                    Console.WriteLine($"results[{ii}].Discount_price = {results[ii].Discount_price}");
                }
            }

            return Ok(results);
        }
    }
}
