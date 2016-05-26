using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.Sqlite
{
    /// <summary>
    /// 表名特性，指定类对应的表名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
    }
}
