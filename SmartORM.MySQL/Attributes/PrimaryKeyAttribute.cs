using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.MySQL
{
    /// <summary>
    /// 主键特性
    /// </summary>
    /// 可以使用多次，不能继承到派生类
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {

    }
}
