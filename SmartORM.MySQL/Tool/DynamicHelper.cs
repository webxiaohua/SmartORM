using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Dynamic;

namespace SmartORM.MySQL.Tool
{
    /// <summary>
    /// 匿名类
    /// </summary>
    public class DynamicHelper
    {
        /// <summary>
        /// 将DataReader数据转为Dynamic对象
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static dynamic DataFillDynamic(IDataReader reader)
        {
            dynamic d = new ExpandoObject();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    ((IDictionary<string, object>)d).Add(reader.GetName(i), reader.GetValue(i));
                }
                catch
                {
                    ((IDictionary<string, object>)d).Add(reader.GetName(i), null);
                }
            }
            return d;
        }

        /// <summary>
        /// 获取模型对象集合
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<dynamic> DataFillDynamicList(IDataReader reader)
        {
            List<dynamic> list = new List<dynamic>();
            if (reader != null && !reader.IsClosed)
            {
                while (reader.Read())
                {
                    list.Add(DataFillDynamic(reader));
                }
                reader.Close();
            }
            return list;
        }
    }
}
