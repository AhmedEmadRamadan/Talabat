using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.IRepositories
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        #region Without Specifications
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task Add(T item);
        void Update(T item);
        void Delete(T item);
        #endregion

        #region With Specifications
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec);
        Task<int> GetCountWithSpec(ISpecifications<T> spec);
        #endregion
    }
}
