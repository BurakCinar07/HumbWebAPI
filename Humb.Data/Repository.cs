using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly EFDbContext _context;
        private IDbSet<T> _entities;

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }

        public Repository(EFDbContext context)
        {
            _context = context;
        }

        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("Insert");
                }
                Entities.Add(entity);
            }
            catch (Exception e)
            {
                // TODO : Loglama işlemlerini burda yap
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                Save();
            }
            catch (Exception e)
            {
                //TODO : Logging..
            }
        }
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                Entities.Remove(entity);
            }
            catch (Exception e)
            {
                //TODO : Logging
            }
        }
        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = Entities.Where(predicate);
            return query;
        }
        public virtual IQueryable<T> GetAll()
        {
            return Entities;
        }
        public virtual void Save()
        {
            _context.SaveChanges();
        }
    }
}
