using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.Sqlite
{
    public static class ArrayExtends
    {
        /// <summary>
        /// 拼接数组
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="dot"></param>
        /// <returns></returns>
        public static string Explode(this string[] arr, string dot)
        {
            StringBuilder sb = new StringBuilder("");
            string result = "";
            foreach (string item in arr)
            {
                sb.Append(item + dot);
            }
            if (sb.Length != 0)
            {
                result = sb.ToString().Substring(0, sb.Length - 1);
            }
            return result;
        }
    }
}
