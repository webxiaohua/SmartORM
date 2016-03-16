using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.MySQL
{
    /// <summary>
    /// IsXXX 扩展函数
    /// </summary>
    public static class IsExtends
    {
        /// <summary>
        /// 验证某个对象是否在数组中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

        /// <summary>
        /// 是否有值
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool HasValue(this object thisValue)
        {
            if (thisValue == null) return false;
            return thisValue.ToString() != "";
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
