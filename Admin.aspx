<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="NexusEd.Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
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
                    <img id="Image1" href="Home.aspx" src="Images/1.png" alt="Logo" />
                </a>

                <h1>Admin</h1>
            </header>

            <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>

            <nav>
                <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" DataSourceID="SiteMapDataSource1" StaticDisplayLevels="3">
                    <Items>
                        <asp:MenuItem NavigateUrl="~/Home.aspx" Text="Home" Value="Home"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Login.aspx" Text="Login" Value="Login"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Admin.aspx" Text="Admin" Value="Admin"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Feedback.aspx" Text="Feedback" Value="Feedback"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/View.aspx"  Text="View" Value="View"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Report.aspx"  Text="Report" Value="Report"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Logout.aspx"  Text="Logout" Value="Logout"></asp:MenuItem>
                    </Items>

                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px"  VerticalPadding="4.5px" />

                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17"></StaticSelectedStyle>
                </asp:Menu>

            <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />


            </nav>

            <center>
            <section id="flexItem1">
                <div class="container">
                    <img src="Images/manage.jpg" alt="admin logo" width="380" height="310" />
                    <h2 class="p-centered">Manage Feedback</h2>
                <p class="p-centered">Select what you would like to manage below</p>
                    <br />

                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnCat" runat="server" Text="Category Type" CssClass="btnAdmin"  OnClick="btnCat_Click" Width="125px" ></asp:Button>
                        </td>

                      <td>
                           <asp:Button ID="btnQues" runat="server" Text="Questions" CssClass="btnAdmin"  OnClick="btnQues_Click" Width="125px"></asp:Button>
                      </td>
                    </tr>

                </table>
                    <br />
                    <br />
                    <br />
                    <br />

                </div>

                <div id="loading-screen">
                  <div class="loading-spinner"></div>
                  <p class="loading-text">Loading...</p>
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
