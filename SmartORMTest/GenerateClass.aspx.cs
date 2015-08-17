using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smart.ORM;

namespace SmartORMTest
{
    public partial class GenerateClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connStr = "Server=.;uid=sa;pwd=landa;database=ORMTest";
            using (SmartORMClient db = new SmartORMClient(connStr))
            {
                db.ClassGenerating.CreateClassFiles(db, Server.MapPath("~/Models"), "Models");
            }
        }
    }
}