using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NexusEd
{
    public static class AuthNavigation
    {
        public const string StudentRole = "Student";
        public const string AdminRole = "Admin_Coordinator";

        public static void Configure(Page page, Menu menu)
        {
            HtmlAnchor signIn = FindControlRecursive(page, "lnkSignIn") as HtmlAnchor;
            HtmlAnchor signOut = FindControlRecursive(page, "lnkSignOut") as HtmlAnchor;
            bool isLoggedIn = IsStudent(page) || IsAdmin(page);

            if (menu != null)
            {
                string currentPage = GetCurrentPageName(page);

                menu.DataSourceID = string.Empty;
                menu.DataSource = null;
                menu.Items.Clear();
                menu.StaticSelectedStyle.CssClass = "nav-active-link";

                AddMenuItem(menu, "Home", "~/Home.aspx", IsCurrentPage(currentPage, "Home.aspx"));

                if (IsStudent(page))
                {
                    AddMenuItem(menu, "Feedback", "~/FeedbackCategory.aspx", IsCurrentPage(currentPage, "FeedbackCategory.aspx"));
                }
                else if (IsAdmin(page))
                {
                    AddMenuItem(menu, "Admin", "~/Admin.aspx", IsCurrentPage(currentPage, "Admin.aspx", "CategoryType.aspx", "Questions.aspx"));
                    AddMenuItem(menu, "Dashboard", "~/AdminDashboard.aspx", IsCurrentPage(currentPage, "AdminDashboard.aspx"));
                    AddMenuItem(menu, "View", "~/View.aspx", IsCurrentPage(currentPage, "View.aspx"));
                    AddMenuItem(menu, "Report", "~/Report.aspx", IsCurrentPage(currentPage, "Report.aspx"));
                }
                else
                {
                    AddMenuItem(menu, "Feedback", "~/FeedbackCategory.aspx", IsCurrentPage(currentPage, "FeedbackCategory.aspx"));
                    AddMenuItem(menu, "Admin", "~/Admin.aspx", IsCurrentPage(currentPage, "Admin.aspx", "AdminDashboard.aspx", "CategoryType.aspx", "Questions.aspx", "View.aspx"));
                    AddMenuItem(menu, "Report", "~/Report.aspx", IsCurrentPage(currentPage, "Report.aspx"));
                    if (signIn == null)
                    {
                        AddMenuItem(menu, "Sign-in", "~/Login.aspx", IsCurrentPage(currentPage, "Login.aspx"));
                    }
                }

                if (isLoggedIn && signOut == null)
                {
                    AddMenuItem(menu, "Sign-out", "~/Logout.aspx", IsCurrentPage(currentPage, "Logout.aspx"));
                }
            }

            if (signIn != null)
            {
                signIn.Visible = !isLoggedIn;
                signIn.HRef = page.ResolveUrl("~/Login.aspx");
                SetAnchorActiveState(signIn, "btnLogin6", IsCurrentPage(GetCurrentPageName(page), "Login.aspx"));
            }

            if (signOut != null)
            {
                signOut.Visible = isLoggedIn;
                signOut.HRef = page.ResolveUrl("~/Logout.aspx");
                SetAnchorActiveState(signOut, "btnLogin5", false);
            }
        }

        public static bool RequireAdmin(Page page)
        {
            if (IsAdmin(page))
            {
                return true;
            }

            RedirectToLogin(page, IsKnownUser(page) ? "unauthorized" : "loginRequired");
            return false;
        }

        public static bool RequireStudent(Page page)
        {
            if (IsStudent(page))
            {
                return true;
            }

            RedirectToLogin(page, IsKnownUser(page) ? "unauthorized" : "loginRequired");
            return false;
        }

        public static bool IsStudent(Page page)
        {
            return HasRole(page, StudentRole);
        }

        public static bool IsAdmin(Page page)
        {
            return HasRole(page, AdminRole);
        }

        public static string GetDefaultLandingUrl(string role)
        {
            if (string.Equals(role, StudentRole, StringComparison.OrdinalIgnoreCase))
            {
                return "FeedbackCategory.aspx";
            }

            if (string.Equals(role, AdminRole, StringComparison.OrdinalIgnoreCase))
            {
                return "Admin.aspx";
            }

            return "Home.aspx";
        }

        public static bool IsReturnUrlAllowedForRole(string returnUrl, string role)
        {
            string pageName = GetReturnPageName(returnUrl);
            if (string.IsNullOrEmpty(pageName))
            {
                return false;
            }

            if (string.Equals(role, StudentRole, StringComparison.OrdinalIgnoreCase))
            {
                return pageName == "Home.aspx" || pageName == "FeedbackCategory.aspx";
            }

            if (string.Equals(role, AdminRole, StringComparison.OrdinalIgnoreCase))
            {
                return pageName == "Home.aspx"
                    || pageName == "Admin.aspx"
                    || pageName == "AdminDashboard.aspx"
                    || pageName == "CategoryType.aspx"
                    || pageName == "Questions.aspx"
                    || pageName == "View.aspx"
                    || pageName == "Report.aspx";
            }

            return false;
        }

        public static string NormalizeReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return string.Empty;
            }

            if (Uri.TryCreate(returnUrl, UriKind.Absolute, out _))
            {
                return string.Empty;
            }

            string normalized = returnUrl.Trim();
            if (normalized.Contains(":") || normalized.Contains("\\"))
            {
                return string.Empty;
            }

            return normalized.TrimStart('~', '/');
        }

        private static bool HasRole(Page page, string role)
        {
            object sessionRole = page.Session["SelectedUserType"];
            return sessionRole != null
                && string.Equals(sessionRole.ToString(), role, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKnownUser(Page page)
        {
            return IsStudent(page) || IsAdmin(page);
        }

        private static void RedirectToLogin(Page page, string messageKey)
        {
            string returnUrl = NormalizeReturnUrl(page.Request.RawUrl);
            string loginUrl = "Login.aspx?msg=" + HttpUtility.UrlEncode(messageKey);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                loginUrl += "&ReturnUrl=" + HttpUtility.UrlEncode(returnUrl);
            }

            page.Response.Redirect(loginUrl, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private static void AddMenuItem(Menu menu, string text, string url, bool isSelected)
        {
            MenuItem item = new MenuItem(text, text, string.Empty, url)
            {
                ToolTip = isSelected ? text + " - current page" : text
            };

            menu.Items.Add(item);

            if (isSelected)
            {
                item.Selected = true;
            }
        }

        private static string GetReturnPageName(string returnUrl)
        {
            string normalized = NormalizeReturnUrl(returnUrl);
            if (string.IsNullOrEmpty(normalized))
            {
                return string.Empty;
            }

            int queryIndex = normalized.IndexOf('?');
            if (queryIndex >= 0)
            {
                normalized = normalized.Substring(0, queryIndex);
            }

            int slashIndex = normalized.LastIndexOf('/');
            return slashIndex >= 0 ? normalized.Substring(slashIndex + 1) : normalized;
        }

        private static string GetCurrentPageName(Page page)
        {
            string path = page.Request.AppRelativeCurrentExecutionFilePath;
            if (string.IsNullOrEmpty(path))
            {
                path = page.Request.Url.AbsolutePath;
            }

            int slashIndex = path.LastIndexOf('/');
            return slashIndex >= 0 ? path.Substring(slashIndex + 1) : path;
        }

        private static bool IsCurrentPage(string currentPage, params string[] pageNames)
        {
            foreach (string pageName in pageNames)
            {
                if (string.Equals(currentPage, pageName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetAnchorActiveState(HtmlAnchor anchor, string baseCssClass, bool isActive)
        {
            anchor.Attributes["class"] = isActive ? baseCssClass + " nav-active-link" : baseCssClass;
            if (isActive)
            {
                anchor.Attributes["aria-current"] = "page";
            }
            else
            {
                anchor.Attributes.Remove("aria-current");
            }
        }

        private static Control FindControlRecursive(Control root, string id)
        {
            if (root == null)
            {
                return null;
            }

            if (string.Equals(root.ID, id, StringComparison.Ordinal))
            {
                return root;
            }

            foreach (Control child in root.Controls)
            {
                Control result = FindControlRecursive(child, id);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
