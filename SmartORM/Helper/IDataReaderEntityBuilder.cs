﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Reflection.Emit;

namespace Smart.ORM.Helper
{
    /// <summary>
    /// @Author:Robin
    /// @Date:2015-08-10
    /// @Desc:SQL DataReader 实体生成
    /// </summary>
    public class IDataReaderEntityBuilder<T>
    {
        private static readonly MethodInfo getValueMethod =
        typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod =
            typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private delegate T Load(IDataRecord dataRecord);
        private Load handler;

        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public static IDataReaderEntityBuilder<T> CreateBuilder(Type type, IDataRecord dataRecord)
        {

            {
                IDataReaderEntityBuilder<T> dynamicBuilder = new IDataReaderEntityBuilder<T>();
                DynamicMethod method = new DynamicMethod("DynamicCreateEntity", type,
                        new Type[] { typeof(IDataRecord) }, type, true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(type);
                generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = type.GetProperty(dataRecord.GetName(i));
                    Label endIfLabel = generator.DefineLabel();
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
                dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
                return dynamicBuilder;
            }
        }
    }
}
