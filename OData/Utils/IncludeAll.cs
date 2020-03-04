using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Utils
{
    public static class CustomIncludeAll
    {
        public static IQueryable<TEntity> IncludeAll<TEntity>(this IQueryable<TEntity> queryable) where TEntity : class
        {
            var type = typeof(TEntity);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var isVirtual = property.GetGetMethod().IsVirtual;
                if (isVirtual)
                {
                    queryable = queryable.Include(property.Name);
                }
            }
            return queryable;
        }
    }
}
