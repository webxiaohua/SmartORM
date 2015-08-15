using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Smart.ORM.Tool;

namespace Smart.ORM.Query
{
    /// <summary>
    /// @Autor:Robin
    /// @Date:2015-08-10
    /// @Desc:Queryable 扩展类
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Queryable<T> Where<T>(this Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }

    }
}
