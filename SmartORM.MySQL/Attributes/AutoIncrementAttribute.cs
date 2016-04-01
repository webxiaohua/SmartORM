using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.MySQL
{
    /// <summary>
    /// 自动增长
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AutoIncrementAttribute:Attribute
    {
    }
}
