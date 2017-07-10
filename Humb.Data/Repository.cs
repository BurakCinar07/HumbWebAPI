using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO : Loglama işlemlerini burda yap
            }
        }
        public virtual void Update(T entity, int id)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                T existing = Entities.Find(id);
                if (existing != null)
                {
                    _context.Entry(entity).CurrentValues.SetValues(entity);
                    _context.SaveChanges();
                }
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
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                //TODO : Logging
            }
        }
        public T FindSingleBy(Expression<Func<T, bool>> match)
        {
            return Entities.FirstOrDefault(match);
        }
        public Task<T> FindSingleByAsync(Expression<Func<T, bool>> match)
        {
            return Entities.FirstOrDefaultAsync(match);
        }
        public IQueryable<T> FindBy(Expression<Func<T, bool>> match)
        {
            return Entities.Where(match);
        }
        public async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> match)
        {
            return await Entities.Where(match).ToListAsync();
        }
        public virtual IQueryable<T> GetAll()
        {
            return Entities;
        }
        public async Task<ICollection<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public int Count()
        {
            return Entities.Count();
        }
        public virtual void Save()
        {
            _context.SaveChanges();
        }

    }
}
