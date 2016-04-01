using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartORM.MySQL
{
    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=true)]
    public class PKAttribute:Attribute
    {

    }
}
