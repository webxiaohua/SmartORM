using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL.Core;
using MySql.Data.MySqlClient;
using SmartORM.MySQL.Tool;
using System.Reflection;
<<<<<<< HEAD
using System.Linq.Expressions;
=======
>>>>>>> 8a023782e2a3a71e4337df9fa727810ef0f558af

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
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
<<<<<<< HEAD
        /// <param name="entity"></param>
        /// <param name="isIdentity"></param>
        /// <returns></returns>
        public object Insert<T>(T entity, bool isIdentity = true) where T : class {
            Type type = entity.GetType();
            StringBuilder sbInsertSql = new StringBuilder();
            List<MySqlParameter> pars = new List<MySqlParameter>();
            //sql语句缓存
            string cacheSqlKey = "db.Insert." + type.Name;
            var cacheSqlManager = CacheHelper<StringBuilder>.GetInstance();
            //属性缓存
            string cachePropertiesKey = "db." + type.Name + ".GetProperties";
            var cachePropertiesManager = CacheHelper<PropertyInfo[]>.GetInstance();
=======
        /// <param name="obj"></param>
        /// <param name="isIdentity"></param>
        /// <returns></returns>
        public object Insert<T>(T obj, bool isIdentity = true) where T : class
        {
            Type type = obj.GetType();
            string tableName = GetTableNameByClassType(type.Name);
            StringBuilder insertSql = new StringBuilder();
            List<MySqlParameter> pars = new List<MySqlParameter>();
            var primaryKey = GetPrimaryKeyByTableName(this, tableName);
            string cacheSqlKey = "db.Insert." + tableName;
            var cacheSqlManager = CacheHelper<StringBuilder>.GetInstance(); //sql 缓存
            string cachePropertiesKey = "db." + tableName + ".GetProperties";
            var cachePropertiesManager = CacheHelper<PropertyInfo[]>.GetInstance(); // 属性缓存
>>>>>>> 8a023782e2a3a71e4337df9fa727810ef0f558af
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
<<<<<<< HEAD
            else {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            if (cacheSqlManager.ContainsKey(cacheSqlKey))
            {
                sbInsertSql = cacheSqlManager[cacheSqlKey];
            }
            else {
                var primaryKeyName = string.Empty;
                //2.获得实体的属性集合 
                //实例化一个StringBuilder做字符串的拼接 
                sbInsertSql.Append("INSERT INTO " + type.Name + " (");
                //3.遍历实体的属性集合 
                Func<PropertyInfo, bool> HasAutoIncrementAttr = prop => {
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
                    if (isIdentity) {
                        if (HasAutoIncrementAttr(prop))
                        {
                            //自增列 跳过
                            continue;
                        }
                    }
                    sbInsertSql.Append(prop.Name + ",");
                    sbParams.Append("?" + prop.Name + ",");
                    object val = prop.GetValue(entity, null);
                    if (val == null)
                        val = DBNull.Value;
                    pars.Add(new MySqlParameter("?" + prop.Name, val));
                }
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                sbParams.Remove(sbParams.Length - 1, 1);
                sbInsertSql.Append(" ) values(" + sbParams.ToString()+");");
                if (isIdentity) {
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
        public bool Update<T>(object rowObj, Expression<Func<T, bool>> expression) where T : class {
            if (rowObj == null) { throw new ArgumentNullException("SmartORMClient.Update.rowObj"); }
            if (expression == null) { throw new ArgumentNullException("SmartORMClient.Update.expression"); }
            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder(string.Format(" UPDATE {0} SET ", type.Name));
            Dictionary<string, string> rows = MySQLConvertHelper.GetObjectToDictionary(rowObj);
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
            re.AnalysisExpression(re, expression);
            sbSql.Append(re.SqlWhere);

            List<MySqlParameter> parsList = new List<MySqlParameter>();
            parsList.AddRange(re.Params);
            parsList.AddRange(rows.Select(c => new MySqlParameter("?" + c.Key, c.Value)));
            var updateRowCount = ExecuteCommand(sbSql.ToString(), parsList.ToArray());
            return updateRowCount > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Delete<T>(Expression<Func<T, bool>> expression) {
            Type type = typeof(T);
            LambdaExpressionAnalysis re = new LambdaExpressionAnalysis();
            re.AnalysisExpression(re, expression);
            string sql = string.Format("DELETE FROM {0} WHERE 1=1 {1}", type.Name, re.SqlWhere);
            bool isSuccess = ExecuteCommand(sql, re.Params.ToArray()) > 0;
            return isSuccess;
        }
=======
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            var isContainCacheSqlKey = cacheSqlManager.ContainsKey(cacheSqlKey);
            if (isContainCacheSqlKey)
            {
                insertSql = cacheSqlManager[cacheSqlKey];
            }
            else
            {
                //获得实体的属性集合 实例化一个StringBuilder做字符串的拼接 
                insertSql.Append("INSERT INTO " + tableName + " (");
                //遍历实体的属性集合 
                foreach (PropertyInfo prop in props)
                {
                    //EntityState,@EntityKey
                    if (!isIdentity || (isIdentity && prop.Name != primaryKey))
                    {
                        //将属性的名字加入到字符串中 
                        insertSql.Append("" + prop.Name + ",");
                    }
                }
                //**去掉最后一个逗号 
                insertSql.Remove(insertSql.Length - 1, 1);
                insertSql.Append(" ) values(");
            }
            //再次遍历，形成参数列表"(@xx,@xx@xx)"的形式 
            foreach (PropertyInfo prop in props)
            {
                //EntityState,@EntityKey
                if (isIdentity == false || (isIdentity && prop.Name != primaryKey))
                {
                    if (!cacheSqlManager.ContainsKey(cacheSqlKey))
                        insertSql.Append("@" + prop.Name + ",");
                    object val = prop.GetValue(obj, null);
                    if (val == null)
                        val = DBNull.Value;
                    var par = new MySqlParameter("@" + prop.Name, val);
                    pars.Add(par);
                }
            }
            if (!isContainCacheSqlKey)
            {
                //**去掉最后一个逗号 
                insertSql.Remove(insertSql.Length - 1, 1);
                if (isIdentity == false)
                {
                    insertSql.Append(");select 'true';");
                }
                else
                {
                    insertSql.Append(");select last_insert_id() as insertId;");
                }
                cacheSqlManager.Add(cacheSqlKey, insertSql, cacheSqlManager.Day);
            }
            var sql = insertSql.ToString();
            try
            {
                var lastInsertRowId = GetScalar(sql, pars.ToArray());
                return lastInsertRowId;
            }
            catch (Exception ex)
            {
                throw new Exception("sql:" + sql + "\n" + ex.Message);
            }
        }


>>>>>>> 8a023782e2a3a71e4337df9fa727810ef0f558af
    }
}
