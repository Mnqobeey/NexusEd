using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthNavigation.RequireAdmin(this))
            {
                return;
            }

            AuthNavigation.Configure(this, Menu1);
        }

        protected void btnCat_Click(object sender, EventArgs e)
        {
            Response.Redirect("CategoryType.aspx");
        }

        protected void btnQues_Click(object sender, EventArgs e)
        {
            Response.Redirect("Questions.aspx");
        }

        protected void btnDash_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx");
        }
    }
}
