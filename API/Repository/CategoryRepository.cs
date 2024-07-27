using API.Context.Table;
using API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using MitraKaryaSystem.Models;

namespace API.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MKSTableContext _context;

        public CategoryRepository(MKSTableContext context)
        {
            _context = context;
        }
        public async Task<object> DeleteCategory(int id)
        {
            try
            {
                _context.Categories.Remove(await _context.Categories.FindAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Task.FromResult<object>(new
                {
                    success = false,
                    error = e.Message
                });

            }
            return Task.FromResult<object>(new { success = true });
        }
        public async Task<CategoryModel> FillFormCategory(int id)
        {
            Category? category = await _context.Categories.FindAsync(id);
            CategoryModel? categoryModel = null;
            if (category == null)
            {
                categoryModel = new CategoryModel();
            }
            else
            {
                categoryModel = new CategoryModel
                {
                    ID = category.ID,
                    CategoryName = category.Name
                };
            }
            return categoryModel;
        }
        public async Task<object> GetCategoryList()=> Task.FromResult<object>(await _context.Categories.Select(x => new { x.ID, CategoryName = x.Name }).ToListAsync());
       
        public async Task<object> SaveCategory(CategoryModel categoryModel)
        {
            try
            {
                if (categoryModel.ID == 0)
                {
                    _context.Categories.Add(new Category
                    {
                        Name = categoryModel.CategoryName
                    });
                }
                else
                {
                    Category category = await _context.Categories.FindAsync(categoryModel.ID);
                    category.Name = categoryModel.CategoryName;
                    _context.Categories.Update(category);
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return Task.FromResult<object>(new
                {
                    success = false,
                    error = e.Message
                });

            }
            return Task.FromResult<object>(new { success = true });
        }
    }
}
