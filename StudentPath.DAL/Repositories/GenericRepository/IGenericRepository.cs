using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.GenericRepository
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {

        public Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int? page = null,
        int pageSize = 10,
        bool noTrack = false,
        params Expression<Func<TEntity, object>>[] includeProperties);

        public Task<TEntity> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
              bool noTrack = false,
              params Expression<Func<TEntity, object>>[] includeProperties);

        public Task CreateOrUpdateAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public object[] GetKeyValues(TEntity entity);


        public Task DeleteRangeAsync(IEnumerable<TEntity> entity);


    }
}
