using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole
{
    public class StudentModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public int ClassID { get; set; }
        public string Name { get; set; }
    }

    [SmartORM.MySQL.TableName(TableName="sys_userinfo")]
    public class UserGenModel
    {
        [SmartORM.MySQL.AutoIncrement]
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public int UserStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public string UserRole { get; set; }
    }
}
