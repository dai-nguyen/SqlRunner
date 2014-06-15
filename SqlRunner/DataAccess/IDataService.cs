/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using System.Linq;
using System.Threading.Tasks;

namespace SqlRunner.DataAccess
{
    public interface IDataService<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<T> GetAsync(long id);
        IQueryable<T> All();
    }
}
