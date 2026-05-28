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

            var notice = document.getElementById("loginNotice");
            if (notice) {
                setTimeout(function () {
                    notice.classList.add("is-hiding");
                    setTimeout(function () {
                        notice.style.display = "none";
                    }, 260);
                }, 5500);
            }
        };

        function closeLoginNotice() {
            var notice = document.getElementById("loginNotice");
            if (notice) {
                notice.style.display = "none";
            }
        }

        function togglePasswordVisibility() {
            var passwordInput = document.getElementById('txtPassword');
            var toggleButton = document.getElementById('togglePasswordButton');

            if (!passwordInput || !toggleButton) {
                return false;
            }

            var shouldShow = passwordInput.type === 'password';
            passwordInput.type = shouldShow ? 'text' : 'password';
            toggleButton.classList.toggle('is-visible', shouldShow);
            toggleButton.setAttribute('aria-label', shouldShow ? 'Hide password' : 'Show password');
            toggleButton.setAttribute('title', shouldShow ? 'Hide password' : 'Show password');

            return false;
        }

    </script>
</head>


<body>
    <form id="form1" runat="server">
        <div id="Page">

             <header>
                <a href="Home.aspx">
                    <img id="Image1" src="Images/1.png" alt="Logo" />
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
                        <asp:MenuItem NavigateUrl="~/FeedbackCategory.aspx" Text="Feedback" Value="Feedback"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/View.aspx"  Text="View" Value="View"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Report.aspx"  Text="Report" Value="Report"></asp:MenuItem>
                    </Items>

                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px"  VerticalPadding="4.5px" />

                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17"></StaticSelectedStyle>
                </asp:Menu>

            <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
            <a id="lnkSignIn" runat="server" href="Login.aspx" class="btnLogin6">Sign-in</a>
            <a id="lnkSignOut" runat="server" href="Logout.aspx" class="btnLogin5">Sign-out</a>

            </nav>

            <section id="flexItem1">
                <div class="container">
                    <div id="loginNotice" runat="server" class="login-notice" visible="false">
                        <div class="login-notice-icon" aria-hidden="true">!</div>
                        <div class="login-notice-copy">
                            <strong>Access notice</strong>
                            <span><asp:Literal ID="litLoginNoticeMessage" runat="server"></asp:Literal></span>
                        </div>
                        <button type="button" class="login-notice-close" onclick="closeLoginNotice()" aria-label="Close">x</button>
                    </div>

                    <div class="login-layout">
                        <img src="Images/login.jpg" alt="Login illustration" class="login-illustration" />

                        <div class="login-panel">
                            <div class="page-eyebrow">NexusEd access</div>
                            <h2>Sign in</h2>

                            <div class="role-selector">
                                <asp:RadioButton ID="rbStudent" runat="server" GroupName="UserType" Text="Student" CssClass="form-radio" />
                                <asp:RadioButton ID="rbAdmin" runat="server" GroupName="UserType" Text="Administrator" CssClass="form-radio" />
                            </div>

                            <div class="form-stack">
                                <div class="form-row">
                                    <div class="login-input-shell">
                                        <span class="login-input-icon" aria-hidden="true">
                                            <img src="Images/user.png" alt="" />
                                        </span>
                                        <asp:TextBox ID="txtName" runat="server" Placeholder="Username" CssClass="user-textbox login-input" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Username is required." Font-Italic="True" ForeColor="Red" ControlToValidate="txtName" Font-Size="Small" CssClass="fieldValidator" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-row">
                                    <div class="login-input-shell">
                                        <span class="login-input-icon" aria-hidden="true">
                                            <img src="Images/key.png" alt="" />
                                        </span>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password" CssClass="password-textbox login-input" ClientIDMode="Static"></asp:TextBox>
                                        <button id="togglePasswordButton" type="button" class="password-toggle" onclick="return togglePasswordVisibility();" aria-label="Show password" title="Show password">
                                            <svg class="eye-icon eye-open" viewBox="0 0 24 24" aria-hidden="true" focusable="false">
                                                <path d="M2.5 12s3.5-6 9.5-6 9.5 6 9.5 6-3.5 6-9.5 6-9.5-6-9.5-6z"></path>
                                                <circle cx="12" cy="12" r="2.5"></circle>
                                            </svg>
                                            <svg class="eye-icon eye-closed" viewBox="0 0 24 24" aria-hidden="true" focusable="false">
                                                <path d="M3 3l18 18"></path>
                                                <path d="M10.6 10.6A2.5 2.5 0 0012 14.5c.7 0 1.3-.3 1.8-.7"></path>
                                                <path d="M6.5 6.8C4 8.5 2.5 12 2.5 12s3.5 6 9.5 6c1.8 0 3.4-.5 4.7-1.2"></path>
                                                <path d="M9.2 4.3C10.1 4.1 11 4 12 4c6 0 9.5 6 9.5 6s-.9 1.6-2.5 3.1"></path>
                                            </svg>
                                        </button>
                                    </div>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Password is required." Font-Italic="True" ForeColor="Red" ControlToValidate="txtPassword" Font-Size="Small" CssClass="fieldValidator" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>

                                <asp:Button ID="btnLogin" runat="server" Text="Sign in" OnClick="btnLogin_Click" />
                                <asp:Label ID="lblerrorMessage" runat="server" TextMode="" ForeColor="Red" Font-Italic="True" ></asp:Label>
                            </div>
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
