using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Repositories;
using Ecom.Infrastructure.Repositories.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection infrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenaricRepository<>), typeof(GenaricRepository<>));
            //services.AddScoped<ICategoryRepository, CategoryRepository>();
            //services.AddScoped<IProductRepository, ProductRepository>();
            //services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IImageManagementService, ImageManagementService>();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot")));
            services.AddDbContext<AppDbContext>(op => 
            {
                op.UseSqlServer(configuration.GetConnectionString("EcomDatabase"));
            });
            return services;
        }
    }
}
