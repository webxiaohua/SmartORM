using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.Sqlite.Tool;
using SmartORM.Sqlite.Core;
using System.Linq.Expressions;

namespace SmartORM.Sqlite
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

        public static List<T> ToList<T>(this Queryable<T> queryable)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                string orderby = queryable.OrderBy.HasValue() ? " ORDER BY " + queryable.OrderBy.Remove(queryable.OrderBy.Length - 1) : "";
                sql.AppendFormat("SELECT " + queryable.Select.GetSelectFields() + " FROM {0} WHERE 1 {1} {2}", queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName, string.Join("", queryable.Where), orderby);

                if (queryable.PageIndex > 0)
                {
                    if (queryable.PageSize > 0)
                    {
                        sql.Append(" LIMIT " + queryable.PageSize + " OFFSET " + (queryable.PageIndex - 1) * queryable.PageSize);
                    }
                }
                else
                {
                    if (queryable.PageSize > 0)
                    {
                        sql.Append(" LIMIT " + queryable.PageSize + " OFFSET 0 ");
                    }
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

        /// <summary>
        /// 分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="groupFields"></param>
        /// <returns></returns>
        public static Queryable<T> GroupBy<T>(this Queryable<T> queryable, string groupFields)
        {
            queryable.GroupBy = groupFields;
            return queryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public static Queryable<T> OrderBy<T>(this Queryable<T> queryable, string orderby)
        {
            queryable.OrderBy += orderby + " ASC,";
            return queryable;
        }

        /// <summary>
        /// 降序排列
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public static Queryable<T> OrderByDescing<T>(this Queryable<T> queryable, string orderby)
        {
            queryable.OrderBy += orderby + " DESC,";
            return queryable;
        }

        public static Queryable<T> PageIndex<T>(this Queryable<T> queryable, int from)
        {
            queryable.PageIndex = from;
            return queryable;
        }

        public static Queryable<T> PageSize<T>(this Queryable<T> queryable, int num)
        {
            queryable.PageSize = num;
            return queryable;
        }

        public static T Single<T>(this Queryable<T> queryable)
        {
            var list = queryable.ToList();
            if (list.Count() == 0)
            {
                return default(T);
            }
            else
            {
                return list.Single();
            }
        }
    }
}
