﻿using API.Repository.Interfaces;
using API.Services.Interfaces;
using MitraKaryaSystem.Models;

namespace API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<object> DeleteProduct(int id)
        {
            return await _productRepository.DeleteProduct(id);
        }
        public async Task<ProductModel> FillFormProduct(int id)
        {
            return await _productRepository.FillFormProduct(id);
        }

        public async Task<object> GetProductComboList(string name)
        {
            return await _productRepository.GetProductComboList(name);
        }

        public async Task<object> GetProductList()
        {
            return await _productRepository.GetProductList();
        }
        public async Task<object> SaveProduct(ProductModel product)
        {
            return await _productRepository.SaveProduct(product);
        }
    }
}
