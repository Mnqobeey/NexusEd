using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class AdminDashboard : System.Web.UI.Page
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
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int totalFeedback = GetFeedbackCount(conn, "SELECT COUNT(*) FROM dbo.FEEDBACK");
                    litTotalFeedback.Text = totalFeedback.ToString("N0", CultureInfo.InvariantCulture);

                    int recentFeedback = GetFeedbackCount(conn,
                        "SELECT COUNT(*) FROM dbo.FEEDBACK WHERE FeedbackDate >= DATEADD(DAY, -7, GETDATE())");
                    litAdminFeedback.Text = recentFeedback.ToString("N0", CultureInfo.InvariantCulture);

                    int categoryCount = GetFeedbackCount(conn, "SELECT COUNT(*) FROM dbo.CATEGORY");
                    litFacilitiesFeedback.Text = categoryCount.ToString("N0", CultureInfo.InvariantCulture);

                    decimal averageRating = ClampRating(GetAverageRating(conn));
                    litLecturerFeedback.Text = averageRating.ToString("0.0", CultureInfo.InvariantCulture) + " / 5";

                    int ratedResponses = GetFeedbackCount(conn, @"
                        SELECT COUNT(*)
                        FROM dbo.FEEDBACK f
                        CROSS APPLY
                        (
                            VALUES
                                (f.[FeedbackRating Line1]),
                                (f.[FeedbackRating Line2]),
                                (f.[FeedbackRating Line3])
                        ) AS r(Rating)
                        WHERE r.Rating BETWEEN 1 AND 5");
                    litRatedResponses.Text = ratedResponses.ToString("N0", CultureInfo.InvariantCulture);

                    BindRecentFeedback(conn);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                // Log error and set safe defaults
                System.Diagnostics.Debug.WriteLine("Dashboard Error: " + ex.Message);
                litTotalFeedback.Text = "0";
                litAdminFeedback.Text = "0";
                litFacilitiesFeedback.Text = "0";
                litLecturerFeedback.Text = "0.0 / 5";
                litRatedResponses.Text = "0";
                gvRecentFeedback.DataSource = CreateEmptyRecentFeedbackTable();
                gvRecentFeedback.DataBind();
            }
        }

        protected void gvRecentFeedback_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRecentFeedback.PageIndex = e.NewPageIndex;
            LoadDashboardData();
        }

        private int GetFeedbackCount(SqlConnection conn, string query)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetAverageRating(SqlConnection conn)
        {
            try
            {
                const string query = @"
                    SELECT ISNULL(AVG(CAST(r.Rating AS DECIMAL(10, 2))), 0)
                    FROM dbo.FEEDBACK f
                    CROSS APPLY
                    (
                        VALUES
                            (f.[FeedbackRating Line1]),
                            (f.[FeedbackRating Line2]),
                            (f.[FeedbackRating Line3])
                    ) AS r(Rating)
                    WHERE r.Rating BETWEEN 1 AND 5";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
                }
            }
            catch
            {
                return 0m;
            }
        }

        private void BindRecentFeedback(SqlConnection conn)
        {
            const string query = @"
                SELECT TOP 15
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

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable source = new DataTable();
                adapter.Fill(source);

                DataTable recentFeedback = CreateEmptyRecentFeedbackTable();
                foreach (DataRow row in source.Rows)
                {
                    DataRow safeRow = recentFeedback.NewRow();
                    safeRow["AnonymousRef"] = row["AnonymousRef"].ToString();
                    safeRow["Category"] = row["Category"].ToString();
                    safeRow["CategoryClass"] = GetCategoryDotClass(row["Category"].ToString());
                    int validRatings = row["ValidRatingCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["ValidRatingCount"], CultureInfo.InvariantCulture);
                    if (validRatings == 0 || row["AverageRating"] == DBNull.Value)
                    {
                        safeRow["Rating"] = "N/A";
                    }
                    else
                    {
                        decimal rating = ClampRating(Convert.ToDecimal(row["AverageRating"], CultureInfo.InvariantCulture));
                        safeRow["Rating"] = rating.ToString("0.0", CultureInfo.InvariantCulture) + " / 5";
                    }
                    safeRow["SubmittedDate"] = Convert.ToDateTime(row["SubmittedDate"], CultureInfo.InvariantCulture);
                    recentFeedback.Rows.Add(safeRow);
                }

                gvRecentFeedback.DataSource = recentFeedback;
                gvRecentFeedback.DataBind();
            }
        }

        private DataTable CreateEmptyRecentFeedbackTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("AnonymousRef", typeof(string));
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("CategoryClass", typeof(string));
            table.Columns.Add("Rating", typeof(string));
            table.Columns.Add("SubmittedDate", typeof(DateTime));
            return table;
        }

        private string GetCategoryDotClass(string category)
        {
            switch ((category ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "lecturers":
                    return "dashboard-category-dot dot-blue";
                case "facilities":
                    return "dashboard-category-dot dot-green";
                case "administration":
                    return "dashboard-category-dot dot-purple";
                case "modules":
                    return "dashboard-category-dot dot-gold";
                case "tutors":
                    return "dashboard-category-dot dot-teal";
                default:
                    return "dashboard-category-dot dot-blue";
            }
        }

        private static decimal ClampRating(decimal rating)
        {
            return Math.Max(0m, Math.Min(5m, rating));
        }
    }
}
