using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class View : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthNavigation.RequireAdmin(this))
            {
                return;
            }

            AuthNavigation.Configure(this, Menu1);

            if (!IsPostBack)
            {
                BindFeedback();
            }
        }

        protected void gvFeedback_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFeedback.PageIndex = e.NewPageIndex;
            BindFeedback();
        }

        private void BindFeedback()
        {
            const string query = @"
SELECT
    'FB-' + RIGHT('000' + CAST(f.FeedbackID AS VARCHAR(10)), 3) AS AnonymousRef,
    c.CategoryType AS Category,
    AVG(CASE WHEN r.Rating BETWEEN 1 AND 5 THEN CAST(r.Rating AS DECIMAL(10, 2)) END) AS AverageRating,
    SUM(CASE WHEN r.Rating BETWEEN 1 AND 5 THEN 1 ELSE 0 END) AS ValidRatingCount,
    f.FeedbackDate AS SubmittedDate
FROM dbo.FEEDBACK f
INNER JOIN dbo.CATEGORY c
    ON c.CategoryID = f.CategoryID
CROSS APPLY
(
    VALUES
        (f.[FeedbackRating Line1]),
        (f.[FeedbackRating Line2]),
        (f.[FeedbackRating Line3])
) AS r(Rating)
GROUP BY f.FeedbackID, c.CategoryType, f.FeedbackDate
ORDER BY f.FeedbackDate DESC, f.FeedbackID DESC;";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable source = new DataTable();
                adapter.Fill(source);

                DataTable feedback = CreateFeedbackTable();
                foreach (DataRow row in source.Rows)
                {
                    DataRow safeRow = feedback.NewRow();
                    safeRow["AnonymousRef"] = row["AnonymousRef"].ToString();
                    safeRow["Category"] = row["Category"].ToString();
                    safeRow["AverageRating"] = FormatAverageRating(row["AverageRating"], row["ValidRatingCount"]);
                    safeRow["SubmittedDate"] = Convert.ToDateTime(row["SubmittedDate"], CultureInfo.InvariantCulture);
                    feedback.Rows.Add(safeRow);
                }

                gvFeedback.DataSource = feedback;
                gvFeedback.DataBind();
            }
        }

        private static DataTable CreateFeedbackTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("AnonymousRef", typeof(string));
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("AverageRating", typeof(string));
            table.Columns.Add("SubmittedDate", typeof(DateTime));
            return table;
        }

        private static string FormatAverageRating(object averageRating, object validRatingCount)
        {
            int count = validRatingCount == DBNull.Value || validRatingCount == null
                ? 0
                : Convert.ToInt32(validRatingCount, CultureInfo.InvariantCulture);

            if (count == 0 || averageRating == DBNull.Value || averageRating == null)
            {
                return "N/A";
            }

            decimal rating = Convert.ToDecimal(averageRating, CultureInfo.InvariantCulture);
            rating = Math.Max(1m, Math.Min(5m, rating));
            return rating.ToString("0.0", CultureInfo.InvariantCulture) + " / 5";
        }
    }
}
