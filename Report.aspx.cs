using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class Report : Page
    {
        private const string RatedResponsesCte = @"
WITH QuestionMap AS
(
    SELECT
        q.CategoryID,
        q.QuestionID,
        q.Question,
        ROW_NUMBER() OVER (PARTITION BY q.CategoryID ORDER BY q.QuestionID) AS QuestionNumber
    FROM dbo.FEEDBACK_QUESTIONS q
),
RatedResponses AS
(
    SELECT
        f.FeedbackID,
        CAST(f.FeedbackDate AS DATE) AS FeedbackDate,
        f.CategoryID,
        c.CategoryType,
        rating.QuestionNumber,
        COALESCE(qm.Question, 'Question ' + CAST(rating.QuestionNumber AS VARCHAR(10))) AS Question,
        rating.Rating
    FROM dbo.FEEDBACK f
    INNER JOIN dbo.CATEGORY c
        ON c.CategoryID = f.CategoryID
    CROSS APPLY
    (
        VALUES
            (1, f.[FeedbackRating Line1]),
            (2, f.[FeedbackRating Line2]),
            (3, f.[FeedbackRating Line3])
    ) AS rating(QuestionNumber, Rating)
    LEFT JOIN QuestionMap qm
        ON qm.CategoryID = f.CategoryID
        AND qm.QuestionNumber = rating.QuestionNumber
    WHERE rating.Rating BETWEEN 1 AND 5
        AND (@CategoryID IS NULL OR f.CategoryID = @CategoryID)
        AND (@FromDate IS NULL OR CAST(f.FeedbackDate AS DATE) >= @FromDate)
        AND (@ToDate IS NULL OR CAST(f.FeedbackDate AS DATE) <= @ToDate)
)
";

        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public string PerformanceChartJson { get; private set; }
        public string DistributionChartJson { get; private set; }
        public string DonutChartJson { get; private set; }
        public string TrendChartJson { get; private set; }
        public string DonutChartTitle { get; private set; }
        public string DonutChartSubtitle { get; private set; }
        public string PerformanceChartTitle { get; private set; }
        public string PerformanceChartSubtitle { get; private set; }
        public string DistributionChartTitle { get; private set; }
        public string DistributionChartSubtitle { get; private set; }
        public string TrendChartTitle { get; private set; }
        public string BreakdownTitle { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetEmptyReportState();

            if (!AuthNavigation.RequireAdmin(this))
            {
                return;
            }

            AuthNavigation.Configure(this, Menu1);

            if (!IsPostBack)
            {
                LoadCategoryFilter();
                ApplyDefaultReportFilters();
                LoadReport();
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadCategoryFilter()
        {
            ddlReportCategory.Items.Clear();
            ddlReportCategory.Items.Add(new ListItem("All Categories", string.Empty));

            const string query = @"
SELECT CategoryID, CategoryType
FROM dbo.CATEGORY
ORDER BY CategoryType;";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlReportCategory.Items.Add(new ListItem(
                            reader["CategoryType"].ToString(),
                            reader["CategoryID"].ToString()));
                    }
                }
            }
        }

        private void ApplyDefaultReportFilters()
        {
            ListItem administrationItem = ddlReportCategory.Items.FindByText("Administration");
            if (administrationItem != null)
            {
                ddlReportCategory.ClearSelection();
                administrationItem.Selected = true;
            }

            DateTime today = DateTime.Today;
            txtFromDate.Text = new DateTime(today.Year, 3, 1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            txtToDate.Text = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private void LoadReport()
        {
            int? categoryId = GetSelectedCategoryId();
            DateTime? fromDate = ParseDate(txtFromDate.Text);
            DateTime? toDate = ParseDate(txtToDate.Text);
            NormalizeDateRange(ref fromDate, ref toDate);

            string categoryLabel = categoryId.HasValue ? ddlReportCategory.SelectedItem.Text : "All Categories";
            lblReportScope.Text = BuildScopeText(categoryLabel, fromDate, toDate);

            PerformanceChartTitle = categoryId.HasValue ? "Question Performance" : "Category Comparison";
            PerformanceChartSubtitle = categoryId.HasValue
                ? "Average rating by question for the selected category."
                : "Average rating by feedback category.";
            DonutChartTitle = categoryId.HasValue
                ? "Rating Distribution for " + categoryLabel
                : "Feedback Distribution by Category";
            DonutChartSubtitle = categoryId.HasValue
                ? "Individual valid question ratings for the selected category."
                : "Submission counts grouped by feedback category.";
            DistributionChartTitle = categoryId.HasValue
                ? "Overall Rating Distribution for " + categoryLabel
                : "Overall Rating Distribution";
            DistributionChartSubtitle = categoryId.HasValue
                ? "Counts from individual question ratings in this category."
                : "Counts from individual question ratings.";
            TrendChartTitle = "Average Rating Trend" + (categoryId.HasValue ? " (" + categoryLabel + ")" : string.Empty);
            BreakdownTitle = categoryId.HasValue
                ? "Question Rating Breakdown (" + categoryLabel + ")"
                : "Question Rating Breakdown";

            LoadKpis(categoryId, fromDate, toDate);
            DonutChartJson = BuildDonutChartJson(categoryId, fromDate, toDate);
            PerformanceChartJson = BuildPerformanceChartJson(categoryId, fromDate, toDate);
            DistributionChartJson = BuildDistributionChartJson(categoryId, fromDate, toDate);
            TrendChartJson = BuildTrendChartJson(categoryId, fromDate, toDate);
            BindBreakdownTable(categoryId, fromDate, toDate);
        }

        private void LoadKpis(int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            const string submissionsQuery = @"
SELECT COUNT(*)
FROM dbo.FEEDBACK f
WHERE (@CategoryID IS NULL OR f.CategoryID = @CategoryID)
    AND (@FromDate IS NULL OR CAST(f.FeedbackDate AS DATE) >= @FromDate)
    AND (@ToDate IS NULL OR CAST(f.FeedbackDate AS DATE) <= @ToDate);";

            string ratedResponsesQuery = RatedResponsesCte + @"
SELECT COUNT(*)
FROM RatedResponses;";

            string averageRatingQuery = RatedResponsesCte + @"
SELECT ISNULL(ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2), 0)
FROM RatedResponses;";

            int submissions = Convert.ToInt32(ExecuteScalar(submissionsQuery, categoryId, fromDate, toDate));
            int ratedResponses = Convert.ToInt32(ExecuteScalar(ratedResponsesQuery, categoryId, fromDate, toDate));
            decimal averageRating = ClampRating(Convert.ToDecimal(ExecuteScalar(averageRatingQuery, categoryId, fromDate, toDate)));

            litFeedbackSubmissions.Text = submissions.ToString("N0", CultureInfo.InvariantCulture);
            litRatedResponses.Text = ratedResponses.ToString("N0", CultureInfo.InvariantCulture);
            litAverageRating.Text = ratedResponses > 0
                ? averageRating.ToString("0.00", CultureInfo.InvariantCulture) + " / 5"
                : "N/A";

            if (categoryId.HasValue)
            {
                string questionsQuery = RatedResponsesCte + @"
SELECT COUNT(DISTINCT QuestionNumber)
FROM RatedResponses;";

                int questionCount = Convert.ToInt32(ExecuteScalar(questionsQuery, categoryId, fromDate, toDate));
                litScopeMetricLabel.Text = "Questions Included";
                litScopeMetricValue.Text = questionCount.ToString("N0", CultureInfo.InvariantCulture);
                litScopeMetricDescription.Text = "Questions represented in this category";
            }
            else
            {
                const string categoriesQuery = @"
SELECT COUNT(DISTINCT f.CategoryID)
FROM dbo.FEEDBACK f
WHERE (@CategoryID IS NULL OR f.CategoryID = @CategoryID)
    AND (@FromDate IS NULL OR CAST(f.FeedbackDate AS DATE) >= @FromDate)
    AND (@ToDate IS NULL OR CAST(f.FeedbackDate AS DATE) <= @ToDate);";

                int categoryCount = Convert.ToInt32(ExecuteScalar(categoriesQuery, categoryId, fromDate, toDate));
                litScopeMetricLabel.Text = "Categories Included";
                litScopeMetricValue.Text = categoryCount.ToString("N0", CultureInfo.InvariantCulture);
                litScopeMetricDescription.Text = "Categories with matching submissions";
            }
        }

        private string BuildPerformanceChartJson(int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            string query = categoryId.HasValue
                ? RatedResponsesCte + @"
SELECT
    Question AS Label,
    ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2) AS AverageRating
FROM RatedResponses
GROUP BY QuestionNumber, Question
ORDER BY QuestionNumber;"
                : RatedResponsesCte + @"
SELECT
    CategoryType AS Label,
    ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2) AS AverageRating
FROM RatedResponses
GROUP BY CategoryType
ORDER BY CategoryType;";

            DataTable data = ExecuteDataTable(query, categoryId, fromDate, toDate);
            List<string> labels = new List<string>();
            List<decimal> values = new List<decimal>();

            foreach (DataRow row in data.Rows)
            {
                labels.Add(row["Label"].ToString());
                values.Add(ToDecimal(row["AverageRating"]));
            }

            return serializer.Serialize(new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        label = categoryId.HasValue ? "Question average" : "Category average",
                        data = values,
                        backgroundColor = BuildPalette(labels.Count),
                        borderColor = "#1d4ed8",
                        borderWidth = 1,
                        borderRadius = 7,
                        barPercentage = 0.72,
                        categoryPercentage = 0.72
                    }
                }
            });
        }

        private string BuildDonutChartJson(int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            if (categoryId.HasValue)
            {
                string query = RatedResponsesCte + @"
SELECT Rating, COUNT(*) AS ResponseCount
FROM RatedResponses
GROUP BY Rating
ORDER BY Rating;";

                DataTable data = ExecuteDataTable(query, categoryId, fromDate, toDate);
                int[] counts = new int[5];
                foreach (DataRow row in data.Rows)
                {
                    int rating = Convert.ToInt32(row["Rating"], CultureInfo.InvariantCulture);
                    if (rating >= 1 && rating <= 5)
                    {
                        counts[rating - 1] = Convert.ToInt32(row["ResponseCount"], CultureInfo.InvariantCulture);
                    }
                }

                string[] rawLabels = GetRatingLabels();
                return SerializeDonutChart(
                    rawLabels,
                    counts,
                    "Rating responses",
                    new[] { "#dc2626", "#f97316", "#facc15", "#22c55e", "#166534" });
            }

            const string queryByCategory = @"
SELECT
    c.CategoryType AS Label,
    COUNT(*) AS SubmissionCount
FROM dbo.FEEDBACK f
INNER JOIN dbo.CATEGORY c
    ON c.CategoryID = f.CategoryID
WHERE (@CategoryID IS NULL OR f.CategoryID = @CategoryID)
    AND (@FromDate IS NULL OR CAST(f.FeedbackDate AS DATE) >= @FromDate)
    AND (@ToDate IS NULL OR CAST(f.FeedbackDate AS DATE) <= @ToDate)
GROUP BY c.CategoryType
ORDER BY c.CategoryType;";

            DataTable categoryData = ExecuteDataTable(queryByCategory, categoryId, fromDate, toDate);
            List<string> labels = new List<string>();
            List<int> submissionCounts = new List<int>();

            foreach (DataRow row in categoryData.Rows)
            {
                labels.Add(row["Label"].ToString());
                submissionCounts.Add(Convert.ToInt32(row["SubmissionCount"], CultureInfo.InvariantCulture));
            }

            return SerializeDonutChart(labels.ToArray(), submissionCounts.ToArray(), "Submissions", BuildPalette(labels.Count));
        }

        private string BuildDistributionChartJson(int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            string query = RatedResponsesCte + @"
SELECT Rating, COUNT(*) AS ResponseCount
FROM RatedResponses
GROUP BY Rating
ORDER BY Rating;";

            DataTable data = ExecuteDataTable(query, categoryId, fromDate, toDate);
            int[] counts = new int[5];

            foreach (DataRow row in data.Rows)
            {
                int rating = Convert.ToInt32(row["Rating"]);
                if (rating >= 1 && rating <= 5)
                {
                    counts[rating - 1] = Convert.ToInt32(row["ResponseCount"]);
                }
            }

            return serializer.Serialize(new
            {
                labels = GetRatingLabels(),
                datasets = new[]
                {
                    new
                    {
                        label = "Responses",
                        data = counts,
                        backgroundColor = new[] { "#dc2626", "#f97316", "#facc15", "#22c55e", "#16a34a" },
                        borderColor = "#ffffff",
                        borderWidth = 1,
                        borderRadius = 7,
                        barPercentage = 0.66,
                        categoryPercentage = 0.76
                    }
                }
            });
        }

        private string BuildTrendChartJson(int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            bool groupByWeek = ShouldGroupTrendByWeek(fromDate, toDate);
            string query = groupByWeek
                ? RatedResponsesCte + @"
SELECT
    DATEADD(DAY, -(DATEDIFF(DAY, 0, FeedbackDate) % 7), FeedbackDate) AS PeriodStart,
    DATEADD(DAY, 6, DATEADD(DAY, -(DATEDIFF(DAY, 0, FeedbackDate) % 7), FeedbackDate)) AS PeriodEnd,
    ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2) AS AverageRating
FROM RatedResponses
GROUP BY DATEADD(DAY, -(DATEDIFF(DAY, 0, FeedbackDate) % 7), FeedbackDate)
ORDER BY PeriodStart;"
                : RatedResponsesCte + @"
SELECT
    FeedbackDate AS PeriodStart,
    FeedbackDate AS PeriodEnd,
    ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2) AS AverageRating
FROM RatedResponses
GROUP BY FeedbackDate
ORDER BY PeriodStart;";

            DataTable data = ExecuteDataTable(query, categoryId, fromDate, toDate);
            List<string> labels = new List<string>();
            List<decimal> values = new List<decimal>();

            foreach (DataRow row in data.Rows)
            {
                DateTime periodStart = Convert.ToDateTime(row["PeriodStart"]);
                DateTime periodEnd = Convert.ToDateTime(row["PeriodEnd"]);
                labels.Add(groupByWeek
                    ? FormatWeekLabel(periodStart, periodEnd)
                    : periodStart.ToString("MMM d", CultureInfo.InvariantCulture));
                values.Add(ToDecimal(row["AverageRating"]));
            }

            return serializer.Serialize(new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        label = "Average rating",
                        data = values,
                        borderColor = "#2563eb",
                        backgroundColor = "rgba(37, 99, 235, 0.14)",
                        pointBackgroundColor = "#2563eb",
                        pointBorderColor = "#ffffff",
                        pointBorderWidth = 2,
                        borderWidth = 3,
                        fill = true
                    }
                }
            });
        }

        private bool ShouldGroupTrendByWeek(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return true;
            }

            return (toDate.Value.Date - fromDate.Value.Date).TotalDays > 31;
        }

        private string FormatWeekLabel(DateTime periodStart, DateTime periodEnd)
        {
            string endLabel = periodStart.Month == periodEnd.Month
                ? periodEnd.Day.ToString(CultureInfo.InvariantCulture)
                : periodEnd.ToString("MMM d", CultureInfo.InvariantCulture);

            return periodStart.ToString("MMM d", CultureInfo.InvariantCulture) + "-" + endLabel;
        }

        private void BindBreakdownTable(int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            string query = categoryId.HasValue
                ? RatedResponsesCte + @"
, GroupedRatings AS
(
    SELECT
        QuestionNumber,
        Question,
        COUNT(*) AS TotalResponses,
        SUM(CASE WHEN Rating = 1 THEN 1 ELSE 0 END) AS StronglyDisagree,
        SUM(CASE WHEN Rating = 2 THEN 1 ELSE 0 END) AS Disagree,
        SUM(CASE WHEN Rating = 3 THEN 1 ELSE 0 END) AS Neutral,
        SUM(CASE WHEN Rating = 4 THEN 1 ELSE 0 END) AS Agree,
        SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) AS StronglyAgree,
        ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2) AS AverageRating
    FROM RatedResponses
    GROUP BY QuestionNumber, Question
)
SELECT
    ROW_NUMBER() OVER (ORDER BY QuestionNumber) AS [#],
    Question,
    CAST(StronglyDisagree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * StronglyDisagree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Strongly Disagree (1)],
    CAST(Disagree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * Disagree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Disagree (2)],
    CAST(Neutral AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * Neutral / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Neutral (3)],
    CAST(Agree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * Agree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Agree (4)],
    CAST(StronglyAgree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * StronglyAgree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Strongly Agree (5)],
    CONVERT(VARCHAR(10), CAST(AverageRating AS DECIMAL(10, 2))) AS [Average Rating]
FROM GroupedRatings
ORDER BY QuestionNumber;"
                : RatedResponsesCte + @"
, GroupedRatings AS
(
    SELECT
        CategoryType,
        QuestionNumber,
        Question,
        COUNT(*) AS TotalResponses,
        SUM(CASE WHEN Rating = 1 THEN 1 ELSE 0 END) AS StronglyDisagree,
        SUM(CASE WHEN Rating = 2 THEN 1 ELSE 0 END) AS Disagree,
        SUM(CASE WHEN Rating = 3 THEN 1 ELSE 0 END) AS Neutral,
        SUM(CASE WHEN Rating = 4 THEN 1 ELSE 0 END) AS Agree,
        SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) AS StronglyAgree,
        ROUND(AVG(CAST(Rating AS DECIMAL(10, 2))), 2) AS AverageRating
    FROM RatedResponses
    GROUP BY CategoryType, QuestionNumber, Question
)
SELECT
    ROW_NUMBER() OVER (ORDER BY CategoryType, QuestionNumber) AS [#],
    CategoryType AS Category,
    Question,
    CAST(StronglyDisagree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * StronglyDisagree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Strongly Disagree (1)],
    CAST(Disagree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * Disagree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Disagree (2)],
    CAST(Neutral AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * Neutral / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Neutral (3)],
    CAST(Agree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * Agree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Agree (4)],
    CAST(StronglyAgree AS VARCHAR(12)) + ' (' + CAST(CAST(100.0 * StronglyAgree / NULLIF(TotalResponses, 0) AS DECIMAL(5, 1)) AS VARCHAR(12)) + '%)' AS [Strongly Agree (5)],
    CONVERT(VARCHAR(10), CAST(AverageRating AS DECIMAL(10, 2))) AS [Average Rating]
FROM GroupedRatings
ORDER BY CategoryType, QuestionNumber;";

            FeedbackGridView.DataSource = ExecuteDataTable(query, categoryId, fromDate, toDate);
            FeedbackGridView.DataBind();
        }

        private object ExecuteScalar(string query, int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                AddReportParameters(command, categoryId, fromDate, toDate);
                connection.Open();
                return command.ExecuteScalar();
            }
        }

        private DataTable ExecuteDataTable(string query, int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                AddReportParameters(command, categoryId, fromDate, toDate);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        private void AddReportParameters(SqlCommand command, int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            command.Parameters.Add("@CategoryID", SqlDbType.Int).Value =
                categoryId.HasValue ? (object)categoryId.Value : DBNull.Value;
            command.Parameters.Add("@FromDate", SqlDbType.Date).Value =
                fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value;
            command.Parameters.Add("@ToDate", SqlDbType.Date).Value =
                toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value;
        }

        private int? GetSelectedCategoryId()
        {
            int categoryId;
            if (int.TryParse(ddlReportCategory.SelectedValue, out categoryId))
            {
                return categoryId;
            }

            return null;
        }

        private DateTime? ParseDate(string value)
        {
            DateTime date;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date.Date;
            }

            return null;
        }

        private void NormalizeDateRange(ref DateTime? fromDate, ref DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue || fromDate.Value <= toDate.Value)
            {
                return;
            }

            DateTime? previousFromDate = fromDate;
            fromDate = toDate;
            toDate = previousFromDate;

            txtFromDate.Text = fromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            txtToDate.Text = toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private string BuildScopeText(string categoryLabel, DateTime? fromDate, DateTime? toDate)
        {
            List<string> parts = new List<string> { "Showing report for: " + categoryLabel };

            if (fromDate.HasValue)
            {
                parts.Add("from " + fromDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture));
            }

            if (toDate.HasValue)
            {
                parts.Add("to " + toDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture));
            }

            return string.Join(" ", parts);
        }

        private void SetEmptyReportState()
        {
            PerformanceChartTitle = "Category Performance";
            PerformanceChartSubtitle = "Average rating by feedback category.";
            DonutChartTitle = "Feedback Distribution by Category";
            DonutChartSubtitle = "Submission counts grouped by feedback category.";
            DistributionChartTitle = "Overall Rating Distribution";
            DistributionChartSubtitle = "Counts from individual question ratings.";
            TrendChartTitle = "Average Rating Trend";
            BreakdownTitle = "Question Rating Breakdown";

            DonutChartJson = SerializeDonutChart(new string[0], new int[0], "Submissions", new string[0]);
            PerformanceChartJson = SerializeEmptyBarChart("Average rating");
            DistributionChartJson = serializer.Serialize(new
            {
                labels = GetRatingLabels(),
                datasets = new[]
                {
                    new
                    {
                        label = "Responses",
                        data = new[] { 0, 0, 0, 0, 0 },
                        backgroundColor = new[] { "#dc2626", "#f97316", "#facc15", "#22c55e", "#16a34a" }
                    }
                }
            });
            TrendChartJson = serializer.Serialize(new
            {
                labels = new string[0],
                datasets = new[]
                {
                    new
                    {
                        label = "Average rating",
                        data = new decimal[0],
                        borderColor = "#2563eb",
                        backgroundColor = "rgba(37, 99, 235, 0.14)"
                    }
                }
            });
        }

        private string SerializeEmptyBarChart(string label)
        {
            return serializer.Serialize(new
            {
                labels = new string[0],
                datasets = new[]
                {
                    new
                    {
                        label = label,
                        data = new decimal[0],
                        backgroundColor = new string[0]
                    }
                }
            });
        }

        private string SerializeDonutChart(string[] rawLabels, int[] counts, string datasetLabel, string[] colors)
        {
            int total = counts.Sum();
            List<string> displayLabels = new List<string>();

            for (int i = 0; i < rawLabels.Length; i++)
            {
                int count = i < counts.Length ? counts[i] : 0;
                decimal percentage = total > 0 ? Math.Round((decimal)count * 100m / total, 1) : 0m;
                displayLabels.Add(rawLabels[i] + " - " + count.ToString("N0", CultureInfo.InvariantCulture) + " (" + percentage.ToString("0.0", CultureInfo.InvariantCulture) + "%)");
            }

            return serializer.Serialize(new
            {
                labels = total > 0 ? displayLabels.ToArray() : new string[0],
                datasets = new[]
                {
                    new
                    {
                        label = datasetLabel,
                        data = total > 0 ? counts : new int[0],
                        backgroundColor = total > 0 ? colors : new string[0],
                        borderColor = "#ffffff",
                        borderWidth = 2,
                        hoverOffset = 8,
                        rawLabels = total > 0 ? rawLabels : new string[0]
                    }
                }
            });
        }

        private string[] GetRatingLabels()
        {
            return new[]
            {
                "Strongly Disagree",
                "Disagree",
                "Neutral",
                "Agree",
                "Strongly Agree"
            };
        }

        private decimal ToDecimal(object value)
        {
            if (value == DBNull.Value || value == null)
            {
                return 0;
            }

            return ClampRating(Convert.ToDecimal(value, CultureInfo.InvariantCulture));
        }

        private decimal ClampRating(decimal rating)
        {
            return Math.Max(0m, Math.Min(5m, rating));
        }

        private string[] BuildPalette(int count)
        {
            string[] palette =
            {
                "#2563eb",
                "#0f766e",
                "#7c3aed",
                "#d4af37",
                "#16a34a",
                "#ea580c",
                "#0891b2",
                "#4f46e5"
            };

            return Enumerable.Range(0, count)
                .Select(index => palette[index % palette.Length])
                .ToArray();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }
    }
}
