using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using SmartORM.MySQL.Tool;

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
        public int CommandTimeOut = 30000; //数据库命令执行超时时间
        MySqlTransaction _tran = null; //事务

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

        public void BeginTran()
        {
            _tran = _sqlConnection.BeginTransaction();
        }
        public void RollbackTran()
        {
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }
        public void CommitTran()
        {
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
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

        private List<KeyValuePair> _mappingTableList = null;

        /// <summary>
        /// 根据对象类型名称获取表名
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal string GetTableNameByClassType(string typeName)
        {
            if (_mappingTableList.IsValuable())
            {
                if (_mappingTableList.Any(c => c.Key == typeName))
                {
                    typeName = _mappingTableList.First(c => c.Key == typeName).Value;
                }
            }
            return typeName;
        }

        /// <summary>
        /// 设置类名和表名的映射  key 为类名  value 为表名
        /// </summary>
        /// <param name="mappingTables"></param>
        public void SetMappingTables(List<KeyValuePair> mappingTables)
        {
            if (mappingTables.IsValuable())
            {
                _mappingTableList = mappingTables;
            }
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

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params MySqlParameter[] pars)
        {
            MySqlDataAdapter _dataAdapter = new MySqlDataAdapter(sql, _sqlConnection);
            _dataAdapter.SelectCommand.Parameters.AddRange(pars);
            _dataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                _dataAdapter.SelectCommand.Transaction = _tran;
            }
            DataTable dt = new DataTable();
            _dataAdapter.Fill(dt);
            _dataAdapter.SelectCommand.Parameters.Clear();
            return dt;
        }

        #region 辅助函数

        /// <summary>
        /// 获取实体类的主键
        /// </summary>
        /// <returns></returns>
        internal static string GetPrimaryKeyByType(Type type) {
            PropertyInfo[] properties = type.GetProperties();
            foreach (var item in properties)
            {
                object[] attributes = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
            }
        }
        /*
        internal static string GetPrimaryKeyByTableName(SmartORMClient db, string tableName)
        {
            string key = "GetPrimaryKeyByTableName";
            tableName = tableName.ToLower();
            var cacheHelper = CacheHelper<List<KeyValuePair>>.GetInstance();
            List<KeyValuePair> primaryInfo = null;
            //获取主键信息
            if (cacheHelper.ContainsKey(key))
                primaryInfo = cacheHelper[key];
            else
            {


                string sql = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE CONSTRAINT_SCHEMA = @DBName";
                MySqlParameter[] parms = new MySqlParameter[] { 
                    new MySqlParameter("@DBName",SqlDbType.VarChar)
                };
                parms[0].Value = db.DBName;
                var dt = db.GetDataTable(sql, parms);
                primaryInfo = new List<KeyValuePair>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        primaryInfo.Add(new KeyValuePair() { Key = dr["TABLE_NAME"].ToString().ToLower(), Value = dr["COLUMN_NAME"].ToString() });
                    }
                }
                cacheHelper.Add(key, primaryInfo);
                
            }
            //反回主键
            if (!primaryInfo.Any(it => it.Key == tableName))
            {
                return null;
            }
            return primaryInfo.First(it => it.Key == tableName).Value;
        }
        */
        #endregion

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
