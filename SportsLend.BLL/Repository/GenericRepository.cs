using Microsoft.EntityFrameworkCore;
using SportsLend.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsLend.BLL.Repository
{
    public class GenericRepository<T> where T : class
    {
        protected SportsLendDBContext _context;
        public GenericRepository()
        {
            _context ??= new SportsLendDBContext();
        }

        public GenericRepository(SportsLendDBContext context)
        {
            _context = context;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<int> CreateAsync(T entity)
        {
            _context.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;

            foreach (var property in tracker.Properties)
            {
                if (!property.Metadata.IsPrimaryKey())
                {
                    property.IsModified = true;
                }
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
