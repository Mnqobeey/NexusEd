using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace NexusEd
{
    public partial class Home : System.Web.UI.Page
    {

        protected void btnShare_Click(object sender, EventArgs e)
        {
            Session["SelectedUserType"] = "student";
            Response.Redirect("Login.aspx");

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}