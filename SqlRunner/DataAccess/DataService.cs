/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SqlRunner.DataAccess
{
    public class DataService<T> : IDataService<T> where T : class
    {
        public DataContext Context { get; private set; }

        public DataService()
        {
            Context = new DataContext();
        }

        public DataService(DataContext context)
        {
            Context = context;
        }

        public async virtual Task<T> CreateAsync(T entity, CancellationToken token)
        {
            try
            {
                Context.Set<T>().Add(entity);

                if (await Context.SaveChangesAsync(token) > 0)
                    return entity;
            }
            catch { }
            return null;
        }

        public async virtual Task<T> UpdateAsync(T entity, CancellationToken token)
        {
            try
            {
                Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                if (await Context.SaveChangesAsync(token) > 0)
                    return entity;
            }
            catch { }
            return null;
        }

        public async virtual Task<bool> DeleteAsync(T entity, CancellationToken token)
        {
            try
            {
                Context.Set<T>().Remove(entity);
                return await Context.SaveChangesAsync(token) > 0;
            }
            catch { }
            return false;
        }

        public async virtual Task<T> GetAsync(int id, CancellationToken token)
        {
            return await Context.Set<T>().FindAsync(id, token);
        }

        public IQueryable<T> All()
        {
            return Context.Set<T>();
        }
    }
}
