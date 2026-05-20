<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CategoryType.aspx.cs" Inherits="NexusEd.CategoryType" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Category</title>
    <link href="StyleSheet1.css" rel="stylesheet" />


    <script type="text/javascript">
        function showAddCategoryForm() {
            document.getElementById('addCategoryForm').style.display = 'block';
            return false;
        }

        function hideAddCategoryForm() {
            document.getElementById('addCategoryForm').style.display = 'none';
            return false;
        }

        function confirmProceed() {
            return confirm("The category has been added successfully. Would you like to proceed to add questions?");
        }

        function Delete() {
            return confirm("Are you sure you want to delete this category?");
        }

        function Update() {
            return confirm("Are you sure you want to update this category?");
        }

            window.onload = function () {
                setTimeout(function () {
                    document.getElementById("loading-screen").style.display = "none";
                }, 500);
    };
    </script>
</head>

<center>
<body>
    <form id="form1" runat="server">
        <div id="Page">
            <header>
                <a href="Home.aspx">
                    <img id="Image1" href="Home.aspx" src="Images/1.png" alt="Logo" />
                </a>
                <h1>Category Type</h1>
            </header>

            <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>

            <nav>
                <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" DataSourceID="SiteMapDataSource1" StaticDisplayLevels="3">
                    <Items>
                        <asp:MenuItem NavigateUrl="~/Home.aspx" Text="Home" Value="Home"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Login.aspx" Text="Login" Value="Login"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Admin.aspx" Text="Admin" Value="Admin"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Feedback.aspx" Text="Feedback" Value="Feedback"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/View.aspx" Text="View" Value="View"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Report.aspx" Text="Report" Value="Report"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Logout.aspx" Text="Logout" Value="Logout"></asp:MenuItem>
                    </Items>
                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px" VerticalPadding="4.5px" />
                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17"></StaticSelectedStyle>
                </asp:Menu>
                <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />

                <a href="Home.aspx"type="button" class="btnLogin5" id="New">Sign-out</a>

            </nav>

            <center>
            <section id="flexItem1">
                <div class="categoryTypeContainer">
                    <div class="header-container">
                        <h2 class="p-centered">Manage Category Type</h2>
                        <asp:Button ID="Button1" runat="server" Text="Add Category" OnClientClick="return showAddCategoryForm();" CssClass="btnShowAddCategoryForm" />
                    </div>
                    <p class="p-centered">Select to edit, or delete category type below.</p>
                    <br />

                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="CategoryID"
                        DataSourceID="SqlDataSource1" Width="684px" ForeColor="Black" BackColor="#CCCCCC"
                        BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2"
                        AutoGenerateSelectButton="True" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                        <Columns>
                            <asp:BoundField DataField="CategoryID" HeaderText="CategoryID" ReadOnly="True" SortExpression="CategoryID" />
                            <asp:BoundField DataField="CategoryType" HeaderText="CategoryType" SortExpression="CategoryType" />
                        </Columns>
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

                    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                        ConnectionString="<%$ ConnectionStrings:MyConnection %>"
                        SelectCommand="SELECT * FROM [CATEGORY]"
                        UpdateCommand="UPDATE CATEGORY SET CategoryType = @CategoryType WHERE CategoryID = @CategoryID"
                        DeleteCommand="DELETE FROM CATEGORY WHERE CategoryID = @CategoryID"
                        InsertCommand="INSERT INTO CATEGORY (CategoryType) VALUES (@CategoryType)">
                        <UpdateParameters>
                            <asp:Parameter Name="CategoryType" Type="String" />
                            <asp:Parameter Name="CategoryID" Type="Int32" />
                        </UpdateParameters>
                        <DeleteParameters>
                            <asp:Parameter Name="CategoryID" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="CategoryType" Type="String" />
                        </InsertParameters>
                    </asp:SqlDataSource>

                    <div id="editForm" runat="server" visible="false">
                            <table>
                                <thead>
                                  <tr>
                                    <th colspan="2">Edit/Delete Category</th>
                                  </tr>
                                </thead>
                                <tr>
                                    <td><asp:Label ID="lblCategoryID" runat="server" Text="Category ID:"></asp:Label></td>
                                     <td><asp:TextBox ID="txtCategoryID" runat="server" ReadOnly="True" BackColor="#CCCCCC" Width="23px" CssClass="textbox"></asp:TextBox></td>
                                </tr>

                                <br />
                                <tr>
                                    <td><asp:Label ID="lblCategoryType" runat="server" Text="Category Type:"></asp:Label></td>
                                    <td><asp:TextBox ID="txtCategoryTypeEdit" runat="server" CssClass="textbox"></asp:TextBox></td>
                                </tr>

                                <br />
                                <tr>
                                    <td><asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" OnClientClick="return Update();" CssClass="btnUpdate" /></td>
                                    <td><asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" OnClientClick="return Delete();" CssClass="btnDelete" /></td>
                                </tr>


                            </table>

                    </div>

                        <div id="addCategoryForm" runat="server" style="display:none;">
                            <table>
                                <thead>
                                  <tr>
                                    <th colspan="2">Add New Category</th>
                                  </tr>
                                </thead>
                                <tr>
                                    <td><asp:Label ID="lblCatType" runat="server" Text="Category Type:"></asp:Label></td>
                                    <td><asp:TextBox ID="txtCategoryType" runat="server" MaxLength="50" placeholder="Enter Category Type" CssClass="textbox"></asp:TextBox></td>
                                </tr>


                                <br />
                                <tr>
                                    <td><asp:Button ID="btnAddCategory" runat="server" Text="Add" OnClick="btnAddCategory_Click" CssClass="btnAdd"/></td>
                                    <td><asp:Button ID="btnCancelAddCategory" runat="server" Text="Cancel" OnClientClick="return hideAddCategoryForm();" CssClass="btnCancel"/></td>
                                </tr>


                            </table>


                        </div>
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
