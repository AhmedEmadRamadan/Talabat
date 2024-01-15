using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Repo.Data.Contexts;

namespace Talabat.Repo.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbcontext)
        {
            await GetBrandsData(dbcontext);
            await GetTypesData(dbcontext);
            await GetProductsData(dbcontext);
            await GetOrderDeliveryMethod(dbcontext);
        }

        private static async Task GetBrandsData(StoreContext dbcontext)
        {
            if(!dbcontext.ProductBrands.Any())
            {
                var BrandsData = File.ReadAllText("../Talabat.Repo/Data/DataSeed/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
                if (Brands?.Count > 0)
                {
                    foreach (var Brand in Brands)
                        await dbcontext.Set<ProductBrand>().AddAsync(Brand);
                    await dbcontext.SaveChangesAsync();
                }
            }
        }

        private static async Task GetTypesData(StoreContext dbcontext)
        {
            if (!dbcontext.ProductTypes.Any())
            {
                var TypesData = File.ReadAllText("../Talabat.Repo/Data/DataSeed/types.json");
                var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
                if (Types?.Count > 0)
                {
                    foreach (var Type in Types)
                        await dbcontext.Set<ProductType>().AddAsync(Type);
                    await dbcontext.SaveChangesAsync();
                }
            }
        }

        private static async Task GetProductsData(StoreContext dbcontext)
        {
            if (!dbcontext.Products.Any())
            {
                var ProductsData = File.ReadAllText("../Talabat.Repo/Data/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                if (Products?.Count > 0)
                {
                    foreach (var product in Products)
                        await dbcontext.Set<Product>().AddAsync(product);
                    await dbcontext.SaveChangesAsync();
                }
            }
        }
        private static async Task GetOrderDeliveryMethod(StoreContext dbcontext)
        {
            if (!dbcontext.DeliveryMethods.Any())
            {
                var DeliveryMethodsData = File.ReadAllText("../Talabat.Repo/Data/DataSeed/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                if (deliveryMethods?.Count > 0)
                {
                    foreach (var DMetho in deliveryMethods)
                        await dbcontext.Set<DeliveryMethod>().AddAsync(DMetho);
                    await dbcontext.SaveChangesAsync();
                }
            }
        }
    }
}
