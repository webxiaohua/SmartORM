using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Smart.ORM.Helper;
using System.Data.SqlClient;

namespace Smart.ORM.Tool
{
    /// <summary>
    /// @Autor:Robin
    /// @Date:2015-08-10
    /// @Desc:SQL ORM 生成类
    /// </summary>
    public class ClassGenerating
    {
        /// <summary>
        /// 根据匿名类获取实体类的字符串
        /// </summary>
        /// <param name="entity">匿名对象</param>
        /// <param name="className">生成的类名</param>
        /// <returns></returns>
        public string DynamicToClass(object entity, string className)
        {
            StringBuilder reval = new StringBuilder();
            StringBuilder propertiesValue = new StringBuilder();
            var propertiesObj = entity.GetType().GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            string nullable = string.Empty;
            foreach (var r in propertiesObj)
            {

                var type = r.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                    nullable = "?";
                }
                if (!type.Namespace.Contains("System.Collections.Generic"))
                {
                    propertiesValue.AppendLine();
                    string typeName = ChangeType(type);
                    propertiesValue.AppendFormat("public {0}{3} {1} {2}", typeName, r.Name, "{get;set;}", nullable);
                    propertiesValue.AppendLine();
                }
            }

            reval.AppendFormat(@"
                 public class {0}{{
                        {1}
                 }}
            ", className, propertiesValue);


            return reval.ToString();
        }


        /// <summary>
        /// 根据DataTable获取实体类的字符串
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string DataTableToClass(DataTable dt, string className, string nameSpace = null, params DataRow[] allColumns)
        {
            StringBuilder reval = new StringBuilder();
            StringBuilder propertiesValue = new StringBuilder();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (DataColumn r in dt.Columns)
            {
                propertiesValue.AppendLine();
                string typeName = ChangeType(r.DataType);
                string columnDesc = "";
                DataRow drItem = allColumns.Where(c => c.ItemArray[2].ToString() == r.ColumnName).FirstOrDefault();
                if (drItem != null)
                {
                    columnDesc = drItem["ColumnDesc"].ToString();
                }
                string summary = string.Format(@"/// <summary>
        /// {0}
        /// </summary>
", columnDesc);
                propertiesValue.AppendFormat(@"        {0}        public {1} {2} {3}", summary, typeName, r.ColumnName, "{ get; set; }");
                propertiesValue.AppendLine();
            }
            reval.AppendFormat(@"   public class {0}
    {{
        {1}
   }}
            ", className, propertiesValue);

            if (nameSpace != null)
            {
                return string.Format(@"using System;
namespace {1}
{{
 {0}
}}", reval.ToString(), nameSpace);
            }
            else
            {
                return reval.ToString();
            }
        }


        /// <summary>
        /// 根据SQL语句获取实体类的字符串
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string SqlToClass(CtripORMClient db, string sql, string className)
        {
            using (SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = sql;
                DataTable dt = new DataTable();
                SqlDataAdapter sad = new SqlDataAdapter(command);
                sad.Fill(dt);
                var reval = DataTableToClass(dt, className);
                return reval;
            }
        }
        /// <summary>
        /// 根据表名获取实体类的字符串
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string TableNameToClass(CtripORMClient db, string tableName)
        {
            var dt = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
            var reval = DataTableToClass(dt, tableName);
            return reval;
        }



        /// <summary>
        ///  创建SQL实体文件
        /// </summary>
        public void CreateClassFiles(CtripORMClient db, string fileDirectory, string nameSpace = null)
        {
            //查询表字段结构
            string queryTablesDetail = @"select
    [TableName]=c.Name,
    [TableDesc]=isnull(f.[value],''),
    [ColumnName]=a.Name,
    [ColumnNum]=a.Column_id,
    [Identity]=case when is_identity=1 then '√' else '' end,
    [PK]=case when exists(select 1 from sys.objects where parent_object_id=a.object_id and type=N'PK' and name in
                    (select Name from sys.indexes where index_id in
                    (select indid from sysindexkeys where colid=a.column_id)))
                    then '√' else '' end,
    [Type]=b.Name,
    [ByteSize]=case when a.[max_length]=-1 and b.Name!='xml' then 'max/2G'
            when b.Name='xml' then ' 2^31-1 Bytes/2G'
            else rtrim(a.[max_length]) end,
    [Length]=ColumnProperty(a.object_id,a.Name,'Precision'),
    [IsNull]=case when a.is_nullable=1 then '√' else '' end,
    [ColumnDesc]=isnull(e.[value],''),
    [DefaultVal]=isnull(d.text,'')    
from
    sys.columns a
left join
    sys.types b on a.user_type_id=b.user_type_id
inner join
    sys.objects c on a.object_id=c.object_id and c.Type='U'
left join
    syscomments d on a.default_object_id=d.ID
left join
    sys.extended_properties e on e.major_id=c.object_id and e.minor_id=a.Column_id and e.class=1 
left join
    sys.extended_properties f on f.major_id=c.object_id and f.minor_id=0 and f.class=1";
            var tables = db.GetDataTable("select name from sysobjects where xtype='U'");
            var columns = db.GetDataTable(queryTablesDetail);
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString();
                    var currentTable = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
                    var classCode = DataTableToClass(currentTable, tableName, nameSpace, columns.Select("TableName = '" + tableName + "'"));
                    FileHelper.WriteText(fileDirectory.TrimEnd('\\') + "\\" + tableName + ".cs", classCode);
                }
            }
        }

        /// <summary>
        ///  创建SQL实体文件,指定表名
        /// </summary>
        public void CreateClassFilesByTableNames(CtripORMClient db, string fileDirectory, string nameSpace, params string[] tableNames)
        {
            var tables = db.GetDataTable("select name from sysobjects where xtype='U'");
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString().ToLower();
                    if (tableNames.Any(it => it.ToLower() == tableName))
                    {
                        var currentTable = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
                        var classCode = DataTableToClass(currentTable, tableName, nameSpace);
                        FileHelper.WriteText(fileDirectory.TrimEnd('\\') + "\\" + tableName + ".cs", classCode);
                    }
                }
            }
        }

        /// <summary>
        /// 匹配类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ChangeType(Type type)
        {
            string typeName = type.Name;
            switch (typeName)
            {
                case "Int32": typeName = "int"; break;
                case "String": typeName = "string"; break;
            }
            return typeName;
        }
    }
}
