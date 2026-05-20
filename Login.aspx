<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NexusEd.Login" %>

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

                <h1>Login</h1>
            </header>

            <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>

            <nav>
                <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" DataSourceID="SiteMapDataSource1" StaticDisplayLevels="3" CssClass="Menu1">
                    <Items>
                        <asp:MenuItem NavigateUrl="~/Home.aspx" Text="Home" Value="Home"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Login.aspx" Text="Login" Value="Login"></asp:MenuItem>

                        <asp:MenuItem NavigateUrl="~/Admin.aspx" Text="Admin" Value="Admin"/>
                            <asp:MenuItem NavigateUrl="~/CategoryType.aspx" Text="Manage Category" Value="Manage Category" />
                            <asp:MenuItem NavigateUrl="~/Questions.aspx" Text="Manage Questions" Value="Manage Questions" />
                        <asp:MenuItem NavigateUrl="~/Feedback.aspx" Text="Feedback" Value="Feedback"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/View.aspx"  Text="View" Value="View"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Report.aspx"  Text="Report" Value="Report"></asp:MenuItem>
                    </Items>

                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px"  VerticalPadding="4.5px" />

                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17"></StaticSelectedStyle>
                </asp:Menu>

            <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />

            </nav>

            <center>
            <section id="flexItem1">
                <div class="container">
                    <img src="Images/login.jpg" alt="login img" height="250" width="400"/>
                <h2>Sign in</h2>
                    <p> *Please sign in to access the system. </p>
                    <asp:RadioButton ID="rbStudent" runat="server" GroupName="UserType" Text="Student" CssClass="form-radio" />
                     <asp:RadioButton ID="rbAdmin" runat="server" GroupName="UserType" Text="Administrator" CssClass="form-radio" />

                    <table>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtName" runat="server" Placeholder="Username" CssClass="user-textbox"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Enter Username" Font-Italic="True" ForeColor="Red" ControlToValidate="txtName" Font-Size="Small" CssClass="fieldValidator"></asp:RequiredFieldValidator>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password" CssClass="password-textbox"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Enter Password" Font-Italic="True" ForeColor="Red" ControlToValidate="txtPassword" Font-Size="Small" CssClass="fieldValidator"></asp:RequiredFieldValidator>
                            </td>
                        </tr>


                    </table>
                    <br />
                    <asp:Button ID="btnLogin" runat="server" Text="Sign in" OnClick="btnLogin_Click" />
                        <br />
                    <br />
                    <br />
                    <br />
                        <asp:Label ID="lblerrorMessage" runat="server" TextMode="" ForeColor="Red" Font-Italic="True" ></asp:Label>

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
