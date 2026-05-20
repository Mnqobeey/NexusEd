<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Questions.aspx.cs" Inherits="NexusEd.Questions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Questions</title>
    <link href="StyleSheet1.css" rel="stylesheet" />
        <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" rel="stylesheet" />



    <script type="text/javascript">
        function showAddQuestionsForm() {
            document.getElementById('addQuestionsForm').style.display = 'block';
            return false;
        }

        function hideAddQuestionsForm() {
            document.getElementById('addQuestionsForm').style.display = 'none';
            return false;
        }

        function QuesDelete() {
            return confirm("Are you sure you want to delete this question?");
        }

        function QuesUpdate() {
            return confirm("Are you sure you want to update this question?");
        }

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
                <h1>Questions</h1>
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
                <div class="qContainer">
                    <div class="header-container">
                        <h2 class="p-centered">Manage Feedback Questions</h2>
                        <asp:Button ID="Button1" runat="server" Text="Add Question" OnClientClick="return showAddQuestionsForm();" CssClass="btnShowAddCategoryForm" />
                    </div>
                    <p class="p-centered">Select to edit or delete questions below.</p>
                    <br />

                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="QuestionID"
                        DataSourceID="SqlDataSource1" Width="684px" ForeColor="Black" BackColor="#CCCCCC"
                        BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2"
                        AutoGenerateSelectButton="True" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" AllowPaging="True" PageSize="7" HorizontalAlign="Center">
                        <Columns>
                            <asp:BoundField DataField="QuestionID" HeaderText="QuestionID" ReadOnly="True" SortExpression="QuestionID" />
                            <asp:BoundField DataField="Question" HeaderText="Question" SortExpression="Question" />
                            <asp:BoundField DataField="CategoryID" HeaderText="CategoryID" SortExpression="CategoryID" />
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
                        SelectCommand="SELECT * FROM [FEEDBACK_QUESTIONS]"
                        UpdateCommand="UPDATE FEEDBACK_QUESTIONS SET Question = @Question WHERE QuestionID = @QuestionID"
                        DeleteCommand="DELETE FROM FEEDBACK_QUESTIONS WHERE QuestionID = @QuestionID"
                        InsertCommand="INSERT INTO FEEDBACK_QUESTIONS (Question, CategoryID) VALUES (@Question, @CategoryID)">
                        <UpdateParameters>
                            <asp:Parameter Name="Question" Type="String" />
                            <asp:Parameter Name="QuestionID" Type="Int32" />
                        </UpdateParameters>
                        <DeleteParameters>
                            <asp:Parameter Name="QuestionID" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="Question" Type="String" />
                            <asp:Parameter Name="CategoryID" Type="Int32" />
                        </InsertParameters>
                    </asp:SqlDataSource>

                    <div id="editForm" runat="server" visible="false">
                        <table>
                            <thead>
                                <tr>
                                    <th colspan="2">Edit/Delete Question</th>
                                </tr>
                            </thead>
                            <tr>
                                <td><asp:Label ID="lblQuestionID" runat="server" Text="Question ID:"></asp:Label></td>
                                <td><asp:TextBox ID="txtQuestionID" runat="server" ReadOnly="True" BackColor="#CCCCCC" Width="23px" CssClass="textbox"></asp:TextBox></td>
                            </tr>
                            <br />
                            <tr>
                                <td><asp:Label ID="lblQuestion" runat="server" Text="Question:"></asp:Label></td>
                                <td><asp:TextBox ID="txtQuestionEdit" runat="server" TextMode="MultiLine" CssClass="textbox"></asp:TextBox></td>
                            </tr>
                            <br />
                            <tr>
                                <td><asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" OnClientClick="return QuesUpdate();" CssClass="btnUpdate" /></td>
                                <td><asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" OnClientClick="return QuesDelete();" CssClass="btnDelete" /></td>
                            </tr>
                        </table>
                    </div>

                    <div id="addQuestionsForm" runat="server" style="display:none;">
                        <table>
                            <thead>
                                <tr>
                                    <th colspan="2">Add New Question</th>
                                </tr>
                            </thead>
                            <tr>
                                <td><asp:Label ID="lblCategoryDropdown" runat="server" Text="Select Category:" ></asp:Label></td>
                                <td><asp:DropDownList ID="ddlCategories" runat="server"  class="form-control no-box1 "></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label1" runat="server" Text="Question:"></asp:Label></td>
                                <td><asp:TextBox ID="txtQuestion" runat="server"  placeholder="Enter a question..." TextMode="MultiLine" CssClass="textbox"></asp:TextBox></td>
                            </tr>

                            <br />
                            <tr>
                                <td><asp:Button ID="btnAddQuestion" runat="server" Text="Add" OnClick="btnAddQuestion_Click" CssClass="btnAdd" /></td>
                                <td><asp:Button ID="btnCancelAddQuestion" runat="server" Text="Cancel" OnClientClick="return hideAddQuestionsForm();" CssClass="btnCancel" /></td>
                            </tr>
                        </table>
                    </div>

                    <asp:Label ID="lblError" runat="server" TextMode="" ForeColor="Red" Font-Italic="True" ></asp:Label>
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
