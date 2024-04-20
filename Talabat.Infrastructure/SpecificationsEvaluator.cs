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
			//_dbcontext.Set<TEntity>().where(E=>E.id==1);
			//include Expressions
			//1. P=>P.Brand
			//2.P=>P.Category

			spec.Includes.Aggregate(query,(currentQuery,includeExpression)=>currentQuery.Include(includeExpression));
			//_dbcontext.Set<TEntity>().where(E=>E.id==1).Include(P=>P.Brand);
			//_dbcontext.Set<TEntity>().where(E=>E.id==1).Include(P=>P.Brand).Include(P=>P.category);

			return query;
		}

	}
}
