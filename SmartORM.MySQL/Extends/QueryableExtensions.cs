using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL.Core;
using System.Linq.Expressions;
using SmartORM.MySQL.Tool;

namespace SmartORM.MySQL
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
        public static Queryable<T> Where<T>(this Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            LambdaExpressionAnalysis lambda = new LambdaExpressionAnalysis();
            lambda.AnalysisExpression(lambda, expression);
            queryable.Params.AddRange(lambda.Params);
            queryable.Where.Add(lambda.SqlWhere);
            return queryable;
        }

        public static List<T> ToList<T>(this Queryable<T> queryable) where T : class,new()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                string orderby = queryable.OrderBy.HasValue() ? " ORDER BY " + queryable.OrderBy : "";
                sql.AppendFormat("SELECT " + queryable.Select.GetSelectFields() + " FROM {0} WHERE 1 {1} {2}", queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName, string.Join("", queryable.Where), orderby);
                if (queryable.Skip == null && queryable.Take != null)
                {
                    sql.Append(" LIMIT 0," + queryable.Take);
                }
                else if (queryable.Skip != null && queryable.Take == null)
                {
                    sql.Append(" LIMIT " + queryable.Skip);
                }
                else if (queryable.Skip != null && queryable.Take != null)
                {
                    sql.Append(" LIMIT " + queryable.Skip + "," + queryable.Take);
                }
                var reader = queryable.DB.GetReader(sql.ToString(), queryable.Params.ToArray());
                List<T> result = reader.ToList<T>();
                queryable = null;
                return result;
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
