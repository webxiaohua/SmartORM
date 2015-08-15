using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace Smart.ORM.Helper
{
    /// <summary>
    /// @Author:Robin
    /// @Date:2015-08-10
    /// @Desc:SQL 类型转换
    /// </summary>
    internal class SQLConvertHelper
    {
        public static Type StringType = typeof(string);
        public static Type IntType = typeof(int);

        /// <summary>
        /// 将DataReader 转换为 List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="dataReader"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static List<T> DataReaderToList<T>(Type type, IDataReader dataReader, bool isClose = true)
        {
            var cacheHelper = CacheHelper<IDataReaderEntityBuilder<T>>.GetInstance();
            string key = "DataReaderToList." + type.FullName;
            IDataReaderEntityBuilder<T> eblist = null;
            if (cacheHelper.ContainsKey(key))
            {
                eblist = cacheHelper[key];
            }
            else
            {
                eblist = IDataReaderEntityBuilder<T>.CreateBuilder(type, dataReader);
                cacheHelper.Add(key, eblist, cacheHelper.Day);
            }
            List<T> list = new List<T>();
            try
            {
                if (dataReader == null) return list;
                while (dataReader.Read())
                {
                    list.Add(eblist.Build(dataReader));
                }
                if (isClose) { dataReader.Close(); dataReader.Dispose(); dataReader = null; }
            }
            catch (Exception)
            {
                if (isClose) { dataReader.Close(); dataReader.Dispose(); dataReader = null; }
            }
            return list;
        }


        /// <summary>
        /// 将实体对象转换成SqlParameter[] 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SqlParameter[] GetParameters(object obj)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var propertiesObj = type.GetProperties();
                string replaceGuid = Guid.NewGuid().ToString();
                foreach (PropertyInfo r in propertiesObj)
                {
                    var value = r.GetValue(obj, null);
                    if (value == null) value = DBNull.Value;
                    listParams.Add(new SqlParameter("@" + r.Name, value.ToString()));
                }
            }
            return listParams.ToArray();
        }

        /// <summary>
        /// 将实体对象转换成 Dictionary<string, string>
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

        /// <summary>
        /// 获取type属性cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachePropertiesKey"></param>
        /// <param name="cachePropertiesManager"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetGetPropertiesByCache(Type type, string cachePropertiesKey, CacheHelper<PropertyInfo[]> cachePropertiesManager)
        {
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            return props;
        }
    }
}
