using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Data;
using SmartORM.MySQL.Builder;

namespace SmartORM.MySQL
{
    public static class ExtensionFunctions
    {
        /// <summary>
        /// 验证是否相等，跳过单步调试
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsSameAs(this string str1, string str2)
        {
            return string.Compare(str1, str2, true) == 0;
        }

        /// <summary>
        /// 获取要查找的字段 select a,b from xxx  => a,b
        /// </summary>
        /// <param name="selectFields"></param>
        /// <returns></returns>
        public static string GetSelectFields(this string selectFields)
        {
            return selectFields.IsNullOrEmpty() ? "*" : selectFields;
        }

        #region xml
        /// <summary>
        /// XElement的定义中查询包含name的XObject枚举
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<XObject> FindName(this XElement source, string name)
        {
            if (source.Attributes().Any())
            {
                foreach (XAttribute att in source.Attributes())
                {
                    string attname = att.Name.ToString();
                    if (attname.Contains(name))
                        yield return att;
                }
            }
            if (source.Elements().Any())
            {
                foreach (XElement child in source.Elements())
                    foreach (XObject o in child.FindName(name))
                        yield return o;
            }
            else
            {
                string contents = source.Name.ToString();
                if (contents.Contains(name))
                    yield return source;
            }
        }
        /// <summary>
        /// XElement的值中查询包含value的XObject枚举
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<XObject> FindValue(this XElement source, string value)
        {
            if (source.Attributes().Any())
            {
                foreach (XAttribute att in source.Attributes())
                {
                    string Contents = (string)att;
                    if (Contents.Contains(value))
                        yield return att;
                }
            }
            if (source.Elements().Any())
            {
                foreach (XElement child in source.Elements())
                {
                    foreach (XObject o in child.FindValue(value))
                        yield return o;
                }
            }
            else
            {
                string Contents = (string)source;
                if (Contents.Contains(value))
                    yield return source;

            }
        }

        /// <summary>
        /// 转xml
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string Xml(this XElement element)
        {
            StringBuilder buffer = new StringBuilder(1000);
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(buffer));
            element.WriteTo(writer);
            return buffer.ToString();
        }

        /// <summary>
        /// 转xml
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>       
        public static string ToXml(this XmlDocument doc)
        {
            StringBuilder buffer = new StringBuilder(1000);
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(buffer));
            doc.WriteContentTo(writer);
            return buffer.ToString();
        }
        #endregion

        #region IDataReader
        /// <summary>
        /// 转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IDataReader reader)
        {
            return reader.ToList<T>(true);
        }

        /// <summary>
        /// 转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IDataReader reader, bool isClose)
        {
            List<T> list = new List<T>();
            if (reader == null) return list;

            EmitEntityBuilder<T>.DynamicMethodDelegate<IDataRecord> handler
                = EmitEntityBuilder<T>.CreateHandler(reader);
            while (reader.Read())
            {
                list.Add(handler(reader));
            }
            if (isClose) { reader.Close(); reader.Dispose(); reader = null; }
            return list;
        }

        #endregion

        #region DataTable
        /// <summary>
        /// 转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>         
        public static List<T> ToList<T>(this DataTable dt)
            where T : class, new()
        {
            List<T> list = new List<T>();
            if (dt == null) return list;
            //构造转换方法的委托
            EmitEntityBuilder<T>.DynamicMethodDelegate<DataRow> handler
                = EmitEntityBuilder<T>.CreateHandler(dt.Rows[0]);
            foreach (DataRow info in dt.Rows)
            {
                list.Add(handler(info));
            }
            dt.Dispose(); dt = null;
            return list;
        }
        #endregion

    }
}
