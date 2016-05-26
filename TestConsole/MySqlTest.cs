using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL;

namespace TestConsole
{

    public class MySqlTest
    {
        //static string connStr = "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=root";
        static string connStr = "server=localhost;database=monitor;user=root;password=root";
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
            /*
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                object result = _db.Insert<Student>(new Student() { UserName = "Jimmy6", UserPassword = "123456", UserEmail = "Jimmy6@landa.com" });
                Console.WriteLine(result.ToString());
            }
             * */
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                object result = _db.Insert<UserGenModel>(new UserGenModel() { LoginName = "abc", LoginPwd = "123456", UserName = "test", UserPhone = "111111", UserEmail = "", UserStatus = 1 });
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

        public static void TestQuery()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                List<StudentModel> list = _db.Query<StudentModel>("select A.*,B.Name from student as A left join Class as B on A.ClassID = B.ID");
                foreach (var item in list)
                {
                    Console.WriteLine(item.UserName + " " + item.Name);
                }

            }
        }

        public static void TestSingle()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                UserGenModel user = _db.Queryable<UserGenModel>().Where(c => c.UserName == "test").Single();
            }
        }
    }
}
