<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="NexusEd.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NexusEd | Home</title>
      <link href="StyleSheet1.css" rel="stylesheet" />
    <script>
        window.onload = function () {
            setTimeout(function () {
                document.getElementById("loading-screen").style.display = "none";
            }, 500)
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

                <h1>Home</h1>
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

            <section id="flexItem1" class="home-main">
                <h2 class="home-title">Welcome to NexusEd</h2>
                <div class="homeContainer">
                    <div class="home-hero">
                        <div class="hero-media">
                            <img id="Image2" src="Images/Feedback.jpg" alt="Feedback illustration" />
                        </div>

                        <div class="hero-actions">
                            <asp:Button ID="btnShare" runat="server" Text="Give Feedback" OnClick="btnShare_Click" CssClass="centered-button"/>
                        </div>
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
