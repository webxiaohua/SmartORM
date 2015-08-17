using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smart.ORM.SQLExtends
{
    /// <summary>
    /// @Autor:Robin
    /// @Date:2015-08-10
    /// @Desc:SQL 辅助类
    /// </summary>
    public class SQLable
    {
        /// <summary>
        /// 数据库接口
        /// </summary>
        public SmartORMClient DB = null;
        /// <summary>
        /// SQL临时语句
        /// </summary>
        public StringBuilder Sql { get; set; }
        /// <summary>
        /// Where 语句
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// Order By 临时数据
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// Group By 临时数据
        /// </summary>
        public string GroupBy { get; set; }
    }
}
