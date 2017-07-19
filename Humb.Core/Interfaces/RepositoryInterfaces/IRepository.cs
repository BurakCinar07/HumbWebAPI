using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.RepositoryInterfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(object id);
        void Insert(T entity);
        void Update(T entity, int id);
        void Delete(T entity);
        IQueryable<T> GetAll();
        Task<ICollection<T>> GetAllAsync();
        T FindSingleBy(Expression<Func<T, bool>> match);
        Task<T> FindSingleByAsync(Expression<Func<T, bool>> match);
        IQueryable<T> FindBy(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> match);
        bool Any(Expression<Func<T, bool>> match);
        int Count();
        void Save();
    }
}
