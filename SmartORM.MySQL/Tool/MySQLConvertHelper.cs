using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SmartORM.MySQL.Tool
{
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
    }
}
