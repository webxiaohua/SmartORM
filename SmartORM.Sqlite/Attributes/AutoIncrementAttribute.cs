using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.Sqlite
{
    /// <summary>
    /// 自动增长
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}
