using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smart.ORM.Tool
{
    /// <summary>
    /// @Autor:Robin
    /// @Date:2015-08-10
    /// @Desc:SQL 安全类异常
    /// </summary>
    public class SQLSecurityException : Exception
    {
        public SQLSecurityException(string message)
            : base(message)
        {

        }
    }
}
