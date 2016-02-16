using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Smart.ORM.Tool;
using Smart.ORM.Helper;

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

        public static List<T> ToList<T>(this Queryable<T> queryable)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
                var order = queryable.OrderBy.IsValuable() ? (",row_index=ROW_NUMBER() OVER(ORDER BY " + queryable.OrderBy + " )") : null;
                sql.AppendFormat("SELECT " + queryable.Select.GetSelectFiles() + " {1} FROM {0} {2} WHERE 1=1 {3} {4} ", queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName, order, withNoLock, string.Join("", queryable.Where), queryable.GroupBy.GetGroupBy());
                if (queryable.Skip == null && queryable.Take != null)
                {
                    sql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    sql.Append(") t WHERE t.row_index<=" + queryable.Take);
                }
                else if (queryable.Skip != null && queryable.Take == null)
                {
                    sql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    sql.Append(") t WHERE t.row_index>" + (queryable.Skip));
                }
                else if (queryable.Skip != null && queryable.Take != null)
                {
                    sql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    sql.Append(") t WHERE t.row_index BETWEEN " + (queryable.Skip + 1) + " AND " + (queryable.Skip + queryable.Take));
                }
                var reader = queryable.DB.GetReader(sql.ToString(), queryable.Params.ToArray());
                var reval = SQLConvertHelper.DataReaderToList<T>(typeof(T), reader, queryable.Select.GetSelectFiles());
                queryable = null;
                return reval;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("sql:{0}\r\n message:{1}", ex.Message));
            }
            finally
            {
                sql = null;
                queryable = null;
            }
        }

    }
}
