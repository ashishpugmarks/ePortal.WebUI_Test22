let categoryChartInstance; let sectionWiseChartInstance;
$(document).ready(function () {
    let SiteId = $("#ddlSite").val();
    BindSelectList("/VQMS_DPR/GetLines", "ddline", SiteId);
    BindSelectList("/VQMS_DPR/GetModels", "ddlModel", SiteId);
    GetDocType(SiteId)

    $("#ddlSite").change(function () {
        BindSelectList("/VQMS_DPR/GetLines", "ddline", $(this).val());
        BindSelectList("/VQMS_DPR/GetModels", "ddlModel", $(this).val());
        GetDocType($(this).val());
    });

    $("#btnPrint").click(function () {
        window.print();
    });
    $("#btnClose").click(function () {
        window.close();
    });
    $("#btnSearch").click(function (e) {
        e.preventDefault();
        Search();
    });
    $("#txtDate").datepicker({
        format: "dd-M-yyyy",
        autoclose: true
    }).datepicker("setDate", new Date());
    $("#S1").on("click", function (e) {
        e.preventDefault();
        $("#txtDate").datepicker("show");
    });
    $("#R1").click(removefromdate);

    $("#btnExport").click(function (e) {
        e.preventDefault();
        const data = {
            txtDate: $('#txtDate').val(),
            ddlModel: $('#ddlModel').val(),
            ddline: $('#ddline').val(),
            ddlSite: $('#ddlSite').val()
        }
        window.location.href = `/VQMS_DPR/Export?txtDate=${data.txtDate}&ddlModel=${data.ddlModel}&ddlSite=${data.ddlSite}&ddline=${data.ddline}`;
    });
});
function BindSelectList(url, id, SiteId) {
    $.ajax({
        url: url,
        type: 'GET',
        data: { SiteId: SiteId },
        success: function (data) {
            var ddl = $('#' + id);
            ddl.empty();
            $.each(data, function (i, item) {
                ddl.append($('<option>', {
                    value: item.Value,
                    text: item.Text
                }));
            });
        },
        error: function () {
            alert('Error loading dropdown data');
        }
    });
}
function GetDocType(SiteId) {
    $.ajax({
        url: "/VQMS_DPR/GetDocType",
        type: 'GET',
        data: { SiteId: SiteId },
        success: function (data) {
            $("#lblDocType").val(data.lblDocType);
        },
        error: function () {
            alert('Error bind DocType text');
        }
    });
}
function Search() {
    $.ajax({
        url: '/VQMS_DPR/SearchFinalInspectionReport_DPR',
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        type: 'GET',
        data: {
            txtDate: $('#txtDate').val(),
            ddlModel: $('#ddlModel').val(),
            ddline: $('#ddline').val(),
            ddlSite: $('#ddlSite').val()
        },
        success: function (data) {
            // const DefectWiseReportHtml = buildDefectTable(data.DefectWiseReport);
            // $('#tblDefectRows').html(DefectWiseReportHtml);
            // const SectionWiseTableHtml = buildSectionWiseTable(data.DefectWiseReport);
            // $('#tblShopwiseDefects').html(SectionWiseTableHtml);
            updateReportHeader(data.ReportHeader);

            sectionWiseGraph(data.SectionWiseGraph);
            categoryWiseGraph(data.CategoryWiseGraph)

            buildDefectTable(data.DefectWiseReport);
            buildSectionWiseTable(data.SectionWiseReport);
            buildCategorywiseTable(data.CategoryWiseReport);
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            alert('Error loading defect data');
        }
    });
}
function updateReportHeader(data) {
    $('#lblFrameNotEntered').text(data.lblFrameNotEntered);
    $('#lblProductQtyShiftA').text(data.lblProductQtyShiftA);
    $('#lblProductionQtyShiftB').text(data.lblProductionQtyShiftB);
    $('#lblInpectionQtyShiftA').text(data.lblInpectionQtyShiftA);
    $('#lblInspectionQtyShiftB').text(data.lblInspectionQtyShiftB);
    $('#lblStraightPassQtyGR1ShiftA').text(data.lblStraightPassQtyGR1ShiftA);
    $('#lblStraightPassQtyGR2ShiftB').text(data.lblStraightPassQtyGR2ShiftB);
    $('#lblStraightPassRatioGR1ShiftA').text(data.lblStraightPassRatioGR1ShiftA);
    $('#lblStraightPassRatioGR2ShiftB').text(data.lblStraightPassRatioGR2ShiftB);
    $('#lblDirectPassQtyGR1ShiftA').text(data.lblDirectPassQtyGR1ShiftA);
    $('#lblDirectPassQtyGR2ShiftB').text(data.lblDirectPassQtyGR2ShiftB);
    $('#lblDirectPassRatioGR1ShiftA').text(data.lblDirectPassRatioGR1ShiftA);
    $('#lblDirectPassRatioGR2ShiftB').text(data.lblDirectPassRatioGR2ShiftB);
    $('#lblPDIOffGR1ShiftA').text(data.lblPDIOffGR1ShiftA);
    $('#lblPDIOFFGR2ShiftB').text(data.lblPDIOFFGR2ShiftB);
    $('#lblTotalProductionQty').text(data.lblTotalProductionQty);
    $('#lblTotalInpectionQty').text(data.lblTotalInpectionQty);
    $('#lblTotalStraightPassQty').text(data.lblTotalStraightPassQty);
    $('#lblTotalStraightPassRatio').text(data.lblTotalStraightPassRatio);
    $('#lblTotalDirectPassQty').text(data.lblTotalDirectPassQty);
    $('#lblTotalDirectPassRatio').text(data.lblTotalDirectPassRatio);
    $('#lblTotalPDIOff').text(data.lblTotalPDIOff);
    $('#lblDate').text(data.lblDate);
    $('#lblModel').text(data.lblModel);
    $('#lblTotalDPV').text(data.lblTotalDPV);
}
function sectionWiseGraph(data) {
    const labels = data.map(r => r.Section);
    const defects = data.map(r => r.TotalDefects);
    const pareto = data.map(r => r.CM);

    const ctx = document.getElementById('crtSectionWise').getContext('2d');

    if (sectionWiseChartInstance) {
        sectionWiseChartInstance.destroy();
    }

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

    sectionWiseChartInstance = new Chart(ctx, {
        data: {
            labels: labels,
            datasets: [
                {
                    type: 'bar',
                    label: 'Defects',
                    data: defects,
                    backgroundColor: '#9999ff',
                    borderColor: '#1a3b69',
                    borderWidth: 1,
                    barPercentage: 0.3,
                    categoryPercentage: 0.7,
                    yAxisID: 'y'
                },
                {
                    type: 'line',
                    label: 'Pareto',
                    data: pareto,
                    yAxisID: 'y1',
                    borderColor: 'rgba(252,180,65,1)',
                    backgroundColor: 'rgba(252,180,65,0.3)',
                    pointStyle: 'diamond',
                    pointRadius: 6,
                    pointBorderColor: 'midnightblue',
                    pointBackgroundColor: 'red',
                    tension: 0.3
                }
            ]
        },
        options: {
            responsive: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Sectionwise Defects'
                },
                datalabels: {
                    color: 'black',
                    anchor: 'end',
                    align: 'top',
                    formatter: function (value) {
                        return value;
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Defects' },
                    position: 'left'
                },
                y1: {
                    beginAtZero: true,
                    max: 100,
                    position: 'right',
                    grid: { drawOnChartArea: false },
                    title: { display: true, text: 'Pareto (%)' }
                }
            }
        },
        plugins: [ChartDataLabels, bringLineToFront]
    });
}
function categoryWiseGraph(data) {
    const labels = data.map(r => r.Category);
    const defects = data.map(r => r.TotalDefects);
    const pareto = data.map(r => r.CM);

    const ctx = document.getElementById('crtCategoryWise').getContext('2d');

    if (categoryChartInstance) {
        categoryChartInstance.destroy();
    }

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

    categoryChartInstance = new Chart(ctx, {
        data: {
            labels: labels,
            datasets: [
                {
                    type: 'bar',
                    label: 'Defects',
                    data: defects,
                    backgroundColor: '#9999ff',
                    borderColor: '#1a3b69',
                    borderWidth: 1,
                    barPercentage: 0.3,
                    categoryPercentage: 0.7,
                    yAxisID: 'y'
                },
                {
                    type: 'line',
                    label: 'Pareto',
                    data: pareto,
                    yAxisID: 'y1',
                    borderColor: 'rgba(252,180,65,1)',
                    backgroundColor: 'rgba(252,180,65,0.3)',
                    pointStyle: 'diamond',
                    pointRadius: 6,
                    pointBorderColor: 'midnightblue',
                    pointBackgroundColor: 'red',
                    tension: 0.3
                }
            ]
        },
        options: {
            responsive: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Categorywise Defects'
                },
                datalabels: {
                    color: 'black',
                    anchor: 'end',
                    align: 'top',
                    formatter: function (value) {
                        return value;
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Defects' }
                },
                y1: {
                    beginAtZero: true,
                    max: 100,
                    position: 'right',
                    grid: { drawOnChartArea: false },
                    title: { display: true, text: 'Pareto (%)' }
                }
            }
        },
        plugins: [ChartDataLabels, bringLineToFront]
    });
}
function buildDefectTable(data) {
    let html = `
                <table class="table table-bordered table-sm" style="font-size:9px;font-family:Arial;">
                    <thead style="background-color:#FFCC99; font-weight:bold; text-align:center;">
                        <tr>
                            <td>DEFECT</td>
                            <td>CATEGORY</td>
                            <td>SEC</td>
                            <td>A</td>
                            <td>B</td>
                            <td>TOTAL</td>
                        </tr>
                    </thead>
                    <tbody>
            `;

    data.Rows.forEach(row => {
        html += `
                    <tr>
                        <td>${row.DefectDescription}</td>
                        <td>${row.Category}</td>
                        <td>${row.Section}</td>
                        <td>${row.AShift}</td>
                        <td>${row.BShift}</td>
                        <td>${row.Total}</td>
                    </tr>
                `;
    });

    html += `
                <tr style="font-weight:bold; text-align:center;">
                    <td colspan="3">Total</td>
                    <td>${data.TotalShiftA}</td>
                    <td>${data.TotalShiftB}</td>
                    <td>${data.GrandTotal}</td>
                </tr>
                </tbody>
                </table>
            `;
    $('#tblDefectRows').html(html);
    // return html;
}
function buildSectionWiseTable(data) {
    let html = `
                <table class="table table-bordered table-sm" style="font-size:9px;font-family:Arial; width:60%;" align="center">
                    <thead style="background-color:#FFCC99; font-weight:bold; text-align:center;">
                        <tr>
                            <td>Shop</td>
                            <td>Shift 'A'</td>
                            <td>Shift 'B'</td>
                            <td>Total</td>
                            <td>D/1000 V</td>
                        </tr>
                    </thead>
                    <tbody>
            `;

    data.Rows.forEach(row => {
        html += `
                    <tr>
                        <td>${row.Section}</td>
                        <td>${row.AShift}</td>
                        <td>${row.BShift}</td>
                        <td>${row.TotalDefects}</td>
                        <td>${row.Dpv}</td>
                    </tr>
                `;
    });

    html += `
                <tr style="font-weight:bold; text-align:center;">
                    <td>Total</td>
                    <td>${data.TotalShiftA}</td>
                    <td>${data.TotalShiftB}</td>
                    <td>${data.GrandTotal}</td>
                    <td>${data.TotalDpv}</td>
                </tr>
                </tbody>
                </table>
            `;
    $('#tblShopwiseDefects').html(html);
    $('#lblTotalDPV').text(data.lblTotalDPV);
    // return html;
}
function buildCategorywiseTable(data) {
    let html = `
                <table class="table table-bordered table-sm" style="font-size:9px;font-family:Arial; width:60%;"align="center">
                    <thead style="background-color:#FFCC99; font-weight:bold; text-align:center;">
                        <tr>
                            <td>Items</td>
                            <td>Shift 'A'</td>
                            <td>Shift 'B'</td>
                            <td>Total</td>
                        </tr>
                    </thead>
                    <tbody>
            `;

    data.Rows.forEach(row => {
        html += `
                    <tr>
                        <td>${row.Category}</td>
                        <td>${row.AShift}</td>
                        <td>${row.BShift}</td>
                        <td>${row.TotalDefects}</td>
                    </tr>
                `;
    });

    html += `
                <tr style="font-weight:bold; text-align:center;">
                    <td>Total</td>
                    <td>${data.TotalShiftA}</td>
                    <td>${data.TotalShiftB}</td>
                    <td>${data.GrandTotal}</td>
                </tr>
                </tbody>
                </table>
            `;

    $('#tblCategorywiseDefects').html(html);
    // return html;
}

//REMOVE DATES
function removefromdate() {
    document.getElementById('txtDate').value = "";
}
function Readonly() {
    document.getElementById('txtDate').readOnly = true;
}