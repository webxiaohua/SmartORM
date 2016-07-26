using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace SmartORM.MySQL.Tool
{
    public class DBTypeValue
    {
        public DbType DBType { get; set; }
        public object DBValue { get; set; }
    }

    public class MySQLConvertHelper
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
        public static Dictionary<string, DBTypeValue> GetObjectToDictionary(object obj)
        {
            Dictionary<string, DBTypeValue> reval = new Dictionary<string, DBTypeValue>();
            if (obj == null) return reval;
            var type = obj.GetType();
            var propertiesObj = type.GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                DBTypeValue dbTypeValue = new DBTypeValue();
                var val = r.GetValue(obj, null);
                string valType = r.GetType().ToString();
                switch ((r.PropertyType).FullName)
                {
                    case "System.Int32":
                        dbTypeValue.DBType = DbType.Int32;
                        break;
                    case "System.Int64":
                        dbTypeValue.DBType = DbType.Int64;
                        break;
                    case "System.String":
                        dbTypeValue.DBType = DbType.String;
                        break;
                    case "System.Boolean":
                        dbTypeValue.DBType = DbType.Boolean;
                        break;
                    case "System.DateTime":
                        dbTypeValue.DBType = DbType.DateTime;
                        break;
                    case "System.Double":
                        dbTypeValue.DBType = DbType.Double;
                        break;
                    case "System.Decimal":
                        dbTypeValue.DBType = DbType.Decimal;
                        break;
                    case "System.Guid":
                        dbTypeValue.DBType = DbType.Guid;
                        break;
                    default:
                        dbTypeValue.DBType = DbType.String;
                        break;
                }
                dbTypeValue.DBValue = val;
                reval.Add(r.Name, val == null ? null : dbTypeValue);
            }
            return reval;
        }
    }
}
