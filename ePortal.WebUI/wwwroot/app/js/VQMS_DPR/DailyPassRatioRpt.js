$(document).ready(function () {
    //$("#ddlFactory").val($("#hdnFactory").val());
    GetChartsData();
    $("#btnSearch").on("click", function (e) {
        e.preventDefault();
        if ($("#txtDate").val() == "") {
            alert("Please select Date")
            return;
        }
        GetChartsData();
    });
    $("#txtDate").datepicker({
        format: "dd-M-yyyy",
        autoclose: true
    }).datepicker("setDate", new Date());
    $("#S1").on("click", function (e) {
        e.preventDefault();
        $("#txtDate").datepicker("show");
    });
    $("#R1").on("click", function (e) {
        e.preventDefault();
        $("#txtDate").val("");
    });
});

let barChart, pieChart, sectionChart, barChart2, pieChart2, sectionChart2, barChart3, pieChart3, sectionChart3, barChart4, pieChart4, sectionChart4;

function GetChartsData() {
    const factoryText = $("#ddlFactory option:selected").text();

    const data = {
        ddlFactory: $("#ddlFactory").val(),
        txtDate: $("#txtDate").val(),
        ddlShift: $("#ddlShift").val(),
    };

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/VQMS_DPR/SearchDailyPassRatioRpt",
        data: data,
        success: function (vm) {
            //console.log(vm);
            line34Showhide(vm);
            [
                barChart, pieChart, sectionChart, barChart2, pieChart2, sectionChart2,
                barChart3, pieChart3, sectionChart3, barChart4, pieChart4, sectionChart4
            ].forEach(c => { if (c) c.destroy(); });

            // Render Line 01 charts
            barChart = renderBarChart("DPR_BAR1", vm.DPRCnt01, "Pass Ratio (Nos) - " + factoryText);
            pieChart = renderPieChart("DPR_PERCENTAGE1", vm.DPRPer01, "Pass Ratio % - " + factoryText);
            sectionChart = renderSectionChart("VQ_Chart1", vm.Sectionwise01, "Sectionwise Defects - " + factoryText);
            // Render Line 02 charts
            barChart2 = renderBarChart("DPR_BAR2", vm.DPRCnt02, "Pass Ratio (Nos) - " + factoryText);
            pieChart2 = renderPieChart("DPR_PERCENTAGE2", vm.DPRPer02, "Pass Ratio % - " + factoryText);
            sectionChart2 = renderSectionChart("VQ_Chart2", vm.Sectionwise02, "Sectionwise Defects - " + factoryText);
            // Render Line 03 charts
            barChart3 = renderBarChart("DPR_BAR3", vm.DPRCnt03, "Pass Ratio (Nos) - " + factoryText);
            pieChart3 = renderPieChart("DPR_PERCENTAGE3", vm.DPRPer03, "Pass Ratio % - " + factoryText);
            sectionChart3 = renderSectionChart("VQ_Chart3", vm.Sectionwise03, "Sectionwise Defects - " + factoryText);
            // Render Line 04 charts
            barChart4 = renderBarChart("DPR_BAR4", vm.DPRCnt04, "Pass Ratio (Nos) - " + factoryText);
            pieChart4 = renderPieChart("DPR_PERCENTAGE4", vm.DPRPer04, "Pass Ratio % - " + factoryText);
            sectionChart4 = renderSectionChart("VQ_Chart4", vm.Sectionwise04, "Sectionwise Defects - " + factoryText);
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function (xhr) {
            sweetAlert("Oops...", "Something went wrong! " + xhr.responseText, "error");
        }
    });
}

// Bar Chart (Pass Ratio Nos)
function renderBarChart(canvasId, chartData, title) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    const labels = chartData.map(x => x.PASS_TYPE);
    const data = chartData.map(x => x.CNT);

    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: title,
                data,
                backgroundColor: [
                    '#9999ff',
                    '#66ccff',
                    '#ffcc99'
                ],
                borderColor: 'rgba(26,59,105,1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: false,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: title,
                    color: '#1A3B69',
                    font: { family: 'Trebuchet MS', size: 11, weight: 'bold' }
                },
                legend: { display: false },
                datalabels: {
                    color: '#150517',
                    font: { family: 'Trebuchet MS', size: 7 },
                    anchor: 'end',
                    align: 'top',
                    formatter: (value) => value
                }
            },
            scales: {
                x: { ticks: { font: { family: 'Trebuchet MS', size: 9 } }, grid: { display: false } },
                y: { ticks: { font: { family: 'Trebuchet MS', size: 9 } }, grid: { display: false } }
            }
        },
        plugins: [ChartDataLabels]
    });
}

// Pie Chart (Pass Ratio %)
function renderPieChart(canvasId, chartData, title) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    const labels = chartData.map(x => x.PASS_TYPE);
    const data = chartData.map(x => x.PER);

    return new Chart(ctx, {
        type: 'pie',
        data: {
            labels,
            datasets: [{
                data,
                backgroundColor: ['#9999ff', '#66ccff', '#ffcc99', '#99ff99', '#ff9999'],
                borderColor: 'rgba(26,59,105,1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: false,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: title,
                    color: '#1A3B69',
                    font: { family: 'Trebuchet MS', size: 11, weight: 'bold' }
                },
                legend: {
                    display: true,
                    position: 'right',
                    labels: { font: { family: 'Trebuchet MS', size: 7, weight: 'bold' } }
                },
                datalabels: {
                    color: '#150517',
                    font: { family: 'Trebuchet MS', size: 9 },
                    formatter: (value) => value + '%'
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}

// Sectionwise Defects (Bar + Line Pareto)
function renderSectionChart(canvasId, chartData, title) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    const labels = chartData.map(x => x.DEFECTSECTION);
    const counts = chartData.map(x => x.CNT);
    const cumulative = chartData.map(x => x.CM);

    // 👇 Plugin to redraw line datasets after bars
    const bringLineToFront = {
        id: 'bringLineToFront',
        afterDatasetsDraw(chart) {
            const { ctx } = chart;
            chart.getSortedVisibleDatasetMetas()
                .filter(meta => meta.type === 'line')
                .forEach(meta => {
                    meta.controller.draw(ctx);
                });
        }
    };

    return new Chart(ctx, {
        data: {
            labels,
            datasets: [
                {
                    type: 'bar',
                    label: 'Defect Count',
                    data: counts,
                    backgroundColor: 'gainsboro',
                    borderColor: 'rgba(26,59,105,1)',
                    borderWidth: 1,
                    datalabels: {
                        color: '#150517',
                        font: { family: 'Trebuchet MS', size: 7 },
                        anchor: 'end',
                        align: 'top'
                    }
                },
                {
                    type: 'line',
                    label: 'Pareto %',
                    data: cumulative,
                    borderColor: 'rgba(252,180,65,1)',
                    backgroundColor: 'rgba(252,180,65,0.5)',
                    borderWidth: 1,
                    yAxisID: 'y1',
                    tension: 0.3,
                    pointStyle: 'diamond',
                    pointRadius: 5,
                    pointBackgroundColor: 'red',
                    pointBorderColor: 'midnightblue',
                    datalabels: {
                        color: '#150517',
                        font: { family: 'Trebuchet MS', size: 7 },
                        align: 'top',
                        formatter: (value) => value.toFixed(1) + '%'
                    }
                }
            ]
        },
        options: {
            responsive: false,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: title,
                    color: '#1A3B69',
                    font: { family: 'Trebuchet MS', size: 11, weight: 'bold' }
                },
                legend: { display: false }
            },
            scales: {
                y: { stacked: false, ticks: { font: { family: 'Trebuchet MS', size: 9 } }, grid: { display: false } },
                y1: { stacked: false, max: 100, position: 'right', ticks: { font: { family: 'Trebuchet MS', size: 9 } }, grid: { drawOnChartArea: false } },
                x: { stacked: false, ticks: { font: { family: 'Trebuchet MS', size: 9 } }, grid: { display: false } }
            }
        },
        plugins: [ChartDataLabels, bringLineToFront]
    });
}


function line34Showhide(vm) {
    if (vm.IsChar03Visible) {
        $("#TrHeading03,#TrChart03").show();
    } else {
        $("#TrHeading03,#TrChart03").hide();
    }
    if (vm.IsChar04Visible) {
        $("#TrHeading04,#TrChart04").show();
    } else {
        $("#TrHeading04,#TrChart04").hide();
    }
}