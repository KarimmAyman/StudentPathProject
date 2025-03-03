using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.DBHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.GenericRepository
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
    {

        #region prop
        private readonly StudentPathContext _db;
        private DbSet<TEntity> dbSet;
        #endregion

        #region ctor
        public GenericRepo(StudentPathContext db)
        {
            this._db = db;
            this.dbSet = db.Set<TEntity>();
        }

        #endregion

        #region Actions
        public async Task<IEnumerable<TEntity>> GetAsync(
           Expression<Func<TEntity, bool>>? filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
           int? page = null,
           int pageSize = 10,
           bool noTrack = false,
           params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }


            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (noTrack)
            {
                query = query.AsNoTracking();
            }

            if (page.HasValue && page > 0)
            {
                query = query.Skip((page.Value - 1) * pageSize).Take(pageSize);
            }


            return await query.ToListAsync();
        }

        public async Task<TEntity> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            bool noTrack = false,
             params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (noTrack)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task CreateOrUpdateAsync(TEntity entity)
        {

            var existingEntity = await dbSet.FindAsync(GetKeyValues(entity));

            if (existingEntity != null)
            {
                _db.Entry(existingEntity).CurrentValues.SetValues(entity); // edit
            }
            else
            {
                await dbSet.AddAsync(entity); // add
            }

        }


        public async Task DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));


            // if delete
            dbSet.Remove(entity);

        }
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entity)
        {
            dbSet.RemoveRange(entity);
        }

        public object[] GetKeyValues(TEntity entity)
        {
            var entityType = _db.Model.FindEntityType(typeof(TEntity));
            var key = entityType.FindPrimaryKey();
            var keyValues = key.Properties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray();
            return keyValues;
        }



        #endregion
    }
}
