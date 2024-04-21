﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
	{
		public Expression<Func<T, bool>>? Critieria { get ; set; }=null;
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();

        public BaseSpecifications()
        {
            //critiriea=null
            

        }

        public BaseSpecifications(Expression<Func<T,bool>> CritirieaExpression)
        {
            Critieria=CritirieaExpression;
           
        }
    }
}
