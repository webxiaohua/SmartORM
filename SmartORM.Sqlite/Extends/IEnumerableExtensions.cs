using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.Sqlite
{
    /// <summary>
    /// IEmuberable 扩展
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 是否有值
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsValuable(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }
    }
}
