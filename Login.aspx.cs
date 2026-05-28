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
            AuthNavigation.Configure(this, Menu1);
            ShowAccessNotice();


            if (!IsPostBack)
            {
                txtName.Focus();

                string userType = Session["PreferredUserType"] as string;
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

                    Session["PreferredUserType"] = null;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblerrorMessage.Text = string.Empty;

            if (!Page.IsValid)
            {
                lblerrorMessage.Text = "Please enter your username and password.";
                return;
            }

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

                    Response.Redirect(GetPostLoginRedirect(selectedUserType));
                }
                else
                {
                    lblerrorMessage.Text = "Incorrect login details. Check your username, password, and selected role.";
                }
            }
            else
            {
                lblerrorMessage.Text = "Please select Student or Administrator before signing in.";
            }
        }

        private void ShowAccessNotice()
        {
            string messageKey = Request.QueryString["msg"];
            string message = string.Empty;

            if (string.Equals(messageKey, "loginRequired", StringComparison.OrdinalIgnoreCase))
            {
                message = "Please login first to continue.";
            }
            else if (string.Equals(messageKey, "unauthorized", StringComparison.OrdinalIgnoreCase))
            {
                message = "You do not have permission to access that page.";
            }

            if (string.IsNullOrEmpty(message))
            {
                loginNotice.Visible = false;
                return;
            }

            litLoginNoticeMessage.Text = Server.HtmlEncode(message);
            loginNotice.Visible = true;
        }

        private string GetPostLoginRedirect(string selectedUserType)
        {
            string returnUrl = Request.QueryString["ReturnUrl"];
            if (AuthNavigation.IsReturnUrlAllowedForRole(returnUrl, selectedUserType))
            {
                return AuthNavigation.NormalizeReturnUrl(returnUrl);
            }

            return AuthNavigation.GetDefaultLandingUrl(selectedUserType);
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
                    lblerrorMessage.Text = "Please select Student or Administrator before signing in.";
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
