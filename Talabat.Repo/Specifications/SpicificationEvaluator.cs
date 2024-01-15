using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repo.Specifications
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            var Query = inputQuery;
            if(spec.Criteria != null)
                Query = Query.Where(spec.Criteria);

            if(spec.OrderBy != null)
                Query = Query.OrderBy(spec.OrderBy);

            if (spec.OrderByDesc != null)
                Query = Query.OrderByDescending(spec.OrderByDesc);

            if(spec.IsPaginationEnabled)
                Query = Query.Skip(spec.Skip).Take(spec.Take);

            Query = spec.Includes.Aggregate(Query, (CurrentQuery, Ex) => CurrentQuery.Include(Ex));
            return Query;
        }
    }
}
