using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class ProductRepository : GenaricRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;
        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync(ProductParams productParams)
        {
            var query = context.Products.Include(m => m.Category).Include(m => m.Photos).AsNoTracking();

            if (!string.IsNullOrEmpty(productParams.Search))
            {
                var searchWords = productParams.Search.Split(' ');
                query = query.Where(m => searchWords.All(word => m.Name.ToLower().Contains(word.ToLower()) || m.Description.ToLower().Contains(word.ToLower())));
                //query = query.Where(m => m.Name.ToLower().Contains(productParams.Search.ToLower()) || m.Description.ToLower().Contains(productParams.Search.ToLower()));
            }

            if (productParams.CategoryId.HasValue)
                query = query.Where(m => m.CategoryId == productParams.CategoryId);

            if (!string.IsNullOrEmpty(productParams.sort))
            {
                query = productParams.sort switch
                {
                    "PriceAsc" => query.OrderBy(m => m.NewPrice),
                    "PriceDesc" => query.OrderByDescending(m => m.NewPrice),
                    _ => query.OrderBy(m => m.Name),
                };
            }
            
            //pageNumber = pageNumber > 0 ? pageNumber : 1;
            //pageSize = pageSize > 0 ? pageSize : 3;
            query = query.Skip((productParams.pageSize) *(productParams.PageNumber - 1)).Take(productParams.pageSize);

            var result = mapper.Map<List<ProductDTO>>(query);
            return result;
        }

        public async Task<bool> AddAsync(AddProductDTO productDTO)
        {
            if (productDTO == null) return false;

            var product = mapper.Map<Product>(productDTO);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var ImagePath = await imageManagementService.AddImageAsync(productDTO.Photo, productDTO.Name);

            var photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();
            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(UpdateProductDTO updateProductDTO)
        {
            if(updateProductDTO is null) return false;

            var FindProduct = await context.Products.Include(m => m.Category).Include(m => m.Photos).FirstOrDefaultAsync(m => m.Id == updateProductDTO.Id);
            if (FindProduct is null) return false;

            mapper.Map(updateProductDTO, FindProduct);

            var FindPhoto = await context.Photos.Where(m => m.ProductId == updateProductDTO.Id).ToListAsync();
            foreach(var item in FindPhoto)
            {
                imageManagementService.DeleteImageAsnc(item.ImageName);
            }
            context.Photos.RemoveRange(FindPhoto);

            var ImagePath = await imageManagementService.AddImageAsync(updateProductDTO.Photo, updateProductDTO.Name);

            var photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = updateProductDTO.Id
            }).ToList();
            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(Product product)
        {
            var photo = await context.Photos.Where(m => m.ProductId == product.Id).ToListAsync();
            foreach (var item in photo)
            {
                imageManagementService.DeleteImageAsnc(item.ImageName);
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
