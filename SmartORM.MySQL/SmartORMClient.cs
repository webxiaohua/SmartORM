using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL.Core;
using MySql.Data.MySqlClient;
using SmartORM.MySQL.Tool;

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
        }

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
    }
}
