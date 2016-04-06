using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace SmartORM.Sqlite.Tool
{
    public class SQLConvertHelper
    {
        /// <summary>
        /// DataReader 转换成 List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="dataReader"></param>
        /// <param name="fields"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static List<T> DataReaderToList<T>(Type type, IDataReader dataReader, string fields, bool isClose = true)
        {
            return null;
        }

        /// <summary>
        /// 把对象属性转换成dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetObjectToDictionary(object obj)
        {

            Dictionary<string, string> reval = new Dictionary<string, string>();
            if (obj == null) return reval;
            var type = obj.GetType();
            var propertiesObj = type.GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                var val = r.GetValue(obj, null);
                reval.Add(r.Name, val == null ? "" : val.ToString());
            }
            return reval;
        }
    }
}
