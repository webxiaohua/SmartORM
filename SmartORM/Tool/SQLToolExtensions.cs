using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smart.ORM.Tool
{
    /// <summary>
    /// @Autor:Robin
    /// @Date:2015-08-10
    /// @Desc:ORM 扩展工具类
    /// </summary>
    internal static class SQLToolExtensions
    {
        /// <summary>
        /// 数组字串转换成SQL参数格式，例如: 参数 new int{1,2,3} 反回 "'1','2','3'"
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJoinSQLInVal<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ToSQLValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSuperSQLFilter().ToSQLValue()));
            }
        }

        /// <summary>
        /// 将字符串转换成SQL参数格式，例如: 参数value返回'value'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSQLValue(this string value)
        {
            return string.Format("'{0}'", value.ToSQLFilter());
        }

        /// <summary>
        /// SQL关键字过滤,过滤拉姆达式中的特殊字符，出现特殊字符则引发异常
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSQLFilter(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                if (Regex.IsMatch(value, @"'|%|0x|(\@.*\=)", RegexOptions.IgnoreCase))
                {
                    throw new SQLSecurityException("查询参数不允许存在特殊字符。");
                }
            }
            return value;
        }

        /// <summary>
        ///  指定数据类型，如果不在指定类当中则引发异常(只允许输入指定字母、数字、下划线、时间、GUID)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSuperSQLFilter(this string value)
        {
            if (value.IsNullOrEmpty()) return value;
            if (Regex.IsMatch(value, @"^(\w|\.|\:|\-| |\,)+$"))
            {
                return value;
            }
            throw new SQLSecurityException("指定类型(只允许输入指定字母、数字、下划线、时间、guid)。");
        }

        /// <summary>
        /// 获取锁字符串
        /// </summary>
        /// <param name="isNoLock"></param>
        /// <returns></returns>
        public static string GetLockString(this bool isNoLock)
        {
            return isNoLock ? "WITH(NOLOCK)" : null; ;
        }
        /// <summary>
        /// 获取Select需要的字段
        /// </summary>
        /// <param name="selectFileds"></param>
        /// <returns></returns>
        public static string GetSelectFiles(this string selectFileds)
        {
            return selectFileds.IsNullOrEmpty() ? "*" : selectFileds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupByFileds"></param>
        /// <returns></returns>
        public static string GetGroupBy(this string groupByFileds)
        {
            return groupByFileds.IsNullOrEmpty() ? "" : " GROUP BY " + groupByFileds;
        }
    }
}
