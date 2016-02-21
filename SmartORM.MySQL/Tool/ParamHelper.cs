using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace SmartORM.MySQL.Tool
{
    /// <summary>
    /// 参数辅助类
    /// </summary>
    public class ParamHelper
    {
        /// <summary>
        /// 根据对象获取参数数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public MySqlParameter[] GetParameters(object obj)
        {
            List<MySqlParameter> listParams = new List<MySqlParameter>();
            if (obj != null) {
                var type = obj.GetType();
                var propertyList = type.GetProperties();
                foreach (PropertyInfo p in propertyList)
                {
                    var value = p.GetValue(obj,null);
                    if (value == null) {
                        value = DBNull.Value;
                    }
                    listParams.Add(new MySqlParameter("@" + p.Name, value.ToString()));
                }
            }
            return listParams.ToArray();
        }
    }
}
