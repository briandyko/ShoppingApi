﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Services
{
    public class EfSqlShopping : ILookupProducts, IProductCommands
    {
        private readonly ShoppingDataContext _context;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;

        public EfSqlShopping(ShoppingDataContext context, IMapper mapper, MapperConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        public async Task<GetProductDetailsResponse> AddProduct(PostProductRequest productToAdd)
        {
            var product = _mapper.Map<Product>(productToAdd);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<GetProductDetailsResponse>(product);
        }

        public async Task<GetProductDetailsResponse> GetById(int id)
        {
            return await _context.GetItemsInInventory()
                .Where(p => p.Id == id)
                .ProjectTo<GetProductDetailsResponse>(_config)
                .SingleOrDefaultAsync();
        }

        public async Task<GetProductsResponse> GetSummary()
        {
            var response = new GetProductsResponse();

            var countInInventory = await _context.Products.CountAsync(p => p.InInventory);
            var countOutOfInventory = await _context.Products.CountAsync(p => !p.InInventory);

            response.NumberOfProductsInInventory = countInInventory;
            response.NumberOfProdcutsOutOfStock = countOutOfInventory;
            response.NumberOfProductsAddedToday = new Random().Next(12, 300); //HA!
            return response;
        }

        public async Task<GetProductsListSummary> GetSummaryList(string category)
        {
            var list = await _context.GetItemsFromCategory(category)
                .ProjectTo<ProductSummaryItem>(_config)
                .ToListAsync();

            var response = new GetProductsListSummary
            {
                Data = list,
                Category = category,
                Count = list.Count()
            };
            return response;
        }
    }
}
