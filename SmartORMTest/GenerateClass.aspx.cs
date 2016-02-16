using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smart.ORM;
using Smart.ORM.Query;
using Models;

namespace SmartORMTest
{
    public partial class GenerateClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connStr = "Server=.;uid=sa;pwd=landa;database=ORMTest";
            using (SmartORMClient db = new SmartORMClient(connStr))
            {
                //db.ClassGenerating.CreateClassFiles(db, Server.MapPath("~/Models"), "Models");
                //object result = db.Insert<Student>(new Student() { Name = "张三", Sex = "男", ClassId = "09461", StudentNo = "0946101" });
                List<Student> result = db.Queryable<Student>().Where(c => c.Id > 1).ToList(); ;
            }
        }
    }
}