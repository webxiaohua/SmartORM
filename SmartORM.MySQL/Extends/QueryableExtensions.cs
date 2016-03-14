using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL.Core;
using System.Linq.Expressions;

namespace SmartORM.MySQL.Extends
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Queryable<T> Where<T>(this Queryable<T> queryable, Expression<Func<T, bool>> expression) {
            var type = queryable.Type;
            return null;
        }
    }
}
