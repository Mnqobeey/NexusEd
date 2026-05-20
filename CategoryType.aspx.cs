using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class CategoryType : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                editForm.Visible = false;
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            GridViewRow selectedRow = GridView1.SelectedRow;

            if (selectedRow != null)
            {
                txtCategoryID.Text = selectedRow.Cells[1].Text;
                txtCategoryTypeEdit.Text = selectedRow.Cells[2].Text;

                editForm.Visible = true;
            }
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCategoryType.Text))
            {
                SqlDataSource1.InsertParameters["CategoryType"].DefaultValue = txtCategoryType.Text;
                SqlDataSource1.Insert();

                txtCategoryType.Text = "";
                addCategoryForm.Style.Add("display", "none");
                GridView1.DataBind();

                ClientScript.RegisterStartupScript(this.GetType(), "confirmProceed", "if (confirmProceed()) { window.location='Questions.aspx'; }", true);


            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            int categoryID = Convert.ToInt32(txtCategoryID.Text);
            string newCategoryType = txtCategoryTypeEdit.Text;

            SqlDataSource1.UpdateCommand = "UPDATE CATEGORY SET CategoryType = @CategoryType WHERE CategoryID = @CategoryID";

            SqlDataSource1.UpdateParameters["CategoryType"].DefaultValue = newCategoryType;
            SqlDataSource1.UpdateParameters["CategoryID"].DefaultValue = categoryID.ToString();
            SqlDataSource1.Update();

            editForm.Visible = false;
            GridView1.DataBind();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int categoryID = Convert.ToInt32(txtCategoryID.Text);
            string protectedCategories = "Administration,Lecturers,Modules,Tutors,Facilities";

            string categoryType = GetCategoryTypeByID(categoryID);

            if (protectedCategories.Contains(categoryType))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "preventDelete", "alert('This category cannot be deleted.');", true);
            }
            else
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string deleteFeedbackQuery = "DELETE FROM FEEDBACK WHERE CategoryID = @CategoryID";
                    using (SqlCommand deleteFeedbackCmd = new SqlCommand(deleteFeedbackQuery, con))
                    {
                        deleteFeedbackCmd.Parameters.AddWithValue("@CategoryID", categoryID);
                        deleteFeedbackCmd.ExecuteNonQuery();
                    }

                    string deleteQuestionsQuery = "DELETE FROM FEEDBACK_QUESTIONS WHERE CategoryID = @CategoryID";
                    using (SqlCommand deleteQuestionsCmd = new SqlCommand(deleteQuestionsQuery, con))
                    {
                        deleteQuestionsCmd.Parameters.AddWithValue("@CategoryID", categoryID);
                        deleteQuestionsCmd.ExecuteNonQuery();
                    }

                    string deleteCategoryQuery = "DELETE FROM CATEGORY WHERE CategoryID = @CategoryID";
                    using (SqlCommand deleteCategoryCmd = new SqlCommand(deleteCategoryQuery, con))
                    {
                        deleteCategoryCmd.Parameters.AddWithValue("@CategoryID", categoryID);
                        deleteCategoryCmd.ExecuteNonQuery();
                    }

                    editForm.Visible = false;
                    GridView1.DataBind();
                }
            }
        }

        private string GetCategoryTypeByID(int categoryID)
        {
            string categoryType = string.Empty;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT CategoryType FROM CATEGORY WHERE CategoryID = @CategoryID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        categoryType = result.ToString();
                    }
                }
            }
            return categoryType;
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            Response.Redirect("Questions.aspx");
        }
    }
}