using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Infrastructure
{
	internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,ISpecifications<TEntity> spec)
		{
			var query = inputQuery; //_dbcontext.Set<TEntity>();

			if(spec.Critieria is not null)
				query=query.Where(spec.Critieria);

			if(spec.OrderBy is not null)
				query=query.OrderBy(spec.OrderBy);

			else if(spec.OrderByDesc is not null)
				query=query.OrderByDescending(spec.OrderByDesc);


			if(spec.IsPaginationEnabled)
				query=query.Skip(spec.Skip).Take(spec.Take);


			//_dbcontext.Set<TEntity>().where(E=>E.id==1);
			//include Expressions
			//1. P=>P.Brand
			//2.P=>P.Category

			query=spec.Includes.Aggregate(query,(currentQuery,includeExpression)=>currentQuery.Include(includeExpression));
			//_dbcontext.Set<TEntity>().where(E=>E.id==1).Include(P=>P.Brand);
			//_dbcontext.Set<TEntity>().where(E=>E.id==1).Include(P=>P.Brand).Include(P=>P.category);

			return query;
		}

	}
}
