using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RN_TaskManager.DAL.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> FindByIdAsync(int id);

        Task CreateAsync(TEntity item);
        Task CreateAsync(List<TEntity> items);

        Task EditAsync(TEntity item);
        Task EditAsync(List<TEntity> items);

        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Запрос по условиям Лямбда-выражения
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// null или объект по условиям Лямбда-выражения
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
