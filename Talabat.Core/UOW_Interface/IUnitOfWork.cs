using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;

namespace Talabat.Core.UOW_Interface
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();
    }
}
