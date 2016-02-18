using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SmartORM.MySQL.Core
{
    /// <summary>
    /// @Author:Robin
    /// @Date:2016-02-17
    /// @Desc:mysql 数据库操作类
    /// </summary>
    public class MySQLHelper : IDisposable
    {
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
            cmd.Parameters.Add(parms);
            MySqlDataReader sqlDataReader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return sqlDataReader;
        }

        public List<T> GetList<T>(string sql, params MySqlParameter[] parms)
        {
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
