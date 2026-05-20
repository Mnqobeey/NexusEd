using System;
using System.Data;
using System.Web.Services;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public partial class Report : System.Web.UI.Page
    {
        public string LineChartData { get; set; }
        public string BarChartData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SelectedUserType"] == null || Session["SelectedUserType"].ToString() != "Admin_Coordinator")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                BarChartData = GetBarChartData();
                LineChartData = GetLineChartDataForCategory("All");

                ScriptManager.RegisterStartupScript(this, GetType(), "InitializeCharts", "initializeCharts();", true);
            }
        }


        private void LoadChartsData()
        {
            BarChartData = GetBarChartData();
        }


        [WebMethod]
        public static string GetGroupedBarChartData(string selectedCategory)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            string query = @"
                SELECT
                    CASE
                        WHEN @CategoryType = 'Modules' THEN m.ModuleName
                        WHEN @CategoryType = 'Facilities' THEN fac.FaciltyName
                        WHEN @CategoryType = 'Lecturers' THEN l.LecturerName
                        WHEN @CategoryType = 'Tutors' THEN t.TutorName
                        WHEN @CategoryType = 'Administration' THEN a.AdminName
                        ELSE c.CategoryType
                    END AS EntityName,
                    AVG(CAST(f.[FeedbackRating Line1] AS FLOAT)) AS AvgRating1,
                    AVG(CAST(f.[FeedbackRating Line2] AS FLOAT)) AS AvgRating2,
                    AVG(CAST(f.[FeedbackRating Line3] AS FLOAT)) AS AvgRating3
                FROM Feedback f
                JOIN Category c ON f.CategoryID = c.CategoryID
                LEFT JOIN module_feedback mf ON mf.FeedbackID = f.FeedbackID
                LEFT JOIN Modules m ON mf.ModuleID = m.ModuleID
                LEFT JOIN facility_feedback ff ON ff.FeedbackID = f.FeedbackID
                LEFT JOIN Facility fac ON ff.FacilityID = fac.FacilityID
                LEFT JOIN lecturer_feedback lf ON lf.FeedbackID = f.FeedbackID
                LEFT JOIN Lecturer l ON lf.LecturerID = l.LecturerID
                LEFT JOIN tutor_feedback tf ON tf.FeedbackID = f.FeedbackID
                LEFT JOIN Tutor t ON tf.TutorID = t.TutorID
                LEFT JOIN admin_feedback af ON af.FeedbackID = f.FeedbackID
                LEFT JOIN ADMIN_COORDINATOR a ON af.AdminID = a.AdminID
                WHERE (@CategoryType IS NULL OR c.CategoryType = @CategoryType)
                GROUP BY
                    CASE
                        WHEN @CategoryType = 'Modules' THEN m.ModuleName
                        WHEN @CategoryType = 'Facilities' THEN fac.FaciltyName
                        WHEN @CategoryType = 'Lecturers' THEN l.LecturerName
                        WHEN @CategoryType = 'Tutors' THEN t.TutorName
                        WHEN @CategoryType = 'Administration' THEN a.AdminName
                        ELSE c.CategoryType
                    END
                ORDER BY EntityName;";

            Dictionary<string, float[]> averageRatingsData = new Dictionary<string, float[]>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryType", string.IsNullOrEmpty(selectedCategory) ? (object)DBNull.Value : selectedCategory);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string entityName = reader["EntityName"].ToString();

                            if (!averageRatingsData.ContainsKey(entityName))
                            {
                                averageRatingsData[entityName] = new float[3];
                            }

                            averageRatingsData[entityName][0] = Convert.ToSingle(reader["AvgRating1"]);
                            averageRatingsData[entityName][1] = Convert.ToSingle(reader["AvgRating2"]);
                            averageRatingsData[entityName][2] = Convert.ToSingle(reader["AvgRating3"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(new { error = ex.Message });
                }
            }

            string[] colors = {
                "rgba(255, 99, 132, 1)",
                "rgba(54, 162, 235, 1)",
                "rgba(213, 232, 47, 1)"
            };

            List<object> datasets = new List<object>();

            for (int i = 0; i < 3; i++)
            {
                datasets.Add(new
                {
                    label = $"Average Rating - Question {i + 1}",
                    backgroundColor = colors[i],
                    data = averageRatingsData.Values.Select(ratings => ratings[i]).ToArray()
                });
            }

            var groupedBarData = new
            {
                labels = averageRatingsData.Keys.ToArray(),
                datasets = datasets
            };
            return JsonConvert.SerializeObject(groupedBarData);
        }


        [WebMethod]
        public static string GetPieChartData()
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            string query = @"
            SELECT
                c.CategoryType,
                COUNT(f.FeedbackID) AS FeedbackCount
            FROM
                CATEGORY c
            INNER JOIN
                FEEDBACK f ON c.CategoryID = f.CategoryID
            GROUP BY
                c.CategoryType";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            var pieChartData = new
            {
                labels = dt.AsEnumerable().Select(row => row["CategoryType"].ToString()).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Feedback Distribution",
                        backgroundColor = new[]
                        {
                            "rgba(0, 0, 230, 1)",
                            "rgba(255, 0, 0, 1)",
                            "rgba(0, 128, 0, 1)",
                            "rgba(75, 0, 130, 1)",
                            "rgba(0, 0, 0, 1)",
                            "rgba(255, 165, 0, 1)",
                            "rgba(75, 192, 192, 1)" ,
                            "rgba(255, 206, 86, 1)" ,
                            "rgba(153, 102, 255, 1)",
                            "rgba(54, 162, 235, 1)",

                        },

                        data = dt.AsEnumerable().Select(row => Convert.ToInt32(row["FeedbackCount"])).ToArray()
                    }
                }
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(pieChartData);
        }




        [WebMethod]
        public static string GetLineChartDataForCategory(string category)
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            int categoryId = -1;

            if (category != "All")
            {
                categoryId = GetCategoryIdByName(category);

                if (categoryId == -1)
                {
                    return null;
                }
            }

            string query = @"
                SELECT
                    CAST(FeedbackDate AS DATE) AS FeedbackDate,
                    COUNT(FeedbackID) AS FeedbackCount
                FROM
                    FEEDBACK
                WHERE
                    (@CategoryID = -1 OR CategoryID = @CategoryID)
                GROUP BY
                    CAST(FeedbackDate AS DATE)
                ORDER BY
                    CAST(FeedbackDate AS DATE)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            var chartData = new
            {
                labels = dt.AsEnumerable().Select(row => Convert.ToDateTime(row["FeedbackDate"]).ToString("yyyy-MM-dd")).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Feedback Count",
                        backgroundColor = "rgba(75, 192, 192, 0.2)",
                        borderColor = "rgba(75, 192, 192, 1)",
                        borderWidth = 3,
                        data = dt.AsEnumerable()
                                 .Select(row => Convert.ToInt32(row["FeedbackCount"]))
                                 .ToArray()
                    }
                }
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(chartData);
        }



        private static int GetCategoryIdByName(string category)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT CategoryID FROM CATEGORY WHERE CategoryType = @CategoryType";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CategoryType", category);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int categoryId))
                    {
                        return categoryId;
                    }
                }
            }

            return -1;
        }


        private string GetBarChartData()
        {
            DataTable dt = FetchDataForCharts();

            var categories = dt.AsEnumerable().Select(row => row["CategoryType"].ToString()).Distinct().OrderBy(x => x).ToArray();

            var categoryData = dt.AsEnumerable().GroupBy(row => row["CategoryType"].ToString()).OrderBy(g => g.Key).Select(g => g.Count()).ToArray();

            var barChartData = new
            {
                labels = categories,
                datasets = new[]
                {
                    new
                    {
                        label = "Feedback Count",
                        data = categoryData,
                        backgroundColor = new[]
                        {
                             "rgba(0, 0, 230, 1)",
                            "rgba(255, 0, 0, 1)",
                            "rgba(0, 128, 0, 1)",
                            "rgba(75, 0, 130, 1)",
                            "rgba(0, 0, 0, 1)",
                            "rgba(255, 165, 0, 1)",
                            "rgba(75, 192, 192, 1)" ,
                            "rgba(255, 206, 86, 1)" ,
                            "rgba(153, 102, 255, 1)",
                            "rgba(54, 162, 235, 1)",


                        }
                    }
                }
            };
            return JsonConvert.SerializeObject(barChartData);
        }

        private DataTable FetchDataForCharts()
        {
            string con = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            using (SqlConnection db = new SqlConnection(con))
            {
                db.Open();
                string query = @"
            SELECT
                f.FeedbackDate,
                c.CategoryType,
                COUNT(*) AS Count
            FROM
                FEEDBACK f
            INNER JOIN
                CATEGORY c ON f.CategoryID = c.CategoryID
            GROUP BY
                f.FeedbackDate,
                c.CategoryType
            ORDER BY
                f.FeedbackDate";

                SqlCommand cmd = new SqlCommand(query, db);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }


        [WebMethod]
        public static string LoadFeedbackForCategory(string category)
        {
            string con = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            using (SqlConnection db = new SqlConnection(con))
            {
                db.Open();
                string query = string.Empty;
                int? categoryId = null;

                if (category != "Modules" && category != "Lecturers" && category != "Facilities" && category != "Tutors" && category != "Administration")
                {
                    categoryId = GetCategoryIdByName(category);
                }

                switch (category)
                {
                    case "Modules":
                        query = @"
                    SELECT DISTINCT
                        m.ModuleCode AS [Module Code],
                        m.ModuleName AS [Module Name],
                        m.ModuleYear AS [Module Year],
                        f.[FeedbackRating Line1] AS [Ratings 1],
                        f.[FeedbackRating Line2] AS [Ratings 2],
                        f.[FeedbackRating Line3] AS [Ratings 3],
                        f.Comment,
                        f.FeedbackDate AS [Date]
                    FROM MODULES m
                    INNER JOIN MODULE_FEEDBACK mf ON m.ModuleID = mf.ModuleID
                    INNER JOIN FEEDBACK f ON mf.FeedbackID = f.FeedbackID";
                        break;

                    case "Lecturers":
                        query = @"
                    SELECT DISTINCT
                        l.LecturerName AS [Lecturer Name],
                        l.LecturerSurname AS [Lecturer Surname],
                        m.ModuleCode AS [Module Code],
                        m.ModuleName AS [Module Name],
                        m.ModuleYear AS [Module Year],
                        f.[FeedbackRating Line1] AS [Ratings 1],
                        f.[FeedbackRating Line2] AS [Ratings 2],
                        f.[FeedbackRating Line3] AS [Ratings 3],
                        f.Comment,
                        f.FeedbackDate AS [Date]
                    FROM LECTURER l
                    INNER JOIN LECTURER_FEEDBACK lf ON l.LecturerID = lf.LecturerID
                    INNER JOIN FEEDBACK f ON lf.FeedbackID = f.FeedbackID
                    INNER JOIN MODULE_LECTURER lm ON l.LecturerID = lm.LecturerID
                    INNER JOIN MODULES m ON lm.ModuleID = m.ModuleID";
                        break;

                    case "Facilities":
                        query = @"
                    SELECT DISTINCT
                        f.FaciltyName AS [Facility Name],
                        fb.[FeedbackRating Line1] AS [Ratings 1],
                        fb.[FeedbackRating Line2] AS [Ratings 2],
                        fb.[FeedbackRating Line3] AS [Ratings 3],
                        fb.Comment,
                        fb.FeedbackDate AS [Date]
                    FROM FACILITY f
                    INNER JOIN FACILITY_FEEDBACK ff ON f.FacilityID = ff.FacilityID
                    INNER JOIN FEEDBACK fb ON ff.FeedbackID = fb.FeedbackID";
                        break;

                    case "Tutors":
                        query = @"
                    SELECT DISTINCT
                        t.TutorName AS [Tutor Name],
                        t.TutorSurname AS [Tutor Surname],
                        m.ModuleName AS [Module Name],
                        f.[FeedbackRating Line1] AS [Ratings 1],
                        f.[FeedbackRating Line2] AS [Ratings 2],
                        f.[FeedbackRating Line3] AS [Ratings 3],
                        f.Comment,
                        f.FeedbackDate AS [Date]
                    FROM
                        TUTOR t
                    INNER JOIN
                        TUTOR_FEEDBACK tf ON t.TutorID = tf.TutorID
                    INNER JOIN
                        FEEDBACK f ON tf.FeedbackID = f.FeedbackID
                    INNER JOIN
                        TUTOR_MODULES tm ON t.TutorID = tm.TutorID
                    INNER JOIN
                        MODULES m ON tm.ModuleID = m.ModuleID";
                        break;

                    case "Administration":
                        query = @"
                    SELECT DISTINCT
                        a.AdminName AS [Administrator Name],
                        a.AdminSurname AS [Administrator Surname],
                        f.[FeedbackRating Line1] AS [Ratings 1],
                        f.[FeedbackRating Line2] AS [Ratings 2],
                        f.[FeedbackRating Line3] AS [Ratings 3],
                        f.Comment,
                        f.FeedbackDate AS [Date]
                    FROM ADMIN_COORDINATOR a
                    INNER JOIN ADMIN_FEEDBACK af ON a.AdminID = af.AdminID
                    INNER JOIN FEEDBACK f ON af.FeedbackID = f.FeedbackID";
                        break;

                    default:
                        if (categoryId.HasValue)
                        {
                            query = @"
                        SELECT DISTINCT
                            f.FeedbackID,
                            f.[FeedbackRating Line1] AS [Ratings 1],
                            f.[FeedbackRating Line2] AS [Ratings 2],
                            f.[FeedbackRating Line3] AS [Ratings 3],
                            f.Comment,
                            f.FeedbackDate AS [Date]
                        FROM FEEDBACK f
                        WHERE f.CategoryID = @CategoryID";
                        }
                        else
                        {
                            return "Invalid category";
                        }
                        break;
                }

                SqlCommand cmd = new SqlCommand(query, db);
                if (categoryId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return RenderTable(dt);
            }
        }



        private static string RenderTable(DataTable dt)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='width:100%; text-align:center;'>");

            sb.Append("<tr><th colspan='" + dt.Columns.Count + "' style='text-align:center; font-size: 24px;'>Feedback</th></tr>");

            sb.Append("<tr>");
            foreach (DataColumn column in dt.Columns)
            {
                sb.AppendFormat("<th style='text-align:center;'>{0}</th>", column.ColumnName);
            }
            sb.Append("</tr>");

            foreach (DataRow row in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    var cellValue = row[column] != DBNull.Value ? row[column].ToString() : string.Empty;
                    sb.AppendFormat("<td style='text-align:center;'>{0}</td>", cellValue);
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }


    }
}
