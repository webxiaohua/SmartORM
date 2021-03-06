﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL.Core;
using MySql.Data.MySqlClient;
using SmartORM.MySQL.Tool;
using System.Reflection;
using System.Linq.Expressions;

namespace SmartORM.MySQL
{
    public class SmartORMClient : MySQLHelper
    {
        public string ConnectionString { get; set; }
        /// <summary>
        /// 初始化orm客户端操作类
        /// </summary>
        /// <param name="connectionString"></param>
        public SmartORMClient(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
            string tmpStr = connectionString.Split(';').Where(c => c.StartsWith("database=", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().ToLower();
            if (tmpStr.IsNullOrEmpty())
                throw new ArgumentException("connectionString:无法匹配到数据库，请检查配置");
            else
                DBName = tmpStr.Substring(tmpStr.IndexOf("=") + 1); //获取DBName
        }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; private set; }

        /// <summary>
        /// 创建表查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Queryable<T> Queryable<T>() where T : new()
        {
            return new Queryable<T>() { DB = this };
        }

        public List<T> Query<T>(string sql, object whereObj = null) where T : class,new()
        {
            MySqlDataReader reader = null;
            var parms = GetParameters(whereObj);
            var type = typeof(T);
            reader = GetReader(sql, parms);
            if (type.IsIn(typeof(int), typeof(string)))
            {
                List<T> strReval = new List<T>();
                using (MySqlDataReader re = reader)
                {
                    while (re.Read())
                    {
                        strReval.Add((T)Convert.ChangeType(re.GetValue(0), type));
                    }
                }
                return strReval;
            }
            var reval = reader.ToList<T>();
            return reval;
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public List<dynamic> QueryDynamicList(string sql, object whereObj = null)
        {
            MySqlDataReader reader = null;
            var parms = GetParameters(whereObj);
            reader = GetReader(sql, parms);
            var reval = DynamicHelper.DataFillDynamicList(reader);
            return reval;
        }
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public dynamic QueryDynamicObj(string sql, object whereObj = null)
        {
            MySqlDataReader reader = null;
            var parms = GetParameters(whereObj);
            reader = GetReader(sql, parms);
            var reval = DynamicHelper.DataFillDynamic(reader);
            return reval;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isIdentity"></param>
        /// <returns></returns>
        public object Insert<T>(T obj, bool isIdentity = true) where T : class
        {
            Type type = obj.GetType();

            string tableName = GetTableNameByClassType(type);
            StringBuilder sbInsertSql = new StringBuilder();
            List<MySqlParameter> pars = new List<MySqlParameter>();
            string cacheSqlKey = "db.Insert." + tableName;
            var cacheSqlManager = CacheHelper<StringBuilder>.GetInstance(); //sql 缓存
            string cachePropertiesKey = "db." + tableName + ".GetProperties";
            var cachePropertiesManager = CacheHelper<PropertyInfo[]>.GetInstance(); // 属性缓存
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            if (cacheSqlManager.ContainsKey(cacheSqlKey))
            {
                sbInsertSql = cacheSqlManager[cacheSqlKey];
                //3.遍历实体的属性集合 
                Func<PropertyInfo, bool> HasAutoIncrementAttr = prop =>
                {
                    foreach (Attribute item in prop.GetCustomAttributes(true))
                    {
                        if (item is AutoIncrementAttribute)
                            return true;
                    }
                    return false;
                };
                foreach (var prop in props)
                {
                    if (isIdentity)
                    {
                        if (HasAutoIncrementAttr(prop))
                        {
                            //自增列 跳过
                            continue;
                        }
                    }
                    object val = prop.GetValue(obj, null);
                    if (val == null)
                        val = DBNull.Value;
                    pars.Add(new MySqlParameter("?" + prop.Name, val));
                }
            }
            else
            {
                var primaryKeyName = string.Empty;
                //2.获得实体的属性集合 
                //实例化一个StringBuilder做字符串的拼接 
                sbInsertSql.Append("INSERT INTO " + tableName + " (");
                //3.遍历实体的属性集合 
                Func<PropertyInfo, bool> HasAutoIncrementAttr = prop =>
                {
                    foreach (Attribute item in prop.GetCustomAttributes(true))
                    {
                        if (item is AutoIncrementAttribute)
                            return true;
                    }
                    return false;
                };
                StringBuilder sbParams = new StringBuilder();
                foreach (var prop in props)
                {
                    if (isIdentity)
                    {
                        if (HasAutoIncrementAttr(prop))
                        {
                            //自增列 跳过
                            continue;
                        }
                    }
                    sbInsertSql.Append(prop.Name + ",");
                    sbParams.Append("?" + prop.Name + ",");
                    object val = prop.GetValue(obj, null);
                    if (val == null)
                        val = DBNull.Value;
                    pars.Add(new MySqlParameter("?" + prop.Name, val));
                }
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                sbParams.Remove(sbParams.Length - 1, 1);
                sbInsertSql.Append(" ) values(" + sbParams.ToString() + ");");
                if (isIdentity)
                {
                    sbInsertSql.Append("SELECT @@IDENTITY;");
                }
                cacheSqlManager.Add(cacheSqlKey, sbInsertSql, cacheSqlManager.Day);
            }
            var sql = sbInsertSql.ToString();
            var lastInsertRowId = GetScalar(sql, pars.ToArray());
            return lastInsertRowId;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Update<T>(object rowObj, Expression<Func<T, bool>> expression) where T : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SmartORMClient.Update.rowObj"); }
            if (expression == null) { throw new ArgumentNullException("SmartORMClient.Update.expression"); }
            Type type = typeof(T);
            string tableName = GetTableNameByClassType(type);
            StringBuilder sbSql = new StringBuilder(string.Format(" UPDATE {0} SET ", tableName));
            Dictionary<string, DBTypeValue> rows = MySQLConvertHelper.GetObjectToDictionary(rowObj);
            int i = 0;
            foreach (var r in rows)
            {
                if (i == 0)
                {
                    if (rowObj.GetType() == type)
                    {
                        ++i;
                        continue;
                    }
                }
                sbSql.Append(string.Format(" {0} =?{0}  ,", r.Key));
                ++i;
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            sbSql.Append(" WHERE  1=1  ");
            LambdaExpressionAnalysis re = new LambdaExpressionAnalysis();
            re.AnalysisWhereExpression(re, expression);
            sbSql.Append(re.SqlWhere);

            List<MySqlParameter> parsList = new List<MySqlParameter>();
            parsList.AddRange(re.Params);
            parsList.AddRange(rows.Select(c => new MySqlParameter("?" + c.Key, c.Value.DBValue)));
            var updateRowCount = ExecuteCommand(sbSql.ToString(), parsList.ToArray());
            return updateRowCount > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Delete<T>(Expression<Func<T, bool>> expression)
        {
            Type type = typeof(T);
            string tableName = GetTableNameByClassType(type);
            LambdaExpressionAnalysis re = new LambdaExpressionAnalysis();
            re.AnalysisWhereExpression(re, expression);
            string sql = string.Format("DELETE FROM {0} WHERE 1=1 {1}", tableName, re.SqlWhere);
            bool isSuccess = ExecuteCommand(sql, re.Params.ToArray()) > 0;
            return isSuccess;
        }

    }
}
