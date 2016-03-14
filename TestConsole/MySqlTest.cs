using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL;

namespace TestConsole
{
    public class Student
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
    }
    public class MySqlTest
    {
        static string connStr = "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=root";
        public static void TestGetList()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                List<Student> list = _db.Query<Student>("select * from Student", null).ToList();
                Console.WriteLine(list.Count);
            }
        }
    }
}
