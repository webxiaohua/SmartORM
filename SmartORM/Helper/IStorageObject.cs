using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smart.ORM.Helper
{
    /// <summary>
    /// @Author:Robin
    /// @Date:2015-08-10
    /// @Desc:存储对象接口
    /// </summary>
    internal abstract class IStorageObject<V>
    {
        public int Minutes = 60;
        public int Hour = 60 * 60;
        public int Day = 60 * 60 * 24;
        public System.Web.HttpContext context = System.Web.HttpContext.Current;
        public abstract void Add(string key, V value);
        public abstract bool ContainsKey(string key);
        public abstract V Get(string key);
        public abstract IEnumerable<string> GetAllKey();
        public abstract void Remove(string key);
        public abstract void RemoveAll();
        public abstract void RemoveAll(Func<string, bool> removeExpression);
        public abstract V this[string key] { get; }
    }
}
