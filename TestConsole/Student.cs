using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SmartORM.MySQL;
using SmartORM.Sqlite;

namespace TestConsole
{
    public class Student
    {
        [AutoIncrement]
        [PrimaryKey]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
    }
}
