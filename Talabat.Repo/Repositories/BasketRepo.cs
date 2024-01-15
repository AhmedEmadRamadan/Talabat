using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;

namespace Talabat.Repo.Repositories
{
    public class BasketRepo : IBasketRepo
    {
        private readonly IDatabase _database;
        public BasketRepo(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string id)
        {
            var Basket = await _database.StringGetAsync(id);
            return Basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(Basket);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            var CreatedOrUpdated = await _database.StringSetAsync(basket.Id, JsonBasket, TimeSpan.FromDays(1));
            return !CreatedOrUpdated ? null : await GetBasketAsync(basket.Id);
        }
    }
}
