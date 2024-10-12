using API.Context.SP;
using API.Context.Table;
using API.Repository.Interfaces;
using MitraKaryaSystem.Models;

namespace API.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MKSTableContext _context;
        private readonly MKSSPContextProcedures _procedures;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductRepository(MKSTableContext context, MKSSPContextProcedures procedures, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _procedures = procedures;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<object> DeleteProduct(int id)
        {
            try
            {
                _context.Products.Remove(await _context.Products.FindAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Task.FromResult<object>(new { success = false, error = e.Message });
            }
            return Task.FromResult<object>(new { success = true });
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
                    SupplierID = product.SupplierID,
                    Barcode = product.Barcode
                };
            }
            return productModel;
        }
        public async Task<object> GetProductList() => Task.FromResult<object>(await _procedures.GetProductListAsync());

        public async Task<object> GetProductComboList() => Task.FromResult<object>(await _procedures.uspGetProductComboListAsync());
        public async Task<object> SaveProduct(ProductModel productModel)
        {
            try
            {
                if (productModel.Barcode != null)
                {
                    if (productModel.Barcode.Length > 50)
                        return Task.FromResult<object>(new { success = false, error = "Barcode maximum is 50 length" });
                }
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
                        CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
                        Barcode = productModel.Barcode,
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
                    product.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    product.Barcode = productModel.Barcode;
                    product.UpdatedAt = DateTime.Now;
                    _context.Products.Update(product);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Task.FromResult<object>(new { success = false, error = e.Message });
            }
            return Task.FromResult<object>(new { success = true });
        }
    }
}
