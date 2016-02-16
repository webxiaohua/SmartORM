﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Smart.ORM.Helper
{
    /// <summary>
    /// @Author:Robin
    /// @Date:2015-08-10
    /// @Desc:MSSQL数据库操作辅助类
    /// </summary>
    public class SQLHelper : IDisposable
    {
        SqlConnection _sqlConnection;
        SqlTransaction _tran = null;

        public SQLHelper(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }
        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
        {
            return _sqlConnection;
        }
        /// <summary>
        /// 启动事务
        /// </summary>
        public void BeginTran()
        {
            _tran = _sqlConnection.BeginTransaction();
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollbackTran()
        {
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
        }
        /// <summary>
        /// 获取String类型的返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public string GetString(string sql, params SqlParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }
        /// <summary>
        /// 获取Int类型的返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int GetInt(string sql, params SqlParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }
        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public object GetScalar(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.Parameters.AddRange(pars);
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            sqlCommand.Parameters.Clear();
            return scalar;
        }
        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.Parameters.AddRange(pars);
            int count = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return count;
        }
        /// <summary>
        /// 获取DataReader对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.Parameters.AddRange(pars);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            sqlCommand.Parameters.Clear();
            return sqlDataReader;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string sql, params SqlParameter[] pars)
        {
            var reval = SQLConvertHelper.DataReaderToList<T>(typeof(T), GetReader(sql, pars),null);
            return reval;
        }
        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public T GetSingle<T>(string sql, params SqlParameter[] pars)
        {
            var reval = SQLConvertHelper.DataReaderToList<T>(typeof(T), GetReader(sql, pars),null).Single();
            return reval;
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params SqlParameter[] pars)
        {
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return dt;
        }

        public DataSet GetDataSetAll(string sql, params SqlParameter[] pars)
        {
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            DataSet ds = new DataSet();
            _sqlDataAdapter.Fill(ds);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return ds;
        }

        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _sqlConnection.Close();
                }
            }
        }
    }
}
