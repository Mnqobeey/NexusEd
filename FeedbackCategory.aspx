<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FeedbackCategory.aspx.cs" Inherits="NexusEd.FeedbackCategory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Feedback Category</title>
    <link href="StyleSheet1.css" rel="stylesheet" />
    <script type="text/javascript">
        window.onload = function () {
            setTimeout(function () {
                var loadingScreen = document.getElementById("loading-screen");
                if (loadingScreen) {
                    loadingScreen.style.display = "none";
                }
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
                <h1>Feedback</h1>
            </header>

            <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>

            <nav>
                <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" DataSourceID="SiteMapDataSource1" StaticDisplayLevels="3">
                    <Items>
                        <asp:MenuItem NavigateUrl="~/Home.aspx" Text="Home" />
                        <asp:MenuItem NavigateUrl="~/Login.aspx" Text="Login" />
                        <asp:MenuItem NavigateUrl="~/Admin.aspx" Text="Admin" />
                        <asp:MenuItem NavigateUrl="~/Feedback.aspx" Text="Feedback" />
                        <asp:MenuItem NavigateUrl="~/View.aspx" Text="View" />
                        <asp:MenuItem NavigateUrl="~/Report.aspx" Text="Report" />
                        <asp:MenuItem NavigateUrl="~/Logout.aspx" Text="Logout" />
                    </Items>
                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px" VerticalPadding="4.5px" />
                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17" />
                </asp:Menu>
                <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
                <a href="Home.aspx"type="button" class="btnLogin5" id="New">Sign-out</a>
            </nav>

            <center>
                <section id="flexItem1">
                    <div class="feedbackCategContainer">
                        <img  src="Images/your-opinion-matters.png" width="400" height="270" />
                        <h2 class="p-centered">Feedback</h2>
                        <asp:Label ID="lblsubtitle1" runat="server" Text="Please choose a category from the options."></asp:Label>
                        <br />
                        <br />
                        <div class="dropdown">
                            <asp:DropDownList ID="ddlCategories" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategories_SelectedIndexChanged" CssClass="form-control no-box">
                            </asp:DropDownList>
                        </div>
                        <br />
                        <asp:Label ID="lblsubtitle2" runat="server" Text="Select below to provide feedback"></asp:Label>
                        <br />
                        <br />

                        <center>
                            <asp:GridView ID="gvTableData" runat="server" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" ForeColor="Black" Width="609px" AutoGenerateSelectButton="True" OnSelectedIndexChanged="gvTableData_SelectedIndexChanged" HorizontalAlign="Center" PageSize="4">
                                <FooterStyle BackColor="#CCCCCC" />
                                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
                                <RowStyle BackColor="White" />
                                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#808080" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#383838" />
                            </asp:GridView>
                        </center>
                        <br />

                        <div>
                            <asp:TextBox ID="txtStudentID" runat="server" ReadOnly="true" CssClass="form-control" Visible="false" />
                            <asp:Image ID="imgRatings" runat="server" src="Images/ratings.png" Width="470" CssClass="float-right"/>
                            <asp:TextBox ID="txtSelectedID" runat="server" ReadOnly="true" CssClass="form-control" Visible="false"/>


                            <asp:Panel ID="pnlFeedbackForm" runat="server" CssClass="feedback-form-panel"></asp:Panel>
                            <br />
                            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Placeholder="Any comment/suggestions..." CssClass="textbox" Visible="false" />
                            <br />
                            <br />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btnAdd" />
                        </div>
                        <div id="loading-screen">
                            <div class="loading-spinner"></div>
                            <p class="loading-text">Loading...</p>
                        </div>
                    </div>
                </section>
            </center>
             <footer>
                 <p id="creator">NexusEd &copy; 2024</p>
            </footer>
        </div>
    </form>
</body>
</html>
