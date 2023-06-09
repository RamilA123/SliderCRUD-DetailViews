using System;
using Fiorello.Areas.Admin.ViewModels.Product;
using Fiorello.Data;
using Fiorello.Models;
using Fiorello.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public ProductService(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _context.products.Include(m => m.Images)
                                                                                        .Take(8).Where(m => !m.SoftDelete)
                                                                                        .ToListAsync();
        public async Task<List<Product>> GetAllWithIncludesAsync() => await _context.products.Include(m=>m.Images)
                                                                                             .Include(m=>m.Category)
                                                                                             .Include(m=>m.Discount)
                                                                                             .ToListAsync();
        public async Task<Product> GetByIdAsync(int? id) => await _context.products.FindAsync(id);
        public async Task<Product> GetByIdWithImagesAsync(int? id) => await _context.products.Include(m => m.Images)
                                                                                             .FirstOrDefaultAsync(m => m.Id == id);
        public List<ProductVM> GetMappedDatas(List<Product> products)
        {
            List<ProductVM> list = new();
            foreach (var product in products)
            {
                list.Add(new ProductVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price.ToString("0.####"),
                    Discount = product.Discount.Name,
                    CategoryName = product.Category.Name,
                    Image = product.Images.Where(m=>m.IsMain).FirstOrDefault().Images
                });
            }
            return list;
        }
        public async Task<Product> GetWithIncludesAsync(int id) => await _context.products.Where(m=>m.Id == id)
                                                                                          .Include(m => m.Images)
                                                                                          .Include(m => m.Category)
                                                                                          .Include(m => m.Discount)
                                                                                          .FirstOrDefaultAsync();
        public ProductDetailVM GetMappedData(Product product)
        {
            return new ProductDetailVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.ToString("0.#####"),
                CategoryName = product.Category.Name,
                CreateDate = product.CreatedDate.ToString("dd/MM/yyyy"),
                Discount = product.Discount.Name,
                Image = product.Images.Select(m => m.Images)
            };
        }
    }
}

