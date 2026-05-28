<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="NexusEd.Report" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report</title>
    <link href="StyleSheet1.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js@3.9.1"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="Page">
            <header>
                <a href="Home.aspx">
                    <img id="Image1" src="Images/1.png" alt="NexusEd logo" />
                </a>

                <h1>Reports</h1>
            </header>

            <asp:SiteMapPath ID="SiteMapPath1" runat="server" CssClass="app-breadcrumb"></asp:SiteMapPath>

            <nav>
                <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" DataSourceID="SiteMapDataSource1" StaticDisplayLevels="3">
                    <Items>
                        <asp:MenuItem NavigateUrl="~/Home.aspx" Text="Home" Value="Home"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Login.aspx" Text="Login" Value="Login"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Admin.aspx" Text="Admin" Value="Admin"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/FeedbackCategory.aspx" Text="Feedback" Value="Feedback"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/View.aspx" Text="View" Value="View"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Report.aspx" Text="Report" Value="Report"></asp:MenuItem>
                        <asp:MenuItem NavigateUrl="~/Logout.aspx" Text="Logout" Value="Logout"></asp:MenuItem>
                    </Items>

                    <StaticMenuItemStyle ForeColor="Silver" HorizontalPadding="20px" VerticalPadding="4.5px" />
                    <StaticSelectedStyle BackColor="Silver" BorderColor="Silver" BorderStyle="Solid" Font-Bold="True" HorizontalPadding="1" VerticalPadding="1" Font-Overline="False" ForeColor="Black" BorderWidth="8" Font-Size="17"></StaticSelectedStyle>
                </asp:Menu>

                <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
                <a id="lnkSignIn" runat="server" href="Login.aspx" class="btnLogin6">Sign-in</a>
                <a id="lnkSignOut" runat="server" href="Logout.aspx" class="btnLogin5">Sign-out</a>
            </nav>

            <main class="report-dashboard">
                <section class="report-filter-card">
                    <div class="report-filter-grid">
                        <div class="report-field">
                            <label for="<%= ddlReportCategory.ClientID %>">Category</label>
                            <asp:DropDownList ID="ddlReportCategory" runat="server" CssClass="report-input"></asp:DropDownList>
                        </div>

                        <div class="report-field">
                            <label for="<%= txtFromDate.ClientID %>">From date</label>
                            <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="report-input"></asp:TextBox>
                        </div>

                        <div class="report-field">
                            <label for="<%= txtToDate.ClientID %>">To date</label>
                            <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="report-input"></asp:TextBox>
                        </div>

                        <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" CssClass="report-primary-button" OnClick="btnGenerateReport_Click" />
                    </div>

                    <asp:Label ID="lblReportScope" runat="server" CssClass="report-scope"></asp:Label>
                </section>

                <section class="report-kpi-grid">
                    <article class="report-kpi-card report-kpi-blue">
                        <div class="report-kpi-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24" focusable="false">
                                <path d="M21 11.5a7.5 7.5 0 0 1-7.5 7.5H9l-5 3v-4.6A7.5 7.5 0 1 1 21 11.5Z"></path>
                                <path d="M8.5 11.5h.01M12 11.5h.01M15.5 11.5h.01"></path>
                            </svg>
                        </div>
                        <div>
                            <p>Feedback Submissions</p>
                            <strong><asp:Literal ID="litFeedbackSubmissions" runat="server" /></strong>
                            <span>Total feedback received</span>
                        </div>
                    </article>

                    <article class="report-kpi-card report-kpi-green">
                        <div class="report-kpi-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24" focusable="false">
                                <path d="M9 3h6l1 2h3a2 2 0 0 1 2 2v13a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2h3Z"></path>
                                <path d="M9 12h6M9 16h6M9 8h6"></path>
                            </svg>
                        </div>
                        <div>
                            <p>Rated Responses</p>
                            <strong><asp:Literal ID="litRatedResponses" runat="server" /></strong>
                            <span>Individual question ratings</span>
                        </div>
                    </article>

                    <article class="report-kpi-card report-kpi-gold">
                        <div class="report-kpi-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24" focusable="false">
                                <path d="m12 2.5 2.9 5.9 6.5.9-4.7 4.6 1.1 6.4-5.8-3-5.8 3 1.1-6.4-4.7-4.6 6.5-.9Z"></path>
                            </svg>
                        </div>
                        <div>
                            <p>Average Rating</p>
                            <strong><asp:Literal ID="litAverageRating" runat="server" /></strong>
                            <span>Average across rated questions</span>
                        </div>
                    </article>

                    <article class="report-kpi-card report-kpi-purple">
                        <div class="report-kpi-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24" focusable="false">
                                <circle cx="12" cy="12" r="9"></circle>
                                <path d="M9.8 9a2.2 2.2 0 1 1 3.8 1.5c-.8.7-1.6 1.2-1.6 2.5"></path>
                                <path d="M12 17h.01"></path>
                            </svg>
                        </div>
                        <div>
                            <p><asp:Literal ID="litScopeMetricLabel" runat="server" /></p>
                            <strong><asp:Literal ID="litScopeMetricValue" runat="server" /></strong>
                            <span><asp:Literal ID="litScopeMetricDescription" runat="server" /></span>
                        </div>
                    </article>
                </section>

                <section class="report-chart-grid">
                    <article class="report-panel">
                        <div class="report-panel-header">
                            <div>
                                <h2><%= DonutChartTitle %></h2>
                                <p><%= DonutChartSubtitle %></p>
                            </div>
                        </div>
                        <div class="report-chart-box">
                            <canvas id="donutChart"></canvas>
                        </div>
                    </article>

                    <article class="report-panel">
                        <div class="report-panel-header">
                            <div>
                                <h2><%= DistributionChartTitle %></h2>
                                <p><%= DistributionChartSubtitle %></p>
                            </div>
                        </div>
                        <div class="report-chart-box">
                            <canvas id="distributionChart"></canvas>
                        </div>
                    </article>
                </section>

                <section class="report-panel report-panel-wide">
                    <div class="report-panel-header">
                        <div>
                            <h2><%= PerformanceChartTitle %></h2>
                            <p><%= PerformanceChartSubtitle %></p>
                        </div>
                    </div>
                    <div class="report-chart-box">
                        <canvas id="performanceChart"></canvas>
                    </div>
                </section>

                <section class="report-panel report-panel-wide">
                    <div class="report-panel-header">
                        <div>
                            <h2><%= TrendChartTitle %></h2>
                            <p>Average rating grouped by submitted feedback date.</p>
                        </div>
                    </div>
                    <div class="report-chart-box report-trend-box">
                        <canvas id="trendChart"></canvas>
                    </div>
                </section>

                <section class="report-panel report-panel-wide">
                    <div class="report-panel-header">
                        <div>
                            <h2><%= BreakdownTitle %></h2>
                            <p>Response counts by question and rating scale.</p>
                        </div>
                    </div>
                    <div class="table-scroll">
                        <asp:GridView ID="FeedbackGridView" runat="server" AutoGenerateColumns="True" GridLines="None" CssClass="feedbackContainer modern-grid report-breakdown-table" EmptyDataText="No feedback matches the selected filters."></asp:GridView>
                    </div>
                </section>
            </main>

            <footer>
                <p id="creator">NexusEd &copy; <%= DateTime.Now.Year %></p>
            </footer>
        </div>
    </form>

    <script>
        const donutChartData = <%= DonutChartJson %>;
        const performanceChartData = <%= PerformanceChartJson %>;
        const distributionChartData = <%= DistributionChartJson %>;
        const trendChartData = <%= TrendChartJson %>;

        const noDataPlugin = {
            id: 'noDataMessage',
            afterDraw: function (chart) {
                const hasLabels = chart.data && chart.data.labels && chart.data.labels.length > 0;
                const hasValues = hasLabels && chart.data.datasets.some(function (dataset) {
                    return dataset.data && dataset.data.some(function (value) { return Number(value) > 0; });
                });

                if (hasValues) {
                    return;
                }

                const ctx = chart.ctx;
                const chartArea = chart.chartArea;
                ctx.save();
                ctx.fillStyle = '#6b7280';
                ctx.font = '600 14px Segoe UI, Arial, sans-serif';
                ctx.textAlign = 'center';
                ctx.fillText('No feedback data available for the selected filters.', (chartArea.left + chartArea.right) / 2, (chartArea.top + chartArea.bottom) / 2);
                ctx.restore();
            }
        };

        Chart.register(noDataPlugin);

        const valueLabelPlugin = {
            id: 'valueLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                ctx.save();
                ctx.fillStyle = '#0f172a';
                ctx.font = '700 10px Segoe UI, Arial, sans-serif';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';

                chart.data.datasets.forEach(function (dataset, datasetIndex) {
                    const meta = chart.getDatasetMeta(datasetIndex);
                    if (!meta || meta.hidden) {
                        return;
                    }

                    meta.data.forEach(function (element, index) {
                        const value = Number(dataset.data[index] || 0);
                        if (value <= 0) {
                            return;
                        }

                        const position = element.tooltipPosition();
                        const isDonut = chart.canvas.id === 'donutChart';
                        const isDistribution = chart.canvas.id === 'distributionChart';
                        const isTrend = chart.canvas.id === 'trendChart';
                        const label = isDistribution || isDonut ? String(value) : value.toFixed(2);

                        if (isDonut) {
                            ctx.fillText(label, position.x, position.y);
                        } else if (isTrend) {
                            ctx.fillText(label, position.x, position.y - 14);
                        } else if (chart.options.indexAxis === 'y') {
                            ctx.textAlign = 'left';
                            ctx.fillText(label, position.x + 10, position.y);
                            ctx.textAlign = 'center';
                        } else {
                            ctx.fillText(label, position.x, position.y - 10);
                        }
                    });
                });

                ctx.restore();
            }
        };

        Chart.register(valueLabelPlugin);

        const donutTotalPlugin = {
            id: 'donutTotal',
            afterDatasetsDraw: function (chart) {
                if (chart.canvas.id !== 'donutChart') {
                    return;
                }

                const dataset = chart.data.datasets && chart.data.datasets[0];
                const data = dataset && dataset.data ? dataset.data : [];
                const total = data.reduce(function (sum, value) { return sum + Number(value || 0); }, 0);
                if (total <= 0) {
                    return;
                }

                const meta = chart.getDatasetMeta(0);
                if (!meta || !meta.data || meta.data.length === 0) {
                    return;
                }

                const center = meta.data[0].getProps(['x', 'y'], true);
                const ctx = chart.ctx;
                ctx.save();
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillStyle = '#0f172a';
                ctx.font = '900 24px Segoe UI, Arial, sans-serif';
                ctx.fillText(total.toLocaleString(), center.x, center.y - 6);
                ctx.fillStyle = '#64748b';
                ctx.font = '700 11px Segoe UI, Arial, sans-serif';
                ctx.fillText('Total', center.x, center.y + 15);
                ctx.restore();
            }
        };

        Chart.register(donutTotalPlugin);

        function chartFont(size, weight) {
            return {
                family: 'Segoe UI, Arial, sans-serif',
                size: size,
                weight: weight || '600'
            };
        }

        function createDonutChart() {
            const canvas = document.getElementById('donutChart');
            if (!canvas) {
                return;
            }

            new Chart(canvas, {
                type: 'doughnut',
                data: donutChartData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '68%',
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                color: '#334155',
                                font: chartFont(11, '600'),
                                boxWidth: 14,
                                padding: 14
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const dataset = context.dataset || {};
                                    const data = dataset.data || [];
                                    const total = data.reduce(function (sum, value) { return sum + Number(value || 0); }, 0);
                                    const count = Number(context.raw || 0);
                                    const percent = total > 0 ? ((count / total) * 100).toFixed(1) : '0.0';
                                    const rawLabels = dataset.rawLabels || [];
                                    const label = rawLabels[context.dataIndex] || context.label || '';
                                    return label + ': ' + count + ' (' + percent + '%)';
                                }
                            }
                        }
                    }
                }
            });
        }

        function createPerformanceChart() {
            const canvas = document.getElementById('performanceChart');
            if (!canvas) {
                return;
            }

            new Chart(canvas, {
                type: 'bar',
                data: performanceChartData,
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            beginAtZero: true,
                            max: 5,
                            ticks: { color: '#475569', font: chartFont(11, '500') },
                            grid: { color: 'rgba(148, 163, 184, 0.22)' },
                            title: {
                                display: true,
                                text: 'Average rating out of 5',
                                color: '#334155',
                                font: chartFont(11, '700')
                            }
                        },
                        y: {
                            ticks: { color: '#111827', font: chartFont(11, '600') },
                            grid: { display: false }
                        }
                    },
                    plugins: {
                        legend: { display: false },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    return 'Average: ' + Number(context.raw || 0).toFixed(2);
                                }
                            }
                        }
                    }
                }
            });
        }

        function createDistributionChart() {
            const canvas = document.getElementById('distributionChart');
            if (!canvas) {
                return;
            }

            new Chart(canvas, {
                type: 'bar',
                data: distributionChartData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            ticks: { color: '#334155', font: chartFont(10, '600') },
                            grid: { display: false },
                            title: {
                                display: false,
                                text: '',
                                color: '#334155',
                                font: chartFont(11, '700')
                            }
                        },
                        y: {
                            beginAtZero: true,
                            ticks: { precision: 0, color: '#475569', font: chartFont(11, '500') },
                            grid: { color: 'rgba(148, 163, 184, 0.22)' },
                            title: {
                                display: true,
                                text: 'Number of responses',
                                color: '#334155',
                                font: chartFont(11, '700')
                            }
                        }
                    },
                    plugins: {
                        legend: { display: false },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    return context.raw + ' responses';
                                }
                            }
                        }
                    }
                }
            });
        }

        function createTrendChart() {
            const canvas = document.getElementById('trendChart');
            if (!canvas) {
                return;
            }

            new Chart(canvas, {
                type: 'line',
                data: trendChartData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    elements: {
                        line: { tension: 0.28 },
                        point: { radius: 4, hoverRadius: 5 }
                    },
                    scales: {
                        x: {
                            ticks: { color: '#334155', font: chartFont(11, '500'), autoSkip: true, maxTicksLimit: 8, maxRotation: 0 },
                            grid: { display: false },
                            title: {
                                display: true,
                                text: 'Date range',
                                color: '#334155',
                                font: chartFont(11, '700')
                            }
                        },
                        y: {
                            beginAtZero: false,
                            min: 1,
                            max: 5,
                            ticks: { color: '#475569', font: chartFont(11, '500') },
                            grid: { color: 'rgba(148, 163, 184, 0.22)' },
                            title: {
                                display: true,
                                text: 'Average rating',
                                color: '#334155',
                                font: chartFont(11, '700')
                            }
                        }
                    },
                    plugins: {
                        legend: { display: false },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    return 'Average: ' + Number(context.raw || 0).toFixed(2);
                                }
                            }
                        }
                    }
                }
            });
        }

        window.addEventListener('load', function () {
            createDonutChart();
            createPerformanceChart();
            createDistributionChart();
            createTrendChart();
        });
    </script>
</body>
</html>
