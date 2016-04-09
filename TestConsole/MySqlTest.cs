using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL;

namespace TestConsole
{
    
    public class MySqlTest
    {
        static string connStr = "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=root";
        public static void TestGetList()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                //List<Student> list = _db.Query<Student>("select * from Student", new { UserName = "Robin" }).ToList();
                List<Student> list = _db.Queryable<Student>().Where(c => c.ID != 2).OrderBy("UserName").OrderByDescing("UserEmail").PageIndex(2).PageSize(5).ToList();
                foreach (var item in list)
                {
                    Console.WriteLine(item.UserName + "\t" + item.UserEmail);
                }
                //List<Student> list = _db.Queryable<Student>().Where(c => c.ID == 2).ToList();
                //object result = _db.Insert<Student>(new Student() { UserName = "Tony", UserPassword = "111", UserEmail = "Tony@123.com" });

                //Console.WriteLine(result.ToString());
                //_db.Queryable<Student>()

            }
        }

        public static void TestAdd()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                object result = _db.Insert<Student>(new Student() { UserName = "Jimmy6", UserPassword = "123456", UserEmail = "Jimmy6@landa.com" });
                Console.WriteLine(result.ToString());
            }
        }

        public static void TestUpdate()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                bool result = _db.Update<Student>(new { UserName = "Robinson2" }, o => o.ID == 1);
                Console.WriteLine(result.ToString());
            }
        }

        public static void TestDelete()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                bool result = _db.Delete<Student>(o => o.ID == 2);
                Console.WriteLine(result.ToString());
            }
        }

    }
}
