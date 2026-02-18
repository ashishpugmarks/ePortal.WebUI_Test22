// Created by TTL on 17-July-2025 against SR93758 > CR5975
var arrayList = new Array, attachmentArray = new Array, tmpHeaderArray = new Array, mainHeaderArray = new Array, TotalRowCount = 0, IsUploadAttachment = false, desig = "";
$('.homeContent').removeClass('homeContent');
let myChart = null;
let myChartV2 = null;
let myChartV3 = null;
let associateDepartmentLabels;
let associateReqStatus;

$(document).ready(function () {

    $('#collapseSearch').on('hidden.bs.collapse', function () {
        $('#spanIconSearch').removeClass('fa-chevron-down').addClass('fa-chevron-right');
    });
    $('#collapsechartDiv').on('hidden.bs.collapse', function () {
        $('#spanIconChart1').removeClass('fa-chevron-down').addClass('fa-chevron-right');
    });
    $('#collapsechartDiv2').on('hidden.bs.collapse', function () {
        $('#spanIconChart2').removeClass('fa-chevron-down').addClass('fa-chevron-right');
    });
    $('#collapsechartDiv3').on('hidden.bs.collapse', function () {
        $('#spanIconChart3').removeClass('fa-chevron-down').addClass('fa-chevron-right');
    });

    $('#collapseSearch').on('shown.bs.collapse', function () {
        $('#spanIconSearch').addClass('fa-chevron-down').removeClass('fa-chevron-right');
    });
    $('#collapsechartDiv').on('shown.bs.collapse', function () {
        $('#spanIconChart1').addClass('fa-chevron-down').removeClass('fa-chevron-right');
    });
    $('#collapsechartDiv2').on('shown.bs.collapse', function () {
        $('#spanIconChart2').addClass('fa-chevron-down').removeClass('fa-chevron-right');
    });
    $('#collapsechartDiv3').on('shown.bs.collapse', function () {
        $('#spanIconChart3').addClass('fa-chevron-down').removeClass('fa-chevron-right');
    });
});



const getOrCreateLegendList = (chart, id) => {
    const legendContainer = document.getElementById(id);
    let listContainer = legendContainer.querySelector('ul');

    if (!listContainer) {
        listContainer = document.createElement('ul');
        listContainer.style.display = 'flex';
        listContainer.style.flexDirection = 'row';
        listContainer.style.justifyContent = 'center';
        listContainer.style.margin = 0;
        listContainer.style.padding = 0;

        legendContainer.appendChild(listContainer);
    }

    return listContainer;
};

function RedirectToList(rangeLbl, departmentLbl, statusLbl) {

    const rangeMapJson = $("#h_pageAction").val();
    const rangeMap = JSON.parse(rangeMapJson);
    const rangeId = Object.entries(rangeMap).find(([key, value]) => value === rangeLbl)?.[0];

    departmentLbl = departmentLbl.replace(/\[\d+\]$/, '').trim();
    statusLbl = statusLbl.replace(/\[\d+\]$/, '').trim();

    const departmentLabelId = associateDepartmentLabels.find(d => d.DepartmentLabel === departmentLbl)?.id;
    const statusLabelId = associateReqStatus.find(d => d.desc === statusLbl)?.id;

    //  alert(departmentLabelId + " | " + statusLabelId + " | " + rangeId );

    let hasOpListData = 0;
    let hasDivisionListData = 0;
    let hasDeptListData = 0;
    let hasSecListData = 0;

    if ($("#ddlDiv option").length === 1) {
        hasDivisionListData = -1;
    }
    else if ($("#ddlDep option").length === 1) {
        hasDeptListData = -1;
    }
    else if ($("#ddlSec option").length === 1) {
        hasSecListData = -1;
    }

    const data = {
        OperationID: $('#ddlOp').val() == "" ? departmentLabelId : $('#ddlOp').val(),
        DivisionID: $('#ddlDiv').val() == "" ? 0 : $('#ddlDiv').val(),
        DEPTID: $('#ddlDep').val() == "" ? 0 : $('#ddlDep').val(),
        SECID: $('#ddlSec').val() == "" ? 0 : $('#ddlSec').val(),
        Status: statusLabelId,
        IOMCATMSTID: $('#ddCategory').val() == "" ? 0 : $('#ddCategory').val(),
        KIID: $('#ddlKI').val(),
        HasDivisionListData: hasDivisionListData,
        HasDeptListData: hasDeptListData,
        HasSecListData: hasSecListData,
    }

    let urlParams = "opid=" + data.OperationID + "&divid=" + data.DivisionID + "&deptid=" + data.DEPTID + "&secid=" + data.SECID + "&status=" + data.Status + "&IOMCATMSTID=" + data.IOMCATMSTID + "&KIID=" + data.KIID + "&rangeId=" + rangeId + "&deptLavelId=" + departmentLabelId + "&hasDiv=" + data.HasDivisionListData + "&hasDept=" + data.HasDeptListData + "&hasSec=" + data.HasSecListData;
    urlParams = "/IOM/IOMDashboadReportList?" + urlParams;
    window.open(urlParams, '_blank');
}

function GetGroupedDesignationChart(rangeLbl, departmentLbl, statusLbl) {

    const rangeMapJson = $("#h_pageAction").val();
    const rangeMap = JSON.parse(rangeMapJson);

    const rangeId = Object.entries(rangeMap).find(([key, value]) => value === rangeLbl)?.[0];

    departmentLbl = departmentLbl.replace(/\[\d+\]$/, '').trim();
    statusLbl = statusLbl.replace(/\[\d+\]$/, '').trim();

    const departmentLabelId = associateDepartmentLabels.find(d => d.DepartmentLabel === departmentLbl)?.id;
    const statusLabelId = associateReqStatus.find(d => d.desc === statusLbl)?.id;

    //  alert(departmentLabelId + " | " + statusLabelId + " | " + rangeId );

    let hasOpListData = 0;
    let hasDivisionListData = 0;
    let hasDeptListData = 0;
    let hasSecListData = 0;

    if ($("#ddlDiv option").length === 1) {
        hasDivisionListData = -1;
    }
    else if ($("#ddlDep option").length === 1) {
        hasDeptListData = -1;
    }
    else if ($("#ddlSec option").length === 1) {
        hasSecListData = -1;
    }

    const data = {
        OperationID: $('#ddlOp').val() == "" ? departmentLabelId : $('#ddlOp').val(),
        DivisionID: $('#ddlDiv').val() == "" ? 0 : $('#ddlDiv').val(),
        DEPTID: $('#ddlDep').val() == "" ? 0 : $('#ddlDep').val(),
        SECID: $('#ddlSec').val() == "" ? 0 : $('#ddlSec').val(),
        Status: statusLabelId,
        IOMCATMSTID: $('#ddCategory').val() == "" ? 0 : $('#ddCategory').val(),
        KIID: $('#ddlKI').val(),
        HasDivisionListData: hasDivisionListData,
        HasDeptListData: hasDeptListData,
        HasSecListData: hasSecListData,
        /*Added by TTL on 29-July-2025 against SR104160 > CR6821 - Start*/
        PendingAtUsers: $('#ddlPendingAt').val(),
        PendingWithType: $('input[name="PendingWithType"]:checked').val(),
        rangeId: rangeId,
        deptLavelId: departmentLabelId
        /*Added by TTL on 29-July-2025 against SR104160 > CR6821 - End*/
    }

    let url_Params = "opid=" + data.OperationID + "&divid=" + data.DivisionID + "&deptid=" + data.DEPTID + "&secid=" + data.SECID + "&status=" + data.Status + "&IOMCATMSTID=" + data.IOMCATMSTID + "&KIID=" + data.KIID + "&rangeId=" + rangeId + "&deptLavelId=" + departmentLabelId + "&hasDiv=" + data.HasDivisionListData + "&hasDept=" + data.HasDeptListData + "&hasSec=" + data.HasSecListData + "&PendingAtUsers=" + data.PendingAtUsers + "&PendingWithType=" + data.PendingWithType;
    urlParams = "/IOM/IOMDashboadReportListByDesignationGroup?" + url_Params;
    $('#spanGroupCaption').html('');
    $('#spanGroupCaption2').html('');
    $.ajax({
        type: "POST",
        beforeSend: function () {
            //$("#ajaxLoader").addClass('loader');
            showEstimatedTime();
        },
        url: '/IOM/IOMDashboadReportListByDesignationGroup',
        //data: JSON.stringify(data),
        data: data,
        //contentType: "application/json; charset=utf-8",
        contentType: "application/ x-www-form-urlencoded; charset=UTF-8",
        datatype: "json",
        success: function (d) {
            if (statusLabelId == 2) {
                RenderGroupedDesignationChartForCompleted(d.AggregatedSearchResult, url_Params, data);
                $('#spanGroupCaption2').html(`${departmentLbl} > ${statusLbl}, Range - ${rangeLbl}`);
            }
            else {
                RenderGroupedDesignationChart(d.AggregatedSearchResult, url_Params, data);
                $('#spanGroupCaption').html(`${departmentLbl} > ${statusLbl}, Range - ${rangeLbl}`);
            }
        },
        complete: function () {
            //$("#ajaxLoader").removeClass('loader');
            hideEstimatedTime();
        },
        error: function () {
            hideEstimatedTime();
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}

function RenderGroupedDesignationChart(data, urlParams, requestPayload) {
    toggleChartDiv3(false);
    toggleChartDiv2(true);
    loadGraphV2(data, urlParams, requestPayload);
    $('#collapsechartDiv').collapse('toggle');
}

function RenderGroupedDesignationChartForCompleted(data, urlParams, requestPayload) {
    toggleChartDiv2(false);
    toggleChartDiv3(true);
    loadGraphV3(data, urlParams, requestPayload);
    $('#collapsechartDiv').collapse('toggle');
}

function toggleChartDiv2(isOn) {
    var chartDiv2 = document.getElementById('chartDiv2');
    if (chartDiv2) {
        if (isOn === true) {
            chartDiv2.style.visibility = 'visible';
            chartDiv2.style.display = 'block';
        }
        else {
            chartDiv2.style.visibility = 'hidden';
            chartDiv2.style.display = 'none';
        }
    }
}
function toggleChartDiv3(isOn) {
    var chartDiv3 = document.getElementById('chartDiv3');
    if (chartDiv3) {
        if (isOn === true) {
            chartDiv3.style.visibility = 'visible';
            chartDiv3.style.display = 'block';
        }
        else {
            chartDiv3.style.visibility = 'hidden';
            chartDiv3.style.display = 'none';
        }
    }
}

function transformToDesignationChartData(rawData) {
    const colorPlate = ['#3cb44b', '#0082c8', '#e6194b', '#f58231', '#911eb4', '#46f0f0', '#ffe119', '#f032e6', '#d2f53c', '#fabebe', '#008080', '#e6beff', '#aa6e28', '#fffac8', '#800000', '#aaffc3', '#808000', '#ffd8b1', '#000080', '#808080'];


    const labels = rawData.map(item => item.Designation);
    const data = rawData.map(item => item.CountOfIOMs);
    const backgroundColors = rawData.map((data, idx) => colorPlate[idx]);

    return {
        labels,
        datasets: [
            {
                label: "",
                backgroundColor: backgroundColors,
                data: data,
                borderWidth: 1,
                maxBarThickness: 50
            }
        ]
    };
}

function transformToChartDataset(jsonDataSet) {
    const flatStatusList = [];

    associateDepartmentLabels = jsonDataSet.map(x => x.Group);
    associateReqStatus = jsonDataSet
        .map(group => group.Data.map(status => ({
            id: status.Status.id,
            desc: status.Status.desc
        })))
        .flat()
        .filter((value, index, self) =>
            index === self.findIndex(obj =>
                obj.id === value.id && obj.desc === value.desc
            )
        );

    const groupData = jsonDataSet.map(groupItem => {
        const subGroups = groupItem.Data.map(status => status.Status.desc);

        // Push flattened status records with context for dataset creation
        groupItem.Data.forEach(status => {
            flatStatusList.push({
                group: groupItem.Group.DepartmentLabel,
                label: status.Status.desc,
                records: status.RecordsByRange
            });
        });

        return {
            group: groupItem.Group.DepartmentLabel,
            subGroups
        };
    });

    const uniqueRanges = [...new Set(flatStatusList.flatMap(status =>
        status.records.map(record => record.Range)
    ))];

    // Create datasets by range
    const colorPalette = ['#28a745', '#007bff', '#fd7e14', '#e62e00'];
    const datasets = uniqueRanges.map((range, index) => {
        return {
            label: range,
            data: flatStatusList.map(statusEntry => {
                const match = statusEntry.records.find(r => r.Range === range);
                var recordCount = match ? match.RecordCount : 0;
                return {
                    x: statusEntry.label,
                    y: recordCount,
                    groupName: statusEntry.group
                };
            }),
            backgroundColor: colorPalette[index % colorPalette.length],
            borderWidth: 1,
            maxBarThickness: 50
        };
    });

    return {
        labels: groupData,
        datasets: datasets
    };
}

function transformToChartDatasetV3(jsonDataSet) {
    // Extract all unique ranges
    const uniqueRanges = [...new Set(jsonDataSet.flatMap(d => d.RecordsByRange.map(r => r.Range)))];;
    // Extract X labels (designations)
    const designations = jsonDataSet.map(d => d.Designation);

    // Create datasets by range
    const colorPalette = ['#28a745', '#fd7e14', '#e62e00'];
    const datasets = uniqueRanges.map((range, index) => {
        return {
            label: range,
            data: jsonDataSet.map(d => {
                const match = d.RecordsByRange.find(r => r.Range === range);
                return match ? match.RecordCount : 0;
            }),
            backgroundColor: uniqueRanges.length == 3 ? colorPalette[index % colorPalette.length] : colorPalette[2], //uniqueRange one denotes only >15, so the red color would be the background color
            borderWidth: 1,
            maxBarThickness: 50
        };
    });

    return {
        labels: designations,
        datasets: datasets
    };
}

function GetReportList(crnt) {

    let hasDivisionListData = 0;
    let hasDeptListData = 0;
    let hasSecListData = 0;


    if ($("#ddlDiv option").length === 1) {
        hasDivisionListData = -1;
    }
    else if ($("#ddlDep option").length === 1) {
        hasDeptListData = -1;
    }
    else if ($("#ddlSec option").length === 1) {
        hasSecListData = -1;
    }

    var data = {
        OperationID: $('#ddlOp').val() == "" ? 0 : $('#ddlOp').val(),
        DivisionID: $('#ddlDiv').val() == "" ? 0 : $('#ddlDiv').val(),
        DEPTID: $('#ddlDep').val() == "" ? 0 : $('#ddlDep').val(),
        SECID: $('#ddlSec').val() == "" ? 0 : $('#ddlSec').val(),
        FilterStatus: $('#ddlStatus').val(),
        IOMCATMSTID: $('#ddCategory').val(),
        KIID: $('#ddlKI').val(),
        HasDivisionListData: hasDivisionListData,
        HasDeptListData: hasDeptListData,
        HasSecListData: hasSecListData,
        /*Added by TTL on 29-July-2025 against SR104160 > CR6821 - Start*/
        PendingAtUsers: $('#ddlPendingAt').val(),
        PendingWithType: $('input[name="PendingWithType"]:checked').val()
        /*Added by TTL on 29-July-2025 against SR104160 > CR6821 - End*/
    }
    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
            showEstimatedTime();
        },
        url: "/IOM/IOMDashboard",
        //data: JSON.stringify(data),
        data: data,
        //contentType: "application/json; charset=utf-8",
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        datatype: "json",
        success: function (data) {
            var result = data.result;
            loadGraph(result);
            $('#divBenchmark').fadeIn();
            $('#benchmark').html(data.benchmark);
        },
        complete: function () {
            //$("#ajaxLoader").removeClass('loader');
            hideEstimatedTime();
        },
        error: function () {
            hideEstimatedTime();
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}

function getFinancialMonthNumber(momentDate) {
    const calendarMonth = momentDate.month(); // 0 = Jan, 11 = Dec
    // April = month 3 → financial month 1
    if (calendarMonth >= 3) {
        return calendarMonth - 2; // April (3) becomes 1
    } else {
        return calendarMonth + 10; // Jan (0) becomes 11
    }
}
function showEstimatedTime() {
    const now = moment();
    const financialMonth = getFinancialMonthNumber(now);
    var time = 22;
    if ($('#ddlKI > option:selected')[0].index == 0) {
        time = Math.floor(21 * financialMonth / 12);
    }
    $('#spanTime').html(time);
    $("#ajaxLoader").addClass('loader');
    $('#divGraph1Time').fadeIn();
}
function hideEstimatedTime() {
    $('#spanTime').html('0');
    $("#ajaxLoader").removeClass('loader');
    $('#divGraph1Time').fadeOut();
}

function loadGraph(dataSet) {
    document.getElementById('chartDiv').style.visibility = 'visible';
    var chartDataSet = transformToChartDataset(dataSet);

    const groupData = chartDataSet.labels;
    const xLabels = [];
    const groupBoundaries = [];
    groupData.forEach(g => {
        const start = xLabels.length;
        g.subGroups.forEach(sg => xLabels.push(sg));
        const end = xLabels.length - 1;
        groupBoundaries.push({ group: g.group, start, end });
    });
    const data = {
        labels: xLabels,
        datasets: chartDataSet.datasets,
    };
    const dynamicGroupPlugin = {
        id: 'dynamicGroups',
        afterDraw(chart) {
            const {
                ctx,
                chartArea: { bottom, top },
                scales: { x }
            } = chart;

            ctx.save();
            ctx.font = 'bold 12px sans-serif';
            ctx.textAlign = 'center';

            const maxLabelWidth = 100;
            const lineHeight = 16;
            const groupLabelYOffset = 80; // further down from x-axis ticks

            groupBoundaries.forEach((group, i) => {
                const xStart = x.getPixelForValue(group.start);
                const xEnd = x.getPixelForValue(group.end);
                const center = (xStart + xEnd) / 2;

                const indices = [];
                for (let i = group.start; i <= group.end; i++) {
                    indices.push(i);
                }
                const total = chart.data.datasets.reduce((sum, ds, i) => {
                    const meta = chart.getDatasetMeta(i);
                    if (meta.hidden) return sum;

                    indices.forEach(index => {
                        const point = ds.data[index];
                        const val = typeof point === 'object' ? point?.y ?? 0 : point ?? 0;
                        sum += val;
                    });

                    return sum;
                }, 0);
                const labelText = group.group;
                const countText = `[${total}]`;
                const label = `${labelText} ${countText}`;

                const words = label.split(' ');
                let lines = [];
                let currentLine = words[0];

                for (let j = 1; j < words.length; j++) {
                    const testLine = currentLine + ' ' + words[j];
                    const testWidth = ctx.measureText(testLine).width;
                    if (testWidth < maxLabelWidth) {
                        currentLine = testLine;
                    } else {
                        lines.push(currentLine);
                        currentLine = words[j];
                    }
                }
                lines.push(currentLine);

                lines.forEach((line, idx) => {
                    ctx.fillText(line, center, bottom + groupLabelYOffset + (idx * lineHeight));
                });

                if (i < groupBoundaries.length - 1) {
                    const sepX = x.getPixelForValue(group.end + 1) -
                        ((x.getPixelForValue(group.end + 1) - x.getPixelForValue(group.end)) / 2);
                    ctx.beginPath();
                    ctx.moveTo(sepX, top);
                    ctx.lineTo(sepX, bottom + 5);
                    ctx.setLineDash([4, 3]);
                    ctx.strokeStyle = '#aaa';
                    ctx.stroke();
                }
            });

            ctx.restore();
        }
    };

    const htmlLegendPlugin = {
        id: 'htmlLegend',
        afterUpdate(chart, args, options) {
            const ul = getOrCreateLegendList(chart, options.containerID);

            // Remove old legend items
            while (ul.firstChild) {
                ul.firstChild.remove();
            }

            // Reuse the built-in legendItems generator
            const items = chart.options.plugins.legend.labels.generateLabels(chart);

            items.forEach(item => {
                const li = document.createElement('li');
                li.style.alignItems = 'center';
                li.style.cursor = 'pointer';
                li.style.display = 'flex';
                li.style.flexDirection = 'row';
                li.style.marginLeft = '10px';
                li.style.font = 'bold 12px sans-serif';

                li.onclick = () => {
                    const { type } = chart.config;
                    if (type === 'pie' || type === 'doughnut') {
                        // Pie and doughnut charts only have a single dataset and visibility is per item
                        chart.toggleDataVisibility(item.index);
                    } else {
                        chart.setDatasetVisibility(item.datasetIndex, !chart.isDatasetVisible(item.datasetIndex));
                    }
                    chart.update();
                };

                // Color box
                const boxSpan = document.createElement('span');
                boxSpan.style.background = item.fillStyle;
                boxSpan.style.borderColor = item.strokeStyle;
                boxSpan.style.borderWidth = item.lineWidth + 'px';
                boxSpan.style.display = 'inline-block';
                boxSpan.style.flexShrink = 0;
                boxSpan.style.height = '20px';
                boxSpan.style.marginRight = '10px';
                boxSpan.style.width = '20px';

                // Text
                const textContainer = document.createElement('p');
                textContainer.style.color = item.fontColor;
                textContainer.style.margin = 0;
                textContainer.style.padding = 0;
                textContainer.style.textDecoration = item.hidden ? 'line-through' : '';

                const text = document.createTextNode(item.text);
                textContainer.appendChild(text);

                li.appendChild(boxSpan);
                li.appendChild(textContainer);
                ul.appendChild(li);
            });
        }
    };

    const config = {
        type: 'bar',
        data: data,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'nearest',
                intersect: false
            },
            onHover: function (event, elements) {
                const canvas = event.chart.canvas;
                if (elements.length > 0 && elements[0].element?.$context?.raw != null) {
                    canvas.style.cursor = 'pointer';
                } else {
                    canvas.style.cursor = 'default';
                }
            },
            layout: {
                padding: {
                    bottom: 100,
                    top: 30
                }
            },
            onClick: (evt, elements) => {
                if (elements.length > 0) {
                    const chart = elements[0].element.$context.chart;
                    const datasetIndex = elements[0].datasetIndex;
                    const index = elements[0].index;

                    const clickedData = chart.data.datasets[datasetIndex].data[index];

                    const rangeLabel = chart.data.datasets[datasetIndex].label;
                    const departmentLabel = clickedData.groupName;
                    const statusLabel = clickedData.x;
                    GetGroupedDesignationChart(rangeLabel, departmentLabel, statusLabel);
                    //RedirectToList(rangeLabel, departmentLabel, statusLabel);
                }
            },
            plugins: {
                /*legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        padding: 10
                    }
                },*/
                legend: {
                    display: false,
                },
                htmlLegend: {
                    // ID of the container to put the legend in
                    containerID: 'chart1-legend-container',
                },
                tooltip: {
                    enabled: false
                },
                datalabels: {
                    display: function (context) {
                        return context.dataset.data[context.dataIndex].y !== 0; //xLabels
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: -1,
                    clip: false,
                    color: '#000',
                    backgroundColor: 'transparent',
                    borderRadius: 4,
                    font: function (context) {
                        const bar = context.chart.getDatasetMeta(context.datasetIndex).data[context.dataIndex];
                        const width = bar.width;

                        let size = width > 60 ? 16 : width > 30 ? 13 : 10;
                        size = Math.max(size, 8);

                        return {
                            weight: 'bold',
                            size: size
                        };
                    },
                    padding: {
                        top: 1,
                        bottom: 1,
                        left: 4,
                        right: 4
                    },
                    formatter: function (value, context) {
                        //console.log(context.dataset.label);
                        return value.y !== 0 ? `${value.y}` : '';
                    }
                },
            },
            scales: {
                x: {
                    ticks: {
                        callback: function (value, index, ticks) {
                            const chart = this.chart;
                            const datasets = chart.data.datasets;

                            const total = datasets.reduce((sum, ds, i) => {
                                const meta = chart.getDatasetMeta(i);
                                if (meta.hidden) return sum;

                                const point = ds.data[index];
                                return sum + (typeof point === 'object' ? point.y || 0 : point || 0);
                            }, 0);

                            const label = chart.data.labels[index];
                            return `${label} [${total}]`;
                        },
                        font: {
                            family: 'sans-serif',    // Font family
                            size: 12,           // Font size in px
                            weight: 'bold',     // 'normal', 'bold', 'lighter', etc.
                            //style: 'italic'     // optional: 'normal', 'italic'
                        },
                        color: '#00728C',
                        padding: 2
                    },
                    grid: {
                        display: false
                    },
                    offset: true
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Records',
                        color: '#00728C'
                    },
                    ticks: {
                        color: '#00728C',
                        stepSize: 1
                    },
                    afterDataLimits: function (scale) {
                        const suggestedMax = scale.max;
                        scale.max = suggestedMax;
                    },
                    grid: {
                        color: '#DCDCDC',
                        drawOnChartArea: true,
                        drawTicks: true,
                        drawBorder: true
                    }
                }
            }
        },
        plugins: [dynamicGroupPlugin, ChartDataLabels, htmlLegendPlugin]
    };

    if (myChart) {
        myChart.destroy();
    }
    const ctx = document.getElementById('myChart').getContext('2d');
    myChart = new Chart(ctx, config);
}
function loadGraphV2(dataSet, urlParams_in, requestPayload) {

    var chartDataSet = transformToDesignationChartData(dataSet);
    const maxCount = Math.max(...dataSet.map(item => item.CountOfIOMs));

    const chartConfig = {
        type: 'bar',
        data: chartDataSet,
        options: {
            parsing: true,
            responsive: true,
            interaction: {
                mode: 'nearest',
                intersect: false
            },
            onHover: function (event, elements) {
                const canvas = event.chart.canvas;
                if (elements.length > 0 && elements[0].element?.$context?.raw != null) {
                    canvas.style.cursor = 'pointer';
                } else {
                    canvas.style.cursor = 'default';
                }
            },
            onClick: (event, elements, chart) => {
                if (elements.length > 0) {
                    const { datasetIndex, index } = elements[0];
                    const groupName = chart.data.labels[index];
                    const encodedGroupName = encodeURIComponent(groupName);
                    const fid = crypto.randomUUID();
                    requestPayload.designation = groupName;
                    sessionStorage.setItem(fid, JSON.stringify(requestPayload));
                    var urlParams = `/IOM/IOMDashboadReportList?fid=${fid}`;
                    window.open(urlParams, '_blank');

                }
            },
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false,
                },
                tooltip: {
                    enabled: false
                },
                title: {
                    display: true,
                    text: 'Count of Pending Approval Notes by Different Organisation Levels'
                },
                datalabels: {
                    display: function (context) {
                        return context.dataset.data[context.dataIndex] !== 0;
                    },
                    anchor: 'end',
                    align: 'end',
                    clip: false,
                    color: '#000',
                    backgroundColor: 'transparent',
                    borderRadius: 4,
                    font: function (context) {
                        const bar = context.chart.getDatasetMeta(context.datasetIndex).data[context.dataIndex];
                        const width = bar.width;

                        let size = width > 60 ? 16 : width > 30 ? 13 : 10;
                        size = Math.max(size, 8);

                        return {
                            weight: 'bold',
                            size: size
                        };
                    },
                    padding: {
                        top: 1,
                        bottom: 1,
                        left: 4,
                        right: 4
                    },
                    formatter: function (value) {
                        return value !== 0 ? value : '';
                    }
                },
            },
            scales: {
                x: {
                    stacked: false,
                    title: {
                        display: false,
                        text: ''
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Records',
                        color: '#00728C'
                    },
                    ticks: {
                        color: '#00728C',
                        stepSize: 1
                    },
                    afterDataLimits: function (scale) {
                        const suggestedMax = scale.max + (maxCount * .1);
                        scale.max = suggestedMax;
                    },
                    grid: {
                        color: '#DCDCDC',
                        drawOnChartArea: true,
                        drawTicks: true,
                        drawBorder: true
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    };

    if (myChartV2) {
        myChartV2.destroy();
    }
    const ctx = document.getElementById('groupedDesignationChart').getContext('2d');
    myChartV2 = new Chart(ctx, chartConfig);
}
function loadGraphV3(dataSet, urlParams_in, requestPayload) {
    document.getElementById('chartDiv3').style.visibility = 'visible';
    var chartDataSet = transformToChartDatasetV3(dataSet);
    const htmlLegendPlugin = {
        id: 'htmlLegend',
        afterUpdate(chart, args, options) {
            const ul = getOrCreateLegendList(chart, options.containerID);

            // Remove old legend items
            while (ul.firstChild) {
                ul.firstChild.remove();
            }

            // Reuse the built-in legendItems generator
            const items = chart.options.plugins.legend.labels.generateLabels(chart);

            items.forEach(item => {
                const li = document.createElement('li');
                li.style.alignItems = 'center';
                li.style.cursor = 'pointer';
                li.style.display = 'flex';
                li.style.flexDirection = 'row';
                li.style.marginLeft = '10px';
                li.style.font = 'bold 12px sans-serif';

                li.onclick = () => {
                    const { type } = chart.config;
                    if (type === 'pie' || type === 'doughnut') {
                        // Pie and doughnut charts only have a single dataset and visibility is per item
                        chart.toggleDataVisibility(item.index);
                    } else {
                        chart.setDatasetVisibility(item.datasetIndex, !chart.isDatasetVisible(item.datasetIndex));
                    }
                    chart.update();
                };

                // Color box
                const boxSpan = document.createElement('span');
                boxSpan.style.background = item.fillStyle;
                boxSpan.style.borderColor = item.strokeStyle;
                boxSpan.style.borderWidth = item.lineWidth + 'px';
                boxSpan.style.display = 'inline-block';
                boxSpan.style.flexShrink = 0;
                boxSpan.style.height = '20px';
                boxSpan.style.marginRight = '10px';
                boxSpan.style.width = '20px';

                // Text
                const textContainer = document.createElement('p');
                textContainer.style.color = item.fontColor;
                textContainer.style.margin = 0;
                textContainer.style.padding = 0;
                textContainer.style.textDecoration = item.hidden ? 'line-through' : '';

                const text = document.createTextNode(item.text);
                textContainer.appendChild(text);

                li.appendChild(boxSpan);
                li.appendChild(textContainer);
                ul.appendChild(li);
            });
        }
    };

    const config = {
        type: 'bar',
        data: chartDataSet,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'nearest',
                intersect: false
            },
            onHover: function (event, elements) {
                const canvas = event.chart.canvas;
                if (elements.length > 0 && elements[0].element?.$context?.raw != null) {
                    canvas.style.cursor = 'pointer';
                } else {
                    canvas.style.cursor = 'default';
                }
            },
            layout: {
                padding: {
                    bottom: 20,
                    top: 30
                }
            },
            onClick: (evt, elements) => {
                if (elements.length > 0) {
                    const chart = elements[0].element.$context.chart;
                    const { datasetIndex, index } = elements[0];
                    const groupName = chart.data.labels[index];
                    const fid = crypto.randomUUID();
                    const rangeLabel = chart.data.datasets[datasetIndex].label;
                    const rangeMapJson = $("#h_pageAction").val();
                    const rangeMap = JSON.parse(rangeMapJson);
                    const rangeId = Object.entries(rangeMap).find(([key, value]) => value === rangeLabel)?.[0];
                    requestPayload.rangeId = rangeId;
                    requestPayload.designation = groupName;
                    sessionStorage.setItem(fid, JSON.stringify(requestPayload));
                    var urlParams = `/IOM/IOMDashboadReportList?fid=${fid}`;
                    window.open(urlParams, '_blank');
                }
            },
            plugins: {
                /*legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        padding: 10
                    }
                },*/
                legend: {
                    display: false,
                },
                htmlLegend: {
                    // ID of the container to put the legend in
                    containerID: 'chart3-legend-container',
                },
                tooltip: {
                    enabled: false
                },
                datalabels: {
                    display: function (context) {
                        return context.dataset.data[context.dataIndex] !== 0; //xLabels
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: -1,
                    clip: false,
                    color: '#000',
                    backgroundColor: 'transparent',
                    borderRadius: 4,
                    font: function (context) {
                        const bar = context.chart.getDatasetMeta(context.datasetIndex).data[context.dataIndex];
                        const width = bar.width;

                        let size = width > 60 ? 16 : width > 30 ? 13 : 10;
                        size = Math.max(size, 8);

                        return {
                            weight: 'bold',
                            size: size
                        };
                    },
                    padding: {
                        top: 1,
                        bottom: 1,
                        left: 4,
                        right: 4
                    },
                    formatter: function (value, context) {
                        //console.log(context.dataset.label);
                        return value !== 0 ? `${value}` : '';
                    }
                },
            },
            scales: {
                x: {
                    ticks: {
                        callback: function (value, index, ticks) {
                            const chart = this.chart;
                            const datasets = chart.data.datasets;

                            const total = datasets.reduce((sum, ds, i) => {
                                const meta = chart.getDatasetMeta(i);
                                if (meta.hidden) return sum;

                                const point = ds.data[index];
                                return sum + (typeof point === 'object' ? point.y || 0 : point || 0);
                            }, 0);

                            const label = chart.data.labels[index];
                            return `${label} [${total}]`;
                        },
                        font: {
                            family: 'sans-serif',    // Font family
                            size: 12,           // Font size in px
                            weight: 'bold',     // 'normal', 'bold', 'lighter', etc.
                            //style: 'italic'     // optional: 'normal', 'italic'
                        },
                        color: '#00728C',
                        padding: 2
                    },
                    grid: {
                        display: false
                    },
                    offset: true
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Records',
                        color: '#00728C'
                    },
                    ticks: {
                        color: '#00728C',
                        stepSize: 1
                    },
                    afterDataLimits: function (scale) {
                        const suggestedMax = scale.max;
                        scale.max = suggestedMax;
                    },
                    grid: {
                        color: '#DCDCDC',
                        drawOnChartArea: true,
                        drawTicks: true,
                        drawBorder: true
                    }
                }
            }
        },
        plugins: [ChartDataLabels, htmlLegendPlugin]
    };

    if (myChartV3) {
        myChartV3.destroy();
    }
    const ctx = document.getElementById('groupedDesignationChartWithRanges').getContext('2d');
    myChartV3 = new Chart(ctx, config);
}

//Added by TTL on 05-Aug-2025 against SR104160 > CR6821 - Start
function GetApprovers() {
    let hasOpListData = 0;
    let hasDivisionListData = 0;
    let hasDeptListData = 0;
    let hasSecListData = 0;

    if ($("#ddlOp option").length === 1) {
        hasOpListData = -1;
    }
    if ($("#ddlDiv option").length === 1) {
        hasDivisionListData = -1;
    }
    else if ($("#ddlDep option").length === 1) {
        hasDeptListData = -1;
    }
    else if ($("#ddlSec option").length === 1) {
        hasSecListData = -1;
    }

    const data = {
        OperationID: $('#ddlOp').val() == "" ? departmentLabelId : $('#ddlOp').val(),
        DivisionID: $('#ddlDiv').val() == "" ? 0 : $('#ddlDiv').val(),
        DEPTID: $('#ddlDep').val() == "" ? 0 : $('#ddlDep').val(),
        SECID: $('#ddlSec').val() == "" ? 0 : $('#ddlSec').val(),
        Status: 0,
        IOMCATMSTID: $('#ddCategory').val() == "" ? 0 : $('#ddCategory').val(),
        KIID: $('#ddlKI').val(),
        HasDivisionListData: hasDivisionListData,
        HasDeptListData: hasDeptListData,
        HasSecListData: hasSecListData,
        /*Added by TTL on 29-July-2025 against SR104160 > CR6821 - Start*/
        PendingAtUsers: $('#ddlPendingAt').val(),
        PendingWithType: $('input[name="PendingWithType"]:checked').val(),
        rangeId: 0,
        deptLavelId: 0
        /*Added by TTL on 29-July-2025 against SR104160 > CR6821 - End*/
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            showEstimatedTime();
        },
        url: '/IOM/GetApproversAsPerOrgLevel',
        //data: JSON.stringify(data),
        data: data,
        //contentType: "application/json; charset=utf-8",
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        datatype: "json",
        success: function (d) {
            PopulateApprovers(d);
        },
        complete: function () {
            hideEstimatedTime();
        },
        error: function () {
            hideEstimatedTime();
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function PopulateApprovers(approverList) {
    try {
        $('#ddlPendingAt').html('');
        if (approverList.length > 0) {
            const options = approverList
                .map(item => `<option value="${item.Value}">${item.Text}</option>`)
                .join('\n');
            $('#ddlPendingAt').html(options);
        }
        $('#ddlPendingAt').multiselect('destroy');
        $('#ddlPendingAt').multiselect({
            nonSelectedText: '-- Select Pending At --',
            includeSelectAllOption: true,
            maxHeight: 250,
            dropUp: false,
            buttonWidth: 170,
            enableFiltering: true,
            buttonWidth: '100%',
            enableCaseInsensitiveFiltering: true,
            numberDisplayed: 3
        });
    } catch (e) {
        console.log(e);
    }
}
//Added by TTL on 05-Aug-2025 against SR104160 > CR6821 - End