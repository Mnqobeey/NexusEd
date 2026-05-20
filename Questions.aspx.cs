using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class Questions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                SelectCommand = "SELECT CategoryID, CategoryType FROM CATEGORY"
            };

            ddlCategories.DataSource = categoryDataSource;
            ddlCategories.DataTextField = "CategoryType";
            ddlCategories.DataValueField = "CategoryID";
            ddlCategories.DataBind();

            ddlCategories.Items.Insert(0, new ListItem("Select Category", "0"));
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuestion.Text) && ddlCategories.SelectedIndex > 0)
            {
                int categoryID = Convert.ToInt32(ddlCategories.SelectedValue);

                SqlDataSource1.InsertParameters["Question"].DefaultValue = txtQuestion.Text;
                SqlDataSource1.InsertParameters["CategoryID"].DefaultValue = categoryID.ToString();
                SqlDataSource1.Insert();

                txtQuestion.Text = "";
                ddlCategories.SelectedIndex = 0;
                addQuestionsForm.Style.Add("display", "none");

                GridView1.DataBind();
            }
            else
            {
                lblError.Text = "Please enter a question and select a category.";
                lblError.Visible = true;
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
            GridViewRow selectedRow = GridView1.SelectedRow;

            if (selectedRow != null)
            {
                txtQuestionID.Text = Server.HtmlEncode(selectedRow.Cells[1].Text);
                txtQuestionEdit.Text = Server.HtmlEncode(selectedRow.Cells[2].Text);
                editForm.Visible = true;
            }
        }
    }
}