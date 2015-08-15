using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smart.ORM.Tool;
using Smart.ORM.Params;

namespace Smart.ORM.SQLExtends
{
    /// <summary>
    /// @Autor:Robin
    /// @Date:2015-08-10
    /// @Desc:SQL 扩展类
    /// </summary>
    public static class SQLableExtends
    {
        /// <summary>
        /// 获取From
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="tableName"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static SQLable From(this SQLable sqlable, object tableName, string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", tableName, shortName, sqlable.DB.IsNoLock.GetLockString());
            return sqlable;
        }

        /// <summary>
        /// Form
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="modelObj">表名</param>
        /// <param name="shortName">表名简写</param>
        /// <returns></returns>
        public static SQLable Form<T>(this SQLable sqlable, string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", typeof(T).Name, shortName, sqlable.DB.IsNoLock.GetLockString());
            return sqlable;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="leftFiled">join左边连接字段</param>
        /// <param name="RightFiled">join右边连接字段</param>
        /// <param name="type">join类型</param>
        /// <returns></returns>
        public static SQLable Join(this SQLable sqlable, object tableName, string shortName, string leftFiled, string RightFiled, JoinType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.From(\"table\").Join");
            sqlable.Sql.AppendFormat(" {0} JOIN {1} {2}  {3} ON  {4} = {5} ", type.ToString(), tableName, shortName, sqlable.DB.IsNoLock.GetLockString(), leftFiled, RightFiled);
            return sqlable;
        }

    }
}
