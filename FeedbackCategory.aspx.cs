using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class FeedbackCategory : System.Web.UI.Page
    {
        // FEEDBACK currently stores three fixed rating columns, so only three rating questions can be saved.
        private const int SupportedRatingQuestionCount = 3;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthNavigation.RequireStudent(this))
            {
                return;
            }

            AuthNavigation.Configure(this, Menu1);

            if (!IsPostBack)
            {
                LoadCategories();
                DisplayStudentID();

                pnlFeedbackForm.Visible = false;
                txtComment.Visible = false;
                btnSubmit.Visible = false;
                lblsubtitle2.Visible = false;
                imgRatings.Visible = false;

            }
        }

        private void DisplayStudentID()
        {
            if (Session["StudentID"] != null)
            {
                int studentID = Convert.ToInt32(Session["StudentID"]);
                txtStudentID.Text = studentID.ToString();
            }
            else
            {
                txtStudentID.Text = "StudentID not available";
            }
        }

        private void LoadCategories()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT CategoryID, CategoryType FROM CATEGORY";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlCategories.DataSource = reader;
                ddlCategories.DataTextField = "CategoryType";
                ddlCategories.DataValueField = "CategoryID";
                ddlCategories.DataBind();
            }

            ddlCategories.Items.Insert(0, new ListItem("Select Category", "0"));
        }

        private void DisplayRegisteredModules()
        {
            int studentID = GetCurrentStudentID();
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT m.ModuleID, m.ModuleCode AS [Module Code], m.ModuleName AS [Module Name], m.ModuleYear AS [Module Year] FROM MODULES m INNER JOIN STUDENT_MODULES sm ON m.ModuleID = sm.ModuleID WHERE sm.StudentID = @StudentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                BindSelectableData(dt, "ModuleID");
            }
        }

        private void BindSelectableData(DataTable dt, string keyColumn)
        {
            gvTableData.DataKeyNames = new[] { keyColumn };
            gvTableData.DataSource = dt;
            gvTableData.DataBind();
            HideGridColumn(gvTableData, keyColumn);
        }

        private void HideGridColumn(GridView grid, string columnName)
        {
            if (grid.HeaderRow == null)
            {
                return;
            }

            int columnIndex = -1;
            for (int i = 0; i < grid.HeaderRow.Cells.Count; i++)
            {
                if (grid.HeaderRow.Cells[i].Text == columnName)
                {
                    columnIndex = i;
                    break;
                }
            }

            if (columnIndex < 0)
            {
                return;
            }

            grid.HeaderRow.Cells[columnIndex].Visible = false;
            foreach (GridViewRow row in grid.Rows)
            {
                row.Cells[columnIndex].Visible = false;
            }
        }




        protected void ddlCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
            string selectedCategory = ddlCategories.SelectedItem.Text;

            txtComment.Visible = false;
            pnlFeedbackForm.Visible = false;
            gvTableData.Visible = false;
            btnSubmit.Visible = false;
            lblsubtitle1.Visible = false;
            lblsubtitle2.Visible = true;
            imgRatings.Visible = false;

            if (categoryId > 0)
            {
                if (selectedCategory == "Modules")
                {
                    DisplayRegisteredModules();
                    gvTableData.Visible = true;
                }
                else if (selectedCategory == "Lecturers")
                {
                    DisplayRegisteredLecturers();
                    gvTableData.Visible = true;
                }
                else if (selectedCategory == "Administration")
                {
                    DisplayAdmins();
                    gvTableData.Visible = true;
                }
                else if (selectedCategory == "Facilities")
                {
                    DisplayFacilities();
                    gvTableData.Visible = true;
                }
                else if (selectedCategory == "Tutors")
                {
                    DisplayTutors();
                    gvTableData.Visible = true;
                }
                else
                {
                    LoadTableData(categoryId);
                    gvTableData.Visible = true;
                    pnlFeedbackForm.Visible = true;
                    btnSubmit.Visible = true;
                    txtComment.Visible = true;
                    lblsubtitle2.Visible = false;
                    imgRatings.Visible = true;
                }

                LoadFeedbackForm(categoryId);

            }
            else
            {
                gvTableData.Visible = false;
                pnlFeedbackForm.Visible = false;
                lblsubtitle1.Visible = true;
                lblsubtitle2.Visible = false;
                imgRatings.Visible = false;

            }
        }

        private void DisplayFacilities()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT FacilityID, FaciltyName AS [Facility Name] FROM FACILITY";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                BindSelectableData(dt, "FacilityID");
            }
        }

        private void DisplayAdmins()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT AdminID, AdminName AS [Admin Name], AdminSurname AS [Admin Surname] FROM ADMIN_COORDINATOR";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                BindSelectableData(dt, "AdminID");
            }
        }


        private void DisplayRegisteredLecturers()
        {
            int studentID = GetCurrentStudentID();
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT l.LecturerID, l.LecturerName AS [Lecturer Name], l.LecturerSurname AS [Lecturer Surname], m.ModuleName FROM LECTURER l INNER JOIN MODULE_LECTURER ml ON l.LecturerID = ml.LecturerID INNER JOIN MODULES m ON ml.ModuleID = m.ModuleID INNER JOIN STUDENT_MODULES sm ON m.ModuleID = sm.ModuleID WHERE sm.StudentID = @StudentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                BindSelectableData(dt, "LecturerID");
            }
        }

        private void DisplayTutors()
        {
            int studentID = GetCurrentStudentID();
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT t.TutorID, t.TutorName AS [Tutor Name], t.TutorSurname AS [Tutor Surname], m.ModuleName FROM TUTOR t INNER JOIN TUTOR_MODULES tm ON t.TutorID = tm.TutorID INNER JOIN MODULES m ON tm.ModuleID = m.ModuleID INNER JOIN STUDENT_MODULES sm ON m.ModuleID = sm.ModuleID WHERE sm.StudentID = @StudentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentID", studentID);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                BindSelectableData(dt, "TutorID");
            }
        }


        private void LoadFeedbackForm(int categoryId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TOP (3) QuestionID, Question FROM FEEDBACK_QUESTIONS WHERE CategoryID = @CategoryID ORDER BY QuestionID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                pnlFeedbackForm.Controls.Clear();

                HtmlTable table = new HtmlTable();
                table.Border = 1;
                table.CellPadding = 5;
                table.CellSpacing = 0;

                HtmlTableRow headerRow = new HtmlTableRow();
                HtmlTableCell questionHeader = new HtmlTableCell { InnerHtml = "<b>Question</b>", Width = "50%" };
                headerRow.Cells.Add(questionHeader);

                string[] headers = { "Strongly Disagree", "Disagree", "Neutral", "Agree", "Strongly Agree" };
                foreach (string header in headers)
                {
                    HtmlTableCell ratingHeader = new HtmlTableCell { InnerHtml = $"<b>{header}</b>" };
                    headerRow.Cells.Add(ratingHeader);
                }
                table.Rows.Add(headerRow);

                int questionCounter = 1;
                while (reader.Read())
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell questionCell = new HtmlTableCell { InnerText = reader["Question"].ToString() };
                    row.Cells.Add(questionCell);

                    for (int i = 1; i <= 5; i++)
                    {
                        HtmlTableCell responseCell = new HtmlTableCell();
                        RadioButton radioButton = new RadioButton
                        {
                            GroupName = "Question_" + questionCounter,
                            ID = $"rb_{questionCounter}_{i}",
                            Text = i.ToString(),
                            CssClass = "radio-button"
                        };
                        radioButton.InputAttributes["value"] = i.ToString();
                        responseCell.Controls.Add(radioButton);
                        row.Cells.Add(responseCell);
                    }

                    table.Rows.Add(row);
                    questionCounter++;
                }

                pnlFeedbackForm.Controls.Add(table);
            }
        }


        private void LoadTableData(int categoryId)
        {
            string selectedCategory = ddlCategories.SelectedItem.Text;
            string tableName = GetTableNameForCategory(selectedCategory);

            if (!string.IsNullOrEmpty(tableName))
            {
                BindTableDataToGrid(tableName);
            }
        }

        private string GetTableNameForCategory(string categoryType)
        {
            switch (categoryType)
            {
                case "Administration":
                    return "ADMIN_COORDINATOR";
                case "Lecturers":
                    return "LECTURER";
                case "Tutors":
                    return "TUTOR";
                case "Facilities":
                    return "FACILITY";
                default:
                    return string.Empty;
            }
        }

        private void BindTableDataToGrid(string tableName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = $"SELECT * FROM {tableName}";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Columns.Count > 0 && dt.Columns[0].ColumnName.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    BindSelectableData(dt, dt.Columns[0].ColumnName);
                }
                else
                {
                    gvTableData.DataKeyNames = new string[0];
                    gvTableData.DataSource = dt;
                    gvTableData.DataBind();
                }
            }
        }

        protected void gvTableData_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvTableData.SelectedRow;
            string selectedId = gvTableData.SelectedDataKey != null ? gvTableData.SelectedDataKey.Value.ToString() : row.Cells[1].Text;
            txtSelectedID.Text = selectedId;

            int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);

            if (categoryId > 0)
            {
                LoadFeedbackForm(categoryId);

                pnlFeedbackForm.Visible = true;
                txtComment.Visible = true;
                btnSubmit.Visible = true;
                lblsubtitle2.Visible = false;
                imgRatings.Visible = true;


            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
            int studentId = GetCurrentStudentID();

            if (categoryId > 0)
            {
                string selectedId = txtSelectedID.Text;
                string selectedCategory = ddlCategories.SelectedItem.Text;

                if (RequiresRelatedSelection(selectedCategory) && string.IsNullOrWhiteSpace(selectedId))
                {
                    RestoreFeedbackForm(categoryId);
                    ShowErrorMessage("Please select an item before submitting your feedback.");
                    return;
                }

                int[] ratings;
                string ratingValidationMessage;
                if (!TryGetSupportedRatings(categoryId, out ratings, out ratingValidationMessage))
                {
                    RestoreFeedbackForm(categoryId);
                    ShowErrorMessage(ratingValidationMessage);
                    return;
                }

                if (categoryId == GetCategoryId("Modules"))
                {
                    if (IsStudentRegisteredForCategory(categoryId, selectedId, studentId))
                    {
                        int feedbackId = SaveFeedback(categoryId, studentId, ratings);
                        SaveModuleFeedback(feedbackId, selectedId);
                        ShowSuccessMessage();
                    }
                    else
                    {
                        ShowErrorMessage("You are not registered for the selected module.");
                    }
                }
                else if (categoryId == GetCategoryId("Lecturers"))
                {
                    if (IsStudentRegisteredForCategory(categoryId, selectedId, studentId))
                    {
                        int feedbackId = SaveFeedback(categoryId, studentId, ratings);
                        SaveLecturerFeedback(feedbackId, selectedId);
                        ShowSuccessMessage();
                    }


                    else
                    {
                        ShowErrorMessage("You are not registered for the selected lecturer.");
                    }
                }

                else if (categoryId == GetCategoryId("Tutors"))
                {
                    if (IsStudentRegisteredForCategory(categoryId, selectedId, studentId))
                    {
                        int feedbackId = SaveFeedback(categoryId, studentId, ratings);
                        SaveTutorFeedback(feedbackId, selectedId);
                        ShowSuccessMessage();
                    }


                    else
                    {
                        ShowErrorMessage("You are not registered for the selected lecturer.");
                    }
                }

                else if (categoryId == GetCategoryId("Administration"))
                {
                    int feedbackId = SaveFeedback(categoryId, studentId, ratings);
                    SaveAdminFeedback(feedbackId, selectedId);

                    ShowSuccessMessage();
                }
                else if (categoryId == GetCategoryId("Facilities"))
                {
                    int feedbackId = SaveFeedback(categoryId, studentId, ratings);
                    SaveFacilityFeedback(feedbackId, selectedId);

                    ShowSuccessMessage();
                }
                else
                {
                    int feedbackId = SaveFeedback(categoryId, studentId, ratings);
                    ShowSuccessMessage();
                }
            }
        }

        private int GetCategoryId(string categoryName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT CategoryID FROM CATEGORY WHERE CategoryType = @CategoryType";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryType", categoryName);

                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }


        private void SaveLecturerFeedback(int feedbackId, string lecturerId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO LECTURER_FEEDBACK (FeedbackID, LecturerID) VALUES (@FeedbackID, @LecturerID)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                cmd.Parameters.AddWithValue("@LecturerID", lecturerId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveTutorFeedback(int feedbackId, string tutorId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO TUTOR_FEEDBACK (FeedbackID, TutorID) VALUES (@FeedbackID, @TutorID)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                cmd.Parameters.AddWithValue("@TutorID", tutorId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveAdminFeedback(int feedbackId, string adminId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO ADMIN_FEEDBACK (FeedbackID, AdminID) VALUES (@FeedbackID, @AdminID)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                cmd.Parameters.AddWithValue("@AdminID", adminId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveFacilityFeedback(int feedbackId, string facilityId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO FACILITY_FEEDBACK (FeedbackID, FacilityID) VALUES (@FeedbackID, @FacilityID)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                cmd.Parameters.AddWithValue("@FacilityID", facilityId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }



        private void SaveModuleFeedback(int feedbackId, string moduleId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO MODULE_FEEDBACK (FeedbackID, ModuleID) VALUES (@FeedbackID, @ModuleID)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                cmd.Parameters.AddWithValue("@ModuleID", moduleId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        private bool IsStudentRegisteredForCategory(int categoryId, string selectedId, int studentId)
        {
            string tableName = GetTableNameForCategory(ddlCategories.SelectedItem.Text);
            string columnName = GetColumnNameForCategory(ddlCategories.SelectedItem.Text);

            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnName))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (tableName == "STUDENT_MODULES" || tableName == "LECTURER_STUDENT")
                    {
                        string query = $"SELECT COUNT(*) FROM {tableName} WHERE StudentID = @StudentID AND {columnName} = @SelectedID";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        cmd.Parameters.AddWithValue("@SelectedID", selectedId);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        private string GetColumnNameForCategory(string categoryType)
        {
            switch (categoryType)
            {
                case "Modules":
                    return "ModuleID";
                case "Tutors":
                    return "TutorID";
                case "Lecturers":
                    return "LecturerID";
                case "Facilities":
                    return "FacilityID";
                default:
                    return string.Empty;
            }
        }

        private int SaveFeedback(int categoryId, int studentId, int[] ratings)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            int feedbackId;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO FEEDBACK ([FeedbackRating Line1], [FeedbackRating Line2], [FeedbackRating Line3], Comment, FeedbackDate, StudentID, CategoryID) VALUES (@Rating1, @Rating2, @Rating3, @Comment, @FeedbackDate, @StudentID, @CategoryID); SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Rating1", ratings[0]);
                cmd.Parameters.AddWithValue("@Rating2", ratings[1]);
                cmd.Parameters.AddWithValue("@Rating3", ratings[2]);
                cmd.Parameters.AddWithValue("@Comment", txtComment.Text);
                cmd.Parameters.AddWithValue("@FeedbackDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@StudentID", studentId);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);

                con.Open();
                feedbackId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return feedbackId;
        }

        private bool TryGetSupportedRatings(int categoryId, out int[] ratings, out string validationMessage)
        {
            ratings = new int[SupportedRatingQuestionCount];
            validationMessage = string.Empty;

            int questionCount = GetSupportedQuestionCount(categoryId);
            if (questionCount == 0)
            {
                validationMessage = "No feedback questions are available for this category.";
                return false;
            }

            if (questionCount < SupportedRatingQuestionCount)
            {
                validationMessage = "This category needs 3 feedback questions before feedback can be submitted.";
                return false;
            }

            for (int questionNumber = 1; questionNumber <= SupportedRatingQuestionCount; questionNumber++)
            {
                int rating;
                if (!TryGetRatingForQuestion(questionNumber, out rating))
                {
                    validationMessage = "Please answer all rating questions before submitting your feedback.";
                    return false;
                }

                ratings[questionNumber - 1] = rating;
            }

            return true;
        }

        private bool TryGetRatingForQuestion(int questionNumber, out int rating)
        {
            rating = 0;
            string radioButtonName = "Question_" + questionNumber.ToString(CultureInfo.InvariantCulture);
            string selectedValue = Request.Form[radioButtonName];

            if (TryParseRating(selectedValue, out rating))
            {
                return true;
            }

            return false;
        }

        private bool TryParseRating(string selectedValue, out int rating)
        {
            rating = 0;
            if (string.IsNullOrWhiteSpace(selectedValue))
            {
                return false;
            }

            if (int.TryParse(selectedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out rating))
            {
                return rating >= 1 && rating <= 5;
            }

            string[] parts = selectedValue.Split('_');
            if (parts.Length > 0 && int.TryParse(parts[parts.Length - 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out rating))
            {
                return rating >= 1 && rating <= 5;
            }

            return false;
        }

        private int GetSupportedQuestionCount(int categoryId)
        {
            const string query = @"
SELECT COUNT(*)
FROM
(
    SELECT TOP (3) QuestionID
    FROM dbo.FEEDBACK_QUESTIONS
    WHERE CategoryID = @CategoryID
    ORDER BY QuestionID
) AS SupportedQuestions;";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CategoryID", categoryId);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
            }
        }

        private bool RequiresRelatedSelection(string categoryType)
        {
            return categoryType == "Administration"
                || categoryType == "Facilities"
                || categoryType == "Lecturers"
                || categoryType == "Modules"
                || categoryType == "Tutors";
        }

        private void RestoreFeedbackForm(int categoryId)
        {
            LoadFeedbackForm(categoryId);
            pnlFeedbackForm.Visible = true;
            txtComment.Visible = true;
            btnSubmit.Visible = true;
            lblsubtitle2.Visible = false;
            imgRatings.Visible = true;
        }


        private int GetCurrentStudentID()
        {
            if (Session["StudentID"] != null)
            {
                return Convert.ToInt32(Session["StudentID"]);
            }
            throw new Exception("StudentID is not available in session.");
        }

        private void ShowSuccessMessage()
        {
            string message = "Feedback saved successfully. Thank you!";
            string script = $"window.onload = function(){{ alert('{message}'); window.location = '{Request.Url.AbsoluteUri}'; }}";
            ClientScript.RegisterStartupScript(this.GetType(), "SuccessMessage", script, true);
        }

        private void ShowErrorMessage(string message)
        {
            string script = $"window.onload = function(){{ alert('{message}'); }}";
            ClientScript.RegisterStartupScript(this.GetType(), "ErrorMessage", script, true);
        }
    }
}
