using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xUnitNetCore.Web.Models;

namespace xUnitNetCore.Web.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbSet.FindAsync(id); //Sadece primary key'lerden veri bulmak için yazılan kod.
        }

        public async Task Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
        }
    }
}
