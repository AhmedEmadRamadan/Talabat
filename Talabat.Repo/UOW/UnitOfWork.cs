using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.UOW_Interface;
using Talabat.Repo.Data.Contexts;
using Talabat.Repo.Repositories;

namespace Talabat.Repo.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        private Hashtable _Repositories;
        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            _Repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

        public IGenericRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;
            if (!_Repositories.ContainsKey(type))
            {
                var Repo = new GenericRepo<TEntity>(_dbContext);
                _Repositories.Add(type, Repo);
            }
            return (GenericRepo<TEntity>) _Repositories[type];
        }
    }
}
