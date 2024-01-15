using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpec : BaseSpecifications<Product>
    {
        public ProductWithBrandAndTypeSpec(ProductSpecParams Params)
                : base(p => (!Params.BrandId.HasValue || p.ProductBrandId == Params.BrandId) && (!Params.TypeId.HasValue || p.ProductTypeId == Params.TypeId))
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);
            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch(Params.Sort)
                {
                    case "PriceAsc" : 
                        AddOrderBy(p => p.Price);
                        break;
                    case "PriceDesc": 
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            ApplyingPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);
        }
        public ProductWithBrandAndTypeSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);
        } 
    }
}
