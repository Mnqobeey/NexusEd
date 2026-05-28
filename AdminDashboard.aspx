<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="NexusEd.AdminDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>
    <link href="StyleSheet1.css" rel="stylesheet" />
    <script>
        window.onload = function () {
            setTimeout(function () {
                document.getElementById("loading-screen").style.display = "none";
            }, 500);
        };
    </script>
</head>

<body>
    <form id="form1" runat="server">
        <div id="Page">

            <header>
                <a href="Home.aspx">
                    <img id="Image1" src="Images/1.png" alt="Logo" />
                </a>

                <h1>Admin Dashboard</h1>
            </header>

            <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>

            <nav>
                <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" DataSourceID="SiteMapDataSource1" StaticDisplayLevels="3">
                    <Items>
                        <asp:MenuItem NavigateUrl="~/Home.aspx" Text="Home" Value="Home"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Login.aspx" Text="Login" Value="Login"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Admin.aspx" Text="Admin" Value="Admin"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/FeedbackCategory.aspx" Text="Feedback" Value="Feedback"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/View.aspx"  Text="View" Value="View"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Report.aspx"  Text="Report" Value="Report"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Logout.aspx"  Text="Logout" Value="Logout"></asp:MenuItem>
                    </Items>

                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px"  VerticalPadding="4.5px" />

                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17"></StaticSelectedStyle>
                </asp:Menu>

                <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
                <a id="lnkSignIn" runat="server" href="Login.aspx" class="btnLogin6">Sign-in</a>
                <a id="lnkSignOut" runat="server" href="Logout.aspx" class="btnLogin5">Sign-out</a>

            </nav>

            <section id="flexItem1" class="admin-dashboard-shell">
                <div class="admin-dashboard-body">
                    <div class="admin-kpi-grid">
                        <article class="admin-kpi-card admin-kpi-blue">
                            <span class="admin-kpi-icon" aria-hidden="true">
                                <svg viewBox="0 0 24 24" focusable="false">
                                    <path d="M21 11.5a7.5 7.5 0 0 1-7.5 7.5H9l-5 3v-4.6A7.5 7.5 0 1 1 21 11.5Z"></path>
                                    <path d="M8.5 11.5h.01M12 11.5h.01M15.5 11.5h.01"></path>
                                </svg>
                            </span>
                            <div>
                                <p>Total Feedback</p>
                                <strong><asp:Literal ID="litTotalFeedback" runat="server">0</asp:Literal></strong>
                                <span>All time submissions</span>
                            </div>
                            <span class="admin-kpi-trend" aria-hidden="true">&#8599;</span>
                        </article>

                        <article class="admin-kpi-card admin-kpi-gold">
                            <span class="admin-kpi-icon" aria-hidden="true">
                                <svg viewBox="0 0 24 24" focusable="false">
                                    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8Z"></path>
                                    <path d="M14 2v6h6M8 13h8M8 17h6"></path>
                                </svg>
                            </span>
                            <div>
                                <p>Recent Submissions</p>
                                <strong><asp:Literal ID="litAdminFeedback" runat="server">0</asp:Literal></strong>
                                <span>Last 7 days</span>
                            </div>
                            <span class="admin-kpi-trend" aria-hidden="true">&#8599;</span>
                        </article>

                        <article class="admin-kpi-card admin-kpi-green">
                            <span class="admin-kpi-icon" aria-hidden="true">
                                <svg viewBox="0 0 24 24" focusable="false">
                                    <path d="M3 7a2 2 0 0 1 2-2h5l2 2h7a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2Z"></path>
                                </svg>
                            </span>
                            <div>
                                <p>Categories</p>
                                <strong><asp:Literal ID="litFacilitiesFeedback" runat="server">0</asp:Literal></strong>
                                <span>Active categories</span>
                            </div>
                            <span class="admin-kpi-trend" aria-hidden="true">&#8599;</span>
                        </article>

                        <article class="admin-kpi-card admin-kpi-amber">
                            <span class="admin-kpi-icon" aria-hidden="true">
                                <svg viewBox="0 0 24 24" focusable="false">
                                    <path d="m12 2.5 2.9 5.9 6.5.9-4.7 4.6 1.1 6.4-5.8-3-5.8 3 1.1-6.4-4.7-4.6 6.5-.9Z"></path>
                                </svg>
                            </span>
                            <div>
                                <p>Overall Rating</p>
                                <strong><asp:Literal ID="litLecturerFeedback" runat="server">0.0 / 5</asp:Literal></strong>
                                <span>Based on <asp:Literal ID="litRatedResponses" runat="server">0</asp:Literal> ratings</span>
                            </div>
                            <span class="admin-kpi-trend" aria-hidden="true">&#8599;</span>
                        </article>
                    </div>

                    <div class="admin-dashboard-grid">
                        <section class="admin-dashboard-panel admin-recent-panel">
                            <div class="admin-panel-header">
                                <div>
                                    <span class="admin-panel-icon admin-panel-clock" aria-hidden="true">
                                        <svg viewBox="0 0 24 24" focusable="false">
                                            <path d="M12 6v6l4 2"></path>
                                            <path d="M21 12a9 9 0 1 1-9-9"></path>
                                            <path d="M18 3v4h4"></path>
                                        </svg>
                                    </span>
                                    <h2>Recent Feedback</h2>
                                </div>
                                <a class="admin-panel-link" href="View.aspx">View all feedback</a>
                            </div>

                            <div class="admin-table-wrap">
                                <asp:GridView ID="gvRecentFeedback" runat="server" AutoGenerateColumns="False" GridLines="None" BorderWidth="0"
                                    AllowPaging="True" PageSize="5" OnPageIndexChanging="gvRecentFeedback_PageIndexChanging"
                                    CssClass="recent-feedback-table" EmptyDataText="No recent feedback submissions.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Category">
                                            <ItemTemplate>
                                                <span class="dashboard-category-cell">
                                                    <span class='<%# Eval("CategoryClass") %>' aria-hidden="true"></span>
                                                    <%# Eval("Category") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rating">
                                            <ItemTemplate>
                                                <span class="dashboard-rating-cell">
                                                    <span class="dashboard-star" aria-hidden="true">&#9733;</span>
                                                    <%# Eval("Rating") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="SubmittedDate" HeaderText="Date" DataFormatString="{0:MMM dd, yyyy}" />
                                    </Columns>
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="&lt;" LastPageText="&gt;" />
                                    <PagerStyle CssClass="dashboard-pager" HorizontalAlign="Center" />
                                </asp:GridView>
                            </div>
                        </section>

                        <aside class="admin-dashboard-panel admin-actions-panel">
                            <div class="admin-panel-header">
                                <div>
                                    <span class="admin-panel-icon admin-panel-bolt" aria-hidden="true">&#9889;</span>
                                    <h2>Quick Actions</h2>
                                </div>
                            </div>

                            <a class="admin-action-card admin-action-blue" href="View.aspx">
                                <span class="admin-action-icon" aria-hidden="true">
                                    <svg viewBox="0 0 24 24" focusable="false">
                                        <path d="M21 11.5a7.5 7.5 0 0 1-7.5 7.5H9l-5 3v-4.6A7.5 7.5 0 1 1 21 11.5Z"></path>
                                        <path d="M8.5 11.5h.01M12 11.5h.01M15.5 11.5h.01"></path>
                                    </svg>
                                </span>
                                <span class="admin-action-copy">
                                    <strong>View Feedback</strong>
                                    <small>Browse and review submitted feedback</small>
                                </span>
                                <span class="admin-action-arrow">&gt;</span>
                            </a>

                            <a class="admin-action-card admin-action-green" href="CategoryType.aspx">
                                <span class="admin-action-icon" aria-hidden="true">
                                    <svg viewBox="0 0 24 24" focusable="false">
                                        <path d="M3 7a2 2 0 0 1 2-2h5l2 2h7a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2Z"></path>
                                    </svg>
                                </span>
                                <span class="admin-action-copy">
                                    <strong>Manage Categories</strong>
                                    <small>Add, edit, or manage categories</small>
                                </span>
                                <span class="admin-action-arrow">&gt;</span>
                            </a>

                            <a class="admin-action-card admin-action-purple" href="Questions.aspx">
                                <span class="admin-action-icon" aria-hidden="true">
                                    <svg viewBox="0 0 24 24" focusable="false">
                                        <circle cx="12" cy="12" r="9"></circle>
                                        <path d="M9.8 9a2.2 2.2 0 1 1 3.8 1.5c-.8.7-1.6 1.2-1.6 2.5"></path>
                                        <path d="M12 17h.01"></path>
                                    </svg>
                                </span>
                                <span class="admin-action-copy">
                                    <strong>Manage Questions</strong>
                                    <small>Add, edit, or manage questions</small>
                                </span>
                                <span class="admin-action-arrow">&gt;</span>
                            </a>
                        </aside>
                    </div>

                </div>

                <div id="loading-screen">
                    <div class="loading-spinner"></div>
                    <p class="loading-text">Loading...</p>
                </div>
            </section>

            <footer>
                <p id="creator">NexusEd &copy; 2026</p>
            </footer>

        </div>

    </form>
</body>

</html>
