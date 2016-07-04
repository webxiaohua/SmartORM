using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Reflection.Emit;
using System.Web;

namespace SmartORM.MySQL.Builder
{
    /// <summary>
    /// DataRow和IDataReader转换实体类
    /// http://www.cnblogs.com/piaopiao7891/p/3367844.html
    /// </summary>
    /// <typeparam name="ItemType"></typeparam>
    public class EmitEntityBuilder<ItemType>
    {
        #region 不可改变的参数
        private static readonly MethodInfo getRow =
                typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullRow =
                typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });

        private static readonly MethodInfo getRecord =
                typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullRecord =
                typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        #endregion

        #region  Emit 委托

        // 自定义转换实体委托
        public delegate ItemType DynamicMethodDelegate<T>(T paramObjs);

        private EmitEntityBuilder() { }
        /// <summary>
        /// 创建委托
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DynamicMethodDelegate<DataRow> CreateHandler(DataRow row)
        {
            System.Type itemType = typeof(ItemType);
            //定义一个名为DynamicCreate的动态方法，返回值typof(T)，参数typeof(IDataRecord)
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity",
                itemType,
                new Type[] { typeof(DataRow) },
                itemType, true);

            ILGenerator generator = method.GetILGenerator();  //创建一个MSIL生成器，为动态方法生成代码
            LocalBuilder result = generator.DeclareLocal(itemType);  //声明指定类型的局部变量 可以T t;这么理解
            generator.Emit(OpCodes.Newobj, itemType.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < row.ItemArray.Length; i++)
            {
                PropertyInfo propertyInfo
                    = itemType.GetProperty(row.Table.Columns[i].ColumnName);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullRow);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getRow);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            DynamicMethodDelegate<DataRow> handler
                   = (DynamicMethodDelegate<DataRow>)method.CreateDelegate(typeof(DynamicMethodDelegate<DataRow>));
            return handler;
        }

        public static DynamicMethodDelegate<DataRow> GetHandler(DataRow row)
        {
            string key = "Emit_Delegate_" + typeof(ItemType).Name;
            DynamicMethodDelegate<DataRow> handler = null;
            if (HttpRuntime.Cache[key] != null)
            {
                handler = HttpRuntime.Cache[key] as DynamicMethodDelegate<DataRow>;
            }
            else
            {
                handler = CreateHandler(row);
                HttpRuntime.Cache[key] = handler;
            }
            return handler;
        }

        public static DynamicMethodDelegate<IDataRecord> CreateHandler(IDataRecord dataRecord)
        {
            System.Type itemType = typeof(ItemType);
            //定义一个名为DynamicCreate的动态方法，返回值typof(T)，参数typeof(IDataRecord)
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity",
                    itemType,
                    new Type[] { typeof(IDataRecord) },
                    itemType, true);
            ILGenerator generator = method.GetILGenerator(); //创建一个MSIL生成器，为动态方法生成代码
            LocalBuilder result = generator.DeclareLocal(itemType); //声明指定类型的局部变量 可以T t;这么理解
            generator.Emit(OpCodes.Newobj, itemType.GetConstructor(Type.EmptyTypes)); //初始化对象
            generator.Emit(OpCodes.Stloc, result);//把局部变量的指针指向对象

            for (int i = 0; i < dataRecord.FieldCount; i++)  //数据集合，熟悉的for循环  
            {
                PropertyInfo propertyInfo
                    = itemType.GetProperty(dataRecord.GetName(i));  //根据列名取属性  原则上属性和列是一一对应的关系
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)  //实体存在该属性 且该属性有SetMethod方法
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullRecord);  //就知道这里要调用IsDBNull方法 如果IsDBNull==true contine
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    /*If the value in the data reader is not null, the code sets the value on the object.*/
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getRecord); //调用get_Item方法
                    generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
                    //generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod()); //给该属性设置对应值
                    generator.MarkLabel(endIfLabel);

                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            DynamicMethodDelegate<IDataRecord> handler
                   = (DynamicMethodDelegate<IDataRecord>)method.CreateDelegate(typeof(DynamicMethodDelegate<IDataRecord>));
            return handler;
        }

        public static DynamicMethodDelegate<IDataRecord> GetHandler(IDataRecord dataRecord)
        {
            string key = "Emit_Delegate_" + typeof(ItemType).Name;
            DynamicMethodDelegate<IDataRecord> handler = null;
            if (HttpRuntime.Cache[key] != null)
            {
                handler = HttpRuntime.Cache[key] as DynamicMethodDelegate<IDataRecord>;
            }
            else
            {
                handler = CreateHandler(dataRecord);
                HttpRuntime.Cache[key] = handler;
            }
            return handler;
        }

        #endregion


        #region 优化emit

        #endregion


        public static T DataReader2Obj<T>(IDataRecord dataRecord)
        {
            T t = System.Activator.CreateInstance<T>();
            Type obj = t.GetType();
            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                object tempValue = null;
                if (dataRecord[i] != DBNull.Value)
                {
                    string typeFullName = obj.GetProperty(dataRecord.GetName(i)).PropertyType.FullName;
                    tempValue = dataRecord.GetValue(i);
                    obj.GetProperty(dataRecord.GetName(i)).SetValue(t, tempValue, null);
                }
            }
            return t;
        }
    }


}
