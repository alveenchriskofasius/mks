﻿using API.Context.SP;
using API.Context.Table;
using API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using MitraKaryaSystem.Models;

namespace API.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MKSTableContext _context;
        private readonly MKSSPContextProcedures _procedures;
        public ProductRepository(MKSTableContext context, MKSSPContextProcedures procedures)
        {
            _context = context;
            _procedures = procedures;
        }

        public async Task DeleteProduct(int id)
        {
            _context.Products.Remove(await _context.Products.FindAsync(id));
            await _context.SaveChangesAsync();
        }
        public async Task<ProductModel> FillFormProduct(int id)
        {
            Product? product = await _context.Products.FindAsync(id);
            ProductModel? productModel = null;
            if (product == null)
            {
                productModel = new ProductModel();
            }
            else
            {
                productModel = new ProductModel
                {
                    ID = product.ID,
                    Name = product.Name,
                    CategoryID = product.CategoryID,
                    UnitID = product.UnitID,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    StockQuantity = product.StockQuantity,
                    SupplierID = product.SupplierID
                };
            }
            return productModel;
        }
        public async Task<object> GetProductList()
        {
            return Task.FromResult<object>(await _procedures.GetProductListAsync());
        }
        public async Task SaveProduct(ProductModel productModel)
        {
            try
            {
                if (productModel.ID == 0)
                {
                    _context.Products.Add(new Product
                    {
                        Name = productModel.Name,
                        CategoryID = productModel.CategoryID,
                        UnitID = productModel.UnitID,
                        Description = productModel.Description,
                        UnitPrice = productModel.UnitPrice,
                        StockQuantity = productModel.StockQuantity,
                        SupplierID = productModel.SupplierID,
                        CreatedBy = AuthRepository.CurrentUser.FullName,
                        CreatedAt = DateTime.Now
                    });
                }
                else
                {
                    Product product = await _context.Products.FindAsync(productModel.ID);
                    product.Name = productModel.Name;
                    product.CategoryID = productModel.CategoryID;
                    product.UnitID = productModel.UnitID;
                    product.Description = productModel.Description;
                    product.UnitPrice = productModel.UnitPrice;
                    product.StockQuantity = productModel.StockQuantity;
                    product.SupplierID = productModel.SupplierID;
                    product.UpdatedBy = AuthRepository.CurrentUser.FullName;
                    product.UpdatedAt = DateTime.Now;
                    _context.Products.Update(product);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}