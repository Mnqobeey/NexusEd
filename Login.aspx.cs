using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;

namespace NexusEd
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                txtName.Focus();

                string userType = Session["SelectedUserType"] as string;
                if (userType != null)
                {
                    if (userType.Equals("Student", StringComparison.OrdinalIgnoreCase))
                    {
                        rbStudent.Checked = true;
                    }
                    else if (userType.Equals("Admin_Coordinator", StringComparison.OrdinalIgnoreCase))
                    {
                        rbAdmin.Checked = true;
                    }

                    Session["SelectedUserType"] = null;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string selectedUserType = string.Empty;


            if (rbStudent.Checked)
            {
                selectedUserType = "Student";
            }
            else if (rbAdmin.Checked)
            {
                selectedUserType = "Admin_Coordinator";
            }

            if (!string.IsNullOrEmpty(selectedUserType))
            {
                if (ValidateLogin(txtName.Text, txtPassword.Text, selectedUserType))
                {
                    FormsAuthentication.SetAuthCookie(txtName.Text, false);

                    Session["SelectedUserType"] = selectedUserType;

                    if (selectedUserType == "Student")
                    {
                        int studentId = GetStudentID(txtName.Text, txtPassword.Text);
                        Session["StudentID"] = studentId;
                    }

                    if (selectedUserType == "Student")
                    {
                        Response.Redirect("FeedbackCategory.aspx");
                    }
                    else if (selectedUserType == "Admin_Coordinator")
                    {
                        Response.Redirect("Admin.aspx");
                    }
                }
                else
                {
                    lblerrorMessage.Text = "Invalid username, password, or selected user type.";
                }
            }
            else
            {
                lblerrorMessage.Text = "Please select a user type.";
            }
        }




        private bool ValidateLogin(string username, string password, string selectedUserType)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                string query = string.Empty;

                if (selectedUserType == "Admin_Coordinator")
                {
                    query = "SELECT COUNT(*) FROM Admin_Coordinator WHERE AdminUsername = @username AND AdminPassword = @password";
                }
                else if (selectedUserType == "Student")
                {
                    query = "SELECT COUNT(*) FROM Student WHERE StudentUsername = @username AND StudentPassword = @password";
                }
                else
                {
                    lblerrorMessage.Text = "User type not found.";
                    return false;
                }

                using (SqlCommand cmd = new SqlCommand(query, db))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }



        private int GetStudentID(string username, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT StudentID FROM Student WHERE StudentUsername = @username AND StudentPassword = @password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception("StudentID could not be found.");
                }
            }
        }
    }
}