using System;
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
                        new Type[] { typeof(IDataRecord) }, type, true);    //构造动态IL代码  method  方法名，返回类型，参数类型 动态方法逻辑关联的类型 是否跳过JIT可见性检查
                ILGenerator generator = method.GetILGenerator();//IL代码生成器
                LocalBuilder result = generator.DeclareLocal(type);//局部变量 
                generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));//new obj 
                generator.Emit(OpCodes.Stloc, result);//从堆栈顶部弹出当前值，并将其存储到指定索引出的局部变量列表中
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = type.GetProperty(dataRecord.GetName(i));
                    Label endIfLabel = generator.DefineLabel(); //指令流中的标签  与 ILGenerator 类一起使用。
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);//将索引为 0 的参数加载到计算堆栈上。
                        generator.Emit(OpCodes.Ldc_I4, i);//将所提供的 int32 类型的值作为 int32 推送到计算堆栈上。
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);//对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
                        generator.Emit(OpCodes.Brtrue, endIfLabel);//如果 value 为 true、非空或非零，则将控制转移到目标指令。
                        generator.Emit(OpCodes.Ldloc, result);//将指定索引处的局部变量加载到计算堆栈上。
                        generator.Emit(OpCodes.Ldarg_0);//将索引为 0 的参数加载到计算堆栈上。
                        generator.Emit(OpCodes.Ldc_I4, i);//将所提供的 int32 类型的值作为 int32 推送到计算堆栈上。
                        generator.Emit(OpCodes.Callvirt, getValueMethod);//对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
                        generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));//将指令中指定类型的已装箱的表示形式转换成未装箱形式。
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());//对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
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
