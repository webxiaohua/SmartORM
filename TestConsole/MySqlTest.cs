using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartORM.MySQL;

namespace TestConsole
{
    [TableName(TableName = "sys_machine")]
    public class MachineGenModel
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string MachineName { get; set; }
        public int MachineStatus { get; set; }
        public DateTime LastStopTime { get; set; }
        public DateTime LastStartTime { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsWatch { get; set; }
        public string IPAddress { get; set; }
        public string ClientVersion { get; set; }
        public string ProgramVersion { get; set; }
        public bool UseAgent { get; set; }
    }

    [TableName(TableName = "machine_log")]
    public class MachineLogGenModel
    {
        [AutoIncrement]
        public int ID { get; set; }
        public string PayerName { get; set; }
        public string LogType { get; set; }
        public string LogContent { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    internal class MachineActiveModel
    {
        public string MachineName { get; set; }
        public long Counts { get; set; }
    }

    public class MySqlTest
    {
        //static string connStr = "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=root";
        //static string connStr = "server=localhost;database=monitor;user=root;password=root";
        static string connStr = "server=121.40.17.224;database=monitor;user=ctrip;password=Ctrip0@1;";
        public static void TestGetList()
        {
            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                /*
                List<MachineGenModel> list = _db.Queryable<MachineGenModel>().Where(c=>c.MachineName.Contains("zhifu1")).ToList();
                foreach (var item in list)
                {
                    Console.WriteLine(item.MachineName + ":" + (item.CreatedTime != null ? item.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss") : ""));
                }
                */

                object result = _db.GetScalar("select count(*) from (SELECT A.MachineName,A.MachineStatus,A.IsWatch,B.LastLogTime FROM sys_machine AS A LEFT JOIN (SELECT MAX(CreatedTime) AS LastLogTime,PayerName FROM machine_log GROUP BY PayerName) AS B ON A.MachineName = B.PayerName ) X where IsWatch = TRUE and ( MachineStatus=0 or ( MachineStatus=2 and LastLogTime > '" + DateTime.Now.AddMinutes(-3).ToString("yyyy-MM-dd HH:mm:ss") + "'))", null);
                Console.WriteLine(Convert.ToInt32(result));

                //List<Student> list = _db.Query<Student>("select * from Student", new { UserName = "Robin" }).ToList();
                /*
                List<Student> list = _db.Queryable<Student>().Where(c => c.ID != 2).OrderBy("UserName").OrderByDescing("UserEmail").PageIndex(2).PageSize(5).ToList();
                foreach (var item in list)
                {
                    Console.WriteLine(item.UserName + "\t" + item.UserEmail);
                }
                 * */
                //List<Student> list = _db.Queryable<Student>().Where(c => c.ID == 2).ToList();
                //object result = _db.Insert<Student>(new Student() { UserName = "Tony", UserPassword = "111", UserEmail = "Tony@123.com" });

                //Console.WriteLine(result.ToString());
                //_db.Queryable<Student>()

                /*
                string startTime = DateTime.Now.AddMinutes(-2).ToString("yyyy-MM-dd HH:mm:ss");//起始时间
                string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//结束时间
                string sqlFind = "select A.MachineName,B.Counts from sys_machine as A left join (select count(*) as Counts ,PayerName from machine_log where CreatedTime > '" + startTime + "' and CreatedTime < '" + endTime + "' and LogType in (6,7,8) GROUP BY PayerName order by PayerName ) as B on A.MachineName = B.PayerName where A.MachineStatus <> 1 and A.IsWatch = 1"; //  发送心跳包、支付成功、支付验证
                List<MachineActiveModel> list = _db.Query<MachineActiveModel>(sqlFind);
                foreach (var item in list)
                {
                    Console.WriteLine(item.MachineName + ":" + item.Counts);
                }
                */

            }
        }

        public static void TestUpdate2()
        {

            using (SmartORMClient _db = new SmartORMClient(connStr))
            {
                List<string> exceptionList = new List<string>() { "zhifu2", "zhifu3", "zhifu4" };
                //_db.Update<MachineGenModel>(new { MachineStatus = 2 }, o => exceptionList.Contains(o.MachineName) && o.MachineStatus == 0); //正常变为异常
                //_db.Update<MachineGenModel>(new { MachineStatus = 1 }, o => o.MachineName.Contains("zhifu1"));

                List<string> typelist = new List<string>() { "6", "7", "8" };
                DateTime dt = DateTime.Now.AddHours(-2);
                _db.Delete<MachineLogGenModel>(c => typelist.Contains(c.LogType) && c.CreatedTime < dt); //清空2小时前的心跳包日志

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
