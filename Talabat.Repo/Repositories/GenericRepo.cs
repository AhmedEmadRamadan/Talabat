using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.Specifications;
using Talabat.Repo.Data.Contexts;
using Talabat.Repo.Specifications;

namespace Talabat.Repo.Repositories
{
    public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
    {
        private readonly StoreContext _dbcontext;

        public GenericRepo(StoreContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        #region Without Specifications
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
                return (IReadOnlyList<T>)await _dbcontext.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();
            else
                return await _dbcontext.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        => await _dbcontext.Set<T>().FindAsync(id);
        public async Task Add(T item)
        => await _dbcontext.AddAsync(item);
        #endregion

        #region With Specifications
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
            => await ApplySpecifications(spec).ToListAsync();

        public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec)
            => await ApplySpecifications(spec).FirstOrDefaultAsync();

        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
            => SpecificationEvaluator<T>.GetQuery(_dbcontext.Set<T>(), spec);

        public async Task<int> GetCountWithSpec(ISpecifications<T> spec)
            => await ApplySpecifications(spec).CountAsync();

        public void Update(T item)
            => _dbcontext.Set<T>().Update(item);

        public void Delete(T item)
            => _dbcontext.Set<T>().Remove(item);


        #endregion
    }
}
