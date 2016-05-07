using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace SmartORM.MySQL.Core
{
    /// <summary>
    /// Queryable 操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Queryable<T>
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TName { get { return typeof(T).Name; } }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get { return typeof(T); } }

        /// <summary>
        /// 数据实例
        /// </summary>
        public SmartORMClient DB = null;

        /// <summary>
        /// Where 临时数据
        /// </summary>
        public List<string> Where = new List<string>();

        //public int? Skip { get; set; }
        public int PageIndex { get; set; }

        //public int? Take { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; }

        public string Select { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public List<MySqlParameter> Params = new List<MySqlParameter>();
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 分组查询
        /// </summary>
        public string GroupBy { get; set; }

    }
}
