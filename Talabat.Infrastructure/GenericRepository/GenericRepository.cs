﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Infrastructure.Data;

namespace Talabat.Infrastructure.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext) //Ask CLR for Creating object from Db Context
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(Product))
            //	return (IEnumerable<T>) await _dbContext.Set<Product>().orderBy(P=>P.Name).AsNoTracking().Include(P => P.Brand).Include(P => P.Cateogry).ToListAsync();
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            //if(typeof(T) == typeof(Product))
            //	return  await _dbContext.Set<Product>().Where(P=>P.Id == id).Include(P=>P.Brand).Include(P=>P.Cateogry).FirstOrDefaultAsync() as T;
            return await _dbContext.Set<T>().FindAsync(id);
        }


        public async Task<IReadOnlyList<T>> GetAllWithSpec(ISpecifications<T> spec)
        {
            return await ApplySpecictaions(spec).AsNoTracking().ToListAsync();
        }


        public async Task<T?> GetWithSpec(ISpecifications<T> spec)
        {
            return await ApplySpecictaions(spec).FirstOrDefaultAsync();
        }



        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await ApplySpecictaions(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecictaions(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

		public void Add(T entity)
		  =>_dbContext.Set<T>().Add(entity);

		public void Updater(T entity)
		=>_dbContext.Set<T>().Update(entity);

		public void Delete(T entity)
	          =>_dbContext.Set<T>().Remove(entity);
	}
}
