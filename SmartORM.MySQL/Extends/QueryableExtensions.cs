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
            lambda.AnalysisWhereExpression(lambda, expression, queryable.Params);
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
                        sql.Append(" LIMIT " + (queryable.PageIndex - 1) * queryable.PageSize + "," + queryable.PageSize);
                    }
                }
                else
                {
                    if (queryable.PageSize > 0)
                    {
                        sql.Append(" LIMIT 0," + queryable.PageSize);
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

        /// <summary>
        /// 聚合查询  计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static int Count<T>(this Queryable<T> queryable)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int count = 0;
                sql.AppendFormat("SELECT COUNT(*) FROM {0} WHERE 1 {1}", queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName, string.Join("", queryable.Where));
                object val = queryable.DB.GetScalar(sql.ToString(), queryable.Params.ToArray());
                if (val is long)
                {
                    count = Int32.Parse(val.ToString());
                }
                else if (val is int)
                {
                    count = (int)val;
                }
                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
