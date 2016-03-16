using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;

namespace SmartORM.MySQL.Core
{
    /// <summary>
    /// @Author:Robin
    /// @Date:2016-02-17
    /// @Desc:mysql 数据库操作类
    /// </summary>
    public class MySQLHelper : IDisposable
    {
        private bool disposed = false;
        MySqlConnection _sqlConnection;
        /// <summary>
        /// 构造  生成sqlconnection实例并且打开连接
        /// </summary>
        /// <param name="connectionString"></param>
        public MySQLHelper(string connectionString)
        {
            _sqlConnection = new MySqlConnection(connectionString);
            _sqlConnection.Open();
        }

        public string GetString(string sql, params MySqlParameter[] parms)
        {
            return Convert.ToString(GetScalar(sql, parms));
        }

        /// <summary>
        /// 返回首行首列结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public object GetScalar(string sql, params MySqlParameter[] parms)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, _sqlConnection);
            sqlCommand.Parameters.AddRange(parms);
            object scalar = sqlCommand.ExecuteScalar();
            sqlCommand.Parameters.Clear();
            return scalar;
        }
        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, params MySqlParameter[] parms)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, _sqlConnection);
            sqlCommand.Parameters.AddRange(parms);
            int count = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return count;
        }

        /// <summary>
        /// 获取SQLDataReader对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public MySqlDataReader GetReader(string sql, params MySqlParameter[] parms)
        {
            MySqlCommand cmd = new MySqlCommand(sql, _sqlConnection);
            if (parms.Count() != 0) cmd.Parameters.AddRange(parms);
            MySqlDataReader sqlDataReader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return sqlDataReader;
        }

        /// <summary>
        /// T 必须声明为引用类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        //public static List<T> ToList<T>(this IDataReader reader) where T : class, new()
        //{
        //    return reader.ToList<T>();
        //}

        /// <summary>
        /// 根据对象获取参数数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public MySqlParameter[] GetParameters(object obj)
        {
            List<MySqlParameter> listParams = new List<MySqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var propertyList = type.GetProperties();
                foreach (PropertyInfo p in propertyList)
                {
                    var value = p.GetValue(obj, null);
                    if (value == null)
                    {
                        value = DBNull.Value;
                    }
                    listParams.Add(new MySqlParameter("?" + p.Name, value.ToString()));
                }
            }
            return listParams.ToArray();
        }

        /// <summary>
        /// 获取datareader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public MySqlDataReader GetDataReader(string sql, MySqlParameter[] parms)
        {
            MySqlCommand cmd = new MySqlCommand(sql, _sqlConnection);
            if (parms.Count() != 0) cmd.Parameters.Add(parms);
            MySqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                //清理托管资源
            }
            //清理非托管资源
            if (_sqlConnection != null)
            {
                _sqlConnection.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);//通知垃圾回收机制不再调用析构器
        }

        ~MySQLHelper()
        {
            Dispose(false);//不用清理托管资源
        }
    }
}
