using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.Sqlite;

namespace TestConsole
{
    public class SqliteTest
    {
        static string connStr = @"Data Source=F:\SXH\Database\Sqlite\test1.s3db";

        public static void TestGetList()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                //List<Student> list = _db.Query<Student>("select * from Student", new { UserName = "Robin" }).ToList();
                List<Student> list = _db.Queryable<Student>().Where(c => c.ID != 3).OrderBy("UserName").OrderByDescing("UserEmail").PageIndex(1).PageSize(10).ToList();
                foreach (var item in list)
                {
                    Console.WriteLine(item.UserName + "\t" + item.UserEmail);
                }

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

        public static void TestAdd()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                object result = _db.Insert<Student>(new Student() { UserName = "Jimmy4", UserPassword = "123456", UserEmail = "Jimmy4@landa.com" });
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

    }
}
