<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="NexusEd.Report" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report</title>
      <link href="StyleSheet1.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js@3.9.1"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2.0.0"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-chart-wordcloud@3"></script>




</head>

<body>
    <form id="form1" runat="server">
        <div id="Page">

             <header>
                <a href="Home.aspx">
                    <img id="Image1" href="Home.aspx" src="Images/1.png" alt="Logo" />
                </a>

                <h1>Report</h1>
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
            <a href="Home.aspx"type="button" class="btnLogin5" id="New">Sign-out</a>

            </nav>


            <div class="rContainer">
              <div class="report-header-container">
                    <center>
                        <div><h2>Feedback dashboard</h2></div>
                    </center>
                  <a href="Report.aspx">
                        <img id="refresh-img" src="Images/refresh.png"  height="20px" width="20px"/>
                        <p>Refresh</p>
                  </a>

                </div>
                <div class="flexRow">
                    <div id="flexItem4">
                        <canvas id="barChart"></canvas>
                    </div>
                    <div id="flexItem3">
                        <canvas id="lineChart"></canvas>
                    </div>

                </div>

                <div class="flexRow">
                    <div id="flexItem7">
                        <canvas id="pieChart"></canvas>
                    </div>
                    <div id="flexItem6">
                        <canvas id="groupedBarChart"></canvas>
                    </div>
                </div>

            <br />

           <center>
           <div id="flexItem5">
            <asp:GridView ID="FeedbackGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="FeedbackID" DataSourceID="SqlDataSource1" CssClass="feedbackContainer" >
                <Columns>
                </Columns>
            </asp:GridView>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:MyConnection %>" SelectCommand="SELECT * FROM [FEEDBACK]"></asp:SqlDataSource>
            </div>
            <br />

        <script>
            let barChart = null;
            let lineChart = null;
            let pieChart = null;
            let groupedBarChart = null;
            let previouslySelectedIndex = null;
            let selectedBarColor = 'rgba(255, 99, 132, 1)';

            function initializeCharts() {
                if (groupedBarChart) groupedBarChart.destroy();
                if (pieChart) pieChart.destroy();
                if (barChart) barChart.destroy();
                if (lineChart) lineChart.destroy();

                $.ajax({
                    type: "POST",
                    url: "Report.aspx/GetPieChartData",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var pieData = JSON.parse(response.d);
                        var ctxPie = document.getElementById('pieChart').getContext('2d');

                        pieChart = new Chart(ctxPie, {
                            type: 'pie',
                            data: pieData,
                            options: {
                                responsive: true,
                                maintainAspectRatio: false,
                                plugins: {
                                    legend: {
                                        position: 'top',
                                        labels: {
                                            font: { size: 14, weight: 'bold' }
                                        }
                                    },
                                    tooltip: {
                                        callbacks: {
                                            label: function (tooltipItem) {
                                                const label = tooltipItem.label || '';
                                                const value = tooltipItem.raw || 0;
                                                const total = tooltipItem.dataset.data.reduce((a, b) => a + b, 0);
                                                const percentage = ((value / total) * 100).toFixed(1);
                                                return `${label}: ${value} (${percentage}%)`;
                                            }
                                        }
                                    },
                                    title: {
                                        display: true,
                                        text: 'Feedback Distribution',
                                        font: { size: 18, weight: 'bold' }
                                    },
                                    datalabels: {
                                        color: '#fff',
                                        font: { size: 14, weight: 'bold' },
                                        formatter: function (value, ctx) {
                                            const total = ctx.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                                            const percentage = ((value / total) * 100).toFixed(1);
                                            return `${percentage}%`;
                                        }
                                    }
                                }
                            },
                            plugins: [ChartDataLabels]
                        });

                        pieChart.canvas.style.width = '200px';
                        pieChart.canvas.style.height = '400px';
                    },
                    error: function (error) {
                        console.error("Error fetching pie chart data", error);
                    }
                });


                var ctxBar = document.getElementById('barChart').getContext('2d');
                var barChartData = <%= this.BarChartData %>;

                barChartData.datasets.forEach((dataset) => {
                    if (!dataset.originalBackgroundColor) {
                        dataset.originalBackgroundColor = dataset.backgroundColor ? [...dataset.backgroundColor] : [];
                    }
                });

                barChart = new Chart(ctxBar, {
                    type: 'bar',
                    data: barChartData,
                    options: {
                        maintainAspectRatio: false,
                        responsive: true,
                        indexAxis: 'x',
                        scales: {
                            x: {
                                title: {
                                    display: true,
                                    text: 'Category',
                                    font: { weight: 'bold', size: 14 }
                                }
                            },
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Count',
                                    font: { weight: 'bold', size: 14 }
                                }
                            }
                        },
                        plugins: {
                            legend: { display: false },
                            title: {
                                display: true,
                                text: 'Feedback Category',
                                font: { weight: 'bold', size: 18 }
                            }
                        },
                        onClick: (event, elements) => {
                            if (elements.length > 0) {
                                const bar = elements[0];
                                const barIndex = bar.index;

                                if (previouslySelectedIndex !== null) {
                                    updateChartAppearance(barChart, previouslySelectedIndex);
                                }

                                updateChartAppearance(barChart, barIndex);

                                var category = barChart.data.labels[barIndex];
                                loadFeedbackForCategory(category);

                                updateLineChart(category, selectedBarColor);

                                updatePieChartAppearance(barIndex);

                                fetchGroupedBarChartData(category);

                                previouslySelectedIndex = barIndex;
                            }
                        }
                    }
                });


                var ctxLine = document.getElementById('lineChart').getContext('2d');
                var lineChartData = <%= this.LineChartData %>;

                lineChart = new Chart(ctxLine, {
                    type: 'line',
                    data: lineChartData,
                    options: {
                        plugins: {
                            legend: { display: false },
                            title: {
                                display: true,
                                text: 'Student Feedback Trend',
                                font: { weight: 'bold', size: 18 },
                                subtitle: {
                                    display: true,
                                    text: 'Feedback over time',
                                    font: { weight: 'normal', size: 14 }
                                }
                            }
                        },
                        scales: {
                            x: {
                                title: {
                                    display: true,
                                    text: 'Date',
                                    font: { weight: 'bold', size: 14 }
                                }
                            },
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Count',
                                    font: { weight: 'bold', size: 14 }
                                }
                            }
                        }
                    }
                });
            }




            function fetchGroupedBarChartData(selectedCategory = null) {
                const canvas = document.getElementById('groupedBarChart');
                const ctx = canvas.getContext('2d');

                canvas.width = 200;
                canvas.height = 400;

                const url = '<%= ResolveUrl("~/Report.aspx/GetGroupedBarChartData") %>';
                const requestData = JSON.stringify({ selectedCategory: selectedCategory });

                $.ajax({
                    type: "POST",
                    url: url,
                    data: requestData,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        const groupedBarData = JSON.parse(response.d);

                        if (groupedBarChart) {
                            groupedBarChart.data = groupedBarData;
                            groupedBarChart.options.plugins.title.text = selectedCategory
                                ? `Feedback Ratings for ${selectedCategory}`
                                : 'Feedback Ratings';
                            groupedBarChart.update();
                        } else {
                            groupedBarChart = new Chart(ctx, {
                                type: 'bar',
                                data: groupedBarData,
                                options: {
                                    maintainAspectRatio: false,
                                    responsive: true,
                                    plugins: {
                                        title: {
                                            display: true,
                                            text: selectedCategory
                                                ? `Feedback Ratings for ${selectedCategory}`
                                                : 'Feedback Ratings',
                                            font: { size: 18, weight: 'bold' }
                                        }
                                    },

                                }
                            });
                        }

                        setTimeout(() => {
                            canvas.style.width = '200px';
                            canvas.style.height = '400px';
                            groupedBarChart.resize();
                        }, 0);
                    },
                    error: function (error) {
                        console.error("Error fetching grouped bar chart data", error);
                    }
                });
            }



            function updateChartAppearance(barChart, index) {
                barChart.data.datasets.forEach((dataset) => {
                    if (!dataset.backgroundColor) {
                        dataset.backgroundColor = [];
                    }
                    if (!dataset.borderWidth) {
                        dataset.borderWidth = [];
                    }

                    dataset.data.forEach((value, i) => {
                        if (i === index) {
                            selectedBarColor = dataset.originalBackgroundColor[i] || 'rgba(255, 99, 132, 1)';
                            dataset.backgroundColor[i] = selectedBarColor;
                            dataset.borderWidth[i] = 2;
                            dataset.barThickness = 40;
                        } else {
                            dataset.backgroundColor[i] = 'rgba(255, 99, 132, 0.2)';
                            dataset.borderWidth[i] = 1;
                            dataset.barThickness = 30;
                        }
                    });
                });

                barChart.update();
            }

            function updatePieChartAppearance(barIndex) {
                if (pieChart) {
                    const selectedCategory = barChart.data.labels[barIndex];

                    pieChart.options.plugins.title.text = `Feedback Distribution % for ${selectedCategory}`;

                    const pieIndex = pieChart.data.labels.indexOf(selectedCategory);

                    pieChart.data.datasets.forEach(dataset => {
                        dataset.backgroundColor = dataset.backgroundColor.map((color, index) =>
                            index === pieIndex ? selectedBarColor : 'rgba(255, 99, 132, 0.2)'
                        );
                    });

                    pieChart.update();
                }
            }


            function updateLineChart(category, color) {
                $.ajax({
                    type: "POST",
                    url: '<%= ResolveUrl("~/Report.aspx/GetLineChartDataForCategory") %>',
                    data: JSON.stringify({ category: category }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var lineChartData = JSON.parse(response.d);
                        if (lineChart) {
                            lineChart.data = lineChartData;
                            lineChart.options.plugins.title.text = `Feedback Trend for ${category}`;
                            lineChart.data.datasets.forEach(dataset => {
                                dataset.borderColor = color;
                                dataset.backgroundColor = color;
                            });
                            lineChart.update();
                        } else {
                            var ctxLine = document.getElementById('lineChart').getContext('2d');
                            lineChart = new Chart(ctxLine, {
                                type: 'line',
                                data: lineChartData,
                                options: {
                                    plugins: {
                                        title: {
                                            display: true,
                                            text: `Feedback Trend for ${category}`,
                                            font: { weight: 'bold', size: 18 }
                                        },
                                        legend: { display: false },
                                    },
                                    scales: {
                                        x: {
                                            title: {
                                                display: true,
                                                text: 'Date',
                                                font: { weight: 'bold', size: 14 }
                                            }
                                        },
                                        y: {
                                            beginAtZero: true,
                                            title: {
                                                display: true,
                                                text: 'Count',
                                                font: { weight: 'bold', size: 14 }
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    },
                    error: function (error) {
                        console.error("Error fetching line chart data", error);
                    }
                });
            }



            function loadFeedbackForCategory(category) {
                $.ajax({
                    type: "POST",
                    url: '<%= ResolveUrl("~/Report.aspx/LoadFeedbackForCategory") %>',
                    data: JSON.stringify({ category: category }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#<%= FeedbackGridView.ClientID %>').html(response.d);
                        $('#<%= FeedbackGridView.ClientID %>').slideDown();
                    },
                    error: function (xhr, status, error) {
                        console.error("Error occurred: " + error);
                    }
                });
            }

            function clearFeedbackGrid() {
                $('#<%= FeedbackGridView.ClientID %>').html('');
                $('#<%= FeedbackGridView.ClientID %>').slideUp();
            }

            function removeLineChartPoint(barIndex) {
                if (lineChart) {
                    lineChart.data.datasets.forEach((dataset) => {
                        dataset.data.splice(barIndex, 1);
                    });
                    lineChart.data.labels.splice(barIndex, 1);
                    lineChart.update();
                }
            }

            window.onload = function () {
                fetchGroupedBarChartData();
                initializeCharts();

                document.getElementById('downloadPdf').addEventListener('click', function () {
                    var barChartCanvas = document.getElementById('barChart');
                    var lineChartCanvas = document.getElementById('lineChart');
                    var pieChartCanvas = document.getElementById('pieChart');
                    var groupedBarChartCanvas = document.getElementById('groupedBarChart');

                    var barChartImage = barChartCanvas.toDataURL('image/png');
                    var lineChartImage = lineChartCanvas.toDataURL('image/png');
                    var pieChartImage = pieChartCanvas.toDataURL('image/png');
                    var groupedBarChartImage = groupedBarChartCanvas.toDataURL('image/png');

                    const { jsPDF } = window.jspdf;
                    var pdf = new jsPDF({
                        orientation: 'portrait',
                        unit: 'px',
                        format: 'a4'
                    });

                    pdf.setFontSize(22);
                    pdf.text(140, 20, 'Feedback Report - Page 1');

                    pdf.setFontSize(16);
                    pdf.text(20, 50, '1. Feedback Summary Bar Chart');
                    pdf.addImage(barChartImage, 'PNG', 20, 60, 390, 250);

                    pdf.text(20, 340, '2. Feedback Trends Line Chart');
                    pdf.addImage(lineChartImage, 'PNG', 20, 360, 390, 250);

                    pdf.addPage();

                    pdf.setFontSize(22);
                    pdf.text(140, 20, 'Feedback Report - Page 2');

                    pdf.setFontSize(16);
                    pdf.text(20, 50, '3. Feedback Distribution Pie Chart');
                    pdf.addImage(pieChartImage, 'PNG', -20, 60, 490, 250);

                    pdf.text(20, 340, '4. Feedback Ratings Grouped Bar Chart');
                    pdf.addImage(groupedBarChartImage, 'PNG', 20, 360, 390, 250);

                    pdf.save('Feedback_Report.pdf');
                });

            };
        </script>


           <button id="downloadPdf" onclick="downloadPDF()">Download Report</button>
            </center>
           </div>

             <footer>
                 <p id="creator">NexusEd &copy; 2024</p>
            </footer>
        </div>
   </form>
</body>
</html>
