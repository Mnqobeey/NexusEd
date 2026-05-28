using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class Questions : System.Web.UI.Page
    {
        // FEEDBACK currently stores three fixed rating columns, so each category is capped at three questions.
        private const int MaximumQuestionsPerCategory = 3;
        private const string MaximumQuestionsMessage = "This version supports a maximum of 3 feedback questions per category.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthNavigation.RequireAdmin(this))
            {
                return;
            }

            AuthNavigation.Configure(this, Menu1);

            if (!IsPostBack)
            {
                LoadCategories();
                editForm.Visible = false;
            }
        }

        private void LoadCategories()
        {
            SqlDataSource categoryDataSource = new SqlDataSource
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString,
                SelectCommand = "SELECT CategoryID, CategoryType FROM CATEGORY ORDER BY CategoryType"
            };

            ddlCategories.DataSource = categoryDataSource;
            ddlCategories.DataTextField = "CategoryType";
            ddlCategories.DataValueField = "CategoryID";
            ddlCategories.DataBind();

            ddlCategories.Items.Insert(0, new ListItem("Select Category", "0"));
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            editForm.Visible = false;
            GridView1.SelectedIndex = -1;

            if (!string.IsNullOrWhiteSpace(txtQuestion.Text) && ddlCategories.SelectedIndex > 0)
            {
                int categoryID = Convert.ToInt32(ddlCategories.SelectedValue);

                if (GetQuestionCount(categoryID) >= MaximumQuestionsPerCategory)
                {
                    lblError.Text = MaximumQuestionsMessage;
                    lblError.Visible = true;
                    addQuestionsForm.Style.Add("display", "block");
                    return;
                }

                SqlDataSource1.InsertParameters["Question"].DefaultValue = txtQuestion.Text;
                SqlDataSource1.InsertParameters["CategoryID"].DefaultValue = categoryID.ToString();
                SqlDataSource1.Insert();

                txtQuestion.Text = "";
                ddlCategories.SelectedIndex = 0;
                addQuestionsForm.Style.Add("display", "none");
                txtQuestionID.Text = string.Empty;
                txtQuestionEdit.Text = string.Empty;

                GridView1.DataBind();
            }
            else
            {
                lblError.Text = "Please enter a question and select a category.";
                lblError.Visible = true;
                addQuestionsForm.Style.Add("display", "block");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuestionEdit.Text) && !string.IsNullOrEmpty(txtQuestionID.Text))
            {
                int questionID = Convert.ToInt32(txtQuestionID.Text);

                SqlDataSource1.UpdateCommand = "UPDATE FEEDBACK_QUESTIONS SET Question = @Question WHERE QuestionID = @QuestionID";
                SqlDataSource1.UpdateParameters["Question"].DefaultValue = txtQuestionEdit.Text;
                SqlDataSource1.UpdateParameters["QuestionID"].DefaultValue = questionID.ToString();
                SqlDataSource1.Update();

                editForm.Visible = false;
                GridView1.DataBind();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuestionID.Text))
            {
                int questionID = Convert.ToInt32(txtQuestionID.Text);

                SqlDataSource1.DeleteCommand = "DELETE FROM FEEDBACK_QUESTIONS WHERE QuestionID = @QuestionID";
                SqlDataSource1.DeleteParameters["QuestionID"].DefaultValue = questionID.ToString();
                SqlDataSource1.Delete();

                editForm.Visible = false;
                GridView1.DataBind();

            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GridView1.SelectedDataKey != null && int.TryParse(GridView1.SelectedDataKey.Value.ToString(), out int questionID))
            {
                txtQuestionID.Text = questionID.ToString();
                txtQuestionEdit.Text = GetQuestionByID(questionID);
                addQuestionsForm.Style.Add("display", "none");
                editForm.Visible = true;
            }
        }

        private int GetQuestionCount(int categoryId)
        {
            const string query = "SELECT COUNT(*) FROM dbo.FEEDBACK_QUESTIONS WHERE CategoryID = @CategoryID";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CategoryID", categoryId);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private string GetQuestionByID(int questionID)
        {
            const string query = "SELECT Question FROM dbo.FEEDBACK_QUESTIONS WHERE QuestionID = @QuestionID";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionID);
                connection.Open();

                object result = command.ExecuteScalar();
                return result == null ? string.Empty : result.ToString();
            }
        }
    }
}
