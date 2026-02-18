$(document).ready(function () {

    //$("#ddlSite").val($("#hdnSite").val());

    GetChart1Data();

    $("#txtdatefrom").datepicker({
        format: "dd-M-yyyy",
        autoclose: true
    }).datepicker("setDate", new Date());
    $("#S1").on("click", function (e) {
        e.preventDefault();
        $("#txtdatefrom").datepicker("show");
    });
    $("#lbldate").text($("#txtdatefrom").val());
    $("#btnSearch").on("click", function (e) {
        e.preventDefault();
        if ($("#txtdatefrom").val() == "") {
            alert("Please select Date")
            return;
        }
        $("#lbldate").text($("#txtdatefrom").val());
        GetChart1Data();
    });
});
let barChart1, barChart2, barChart3;
function GetChart1Data() {
    //const factoryText = $("#ddlSite option:selected").text();

    const data = {
        txtdatefrom: $("#txtdatefrom").val(),
        ddlSite: $("#ddlSite").val(),
    };

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/VQMS_DPR/SerchDefectReport",
        data: data,
        success: function (data) {
            console.log(data)
            if (barChart1) barChart1.destroy();
            barChart1 = bindChart1(data);
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function (xhr) {
            sweetAlert("Oops...", "Something went wrong! " + xhr.responseText, "error");
        }
    });
}
function bindChart1(data) {
    const ctx = document.getElementById('Chart1').getContext('2d');
    const labels = data.map(x => x.DISC);
    const values = data.map(x => x.COUNT);
    const subcode = data.map(x => x.SUBCODE);

    const colors = values.map(() => 'rgba(0, 30, 255, 0.8)');

    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Defects',
                data: values,
                backgroundColor: colors,   // array of colors
                //borderColor: 'rgba(54, 162, 235, 1)',
                //borderWidth: 1,
                barPercentage: 0.5,
                categoryPercentage: 0.8
            }]
        },
        options: {
            responsive: true,
            plugins: {
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    color: 'black',
                    font: { weight: 'bold', size: 10 },
                    formatter: function (value) {
                        return value;
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let tooltip = data[context.dataIndex].SUBCODE;
                            return context.dataset.label + ': ' + context.raw + ' (' + tooltip + ')';
                        }
                    }
                }
            },
            scales: {
                x: {
                    barPercentage: 0.5,
                    categoryPercentage: 0.7
                },
                y: {
                    beginAtZero: true,
                    min: 0,
                    //max: 120,
                    ticks: {
                        stepSize: 20
                    }
                }
            },
            onHover: (event, elements) => {
                if (elements.length > 0) {
                    event.native.target.style.cursor = 'pointer';
                } else {
                    event.native.target.style.cursor = 'default';
                }
            },
            onClick: (evt, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    for (let i = 0; i < colors.length; i++) {
                        colors[i] = 'rgba(0, 30, 255, 0.8)';
                    }
                    colors[index] = 'rgba(255, 0, 0, 0.8)';
                    evt.chart.update();

                    GetChart2Data(data[index].SUBCODE);
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}

function GetChart2Data(index) {
    //const factoryText = $("#ddlSite option:selected").text();

    const data = {
        strShop: index,
        txtdatefrom: $("#txtdatefrom").val(),
        ddlSite: $("#ddlSite").val(),
    };

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/VQMS_DPR/GetModelWiseDefectReport",
        data: data,
        success: function (data) {
            console.log(data)
            if (barChart2) barChart2.destroy();
            barChart2 = bindChart2(data);
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function (xhr) {
            sweetAlert("Oops...", "Something went wrong! " + xhr.responseText, "error");
        }
    });
}
function bindChart2(data) {
    const ctx = document.getElementById('ModelWiseChart').getContext('2d');
    const disc = data.map(x => x.DISC);
    const labels = data.map(x => x.MODELCODE);
    const values = data.map(x => x.COUNT);
    const subcode = data.map(x => x.SUBCODE);
    const colors = values.map(() => 'rgba(0, 30, 255, 0.8)');
    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Defects',
                data: values,
                backgroundColor: colors,   // array of colors
                //borderColor: 'rgba(54, 162, 235, 1)',
                //borderWidth: 1,
                barPercentage: 0.5,
                categoryPercentage: 0.8
            }]
        },
        options: {
            responsive: true,
            plugins: {
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    color: 'black',
                    font: { weight: 'bold', size: 10 },
                    formatter: function (value) {
                        return value;
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let tooltip = data[context.dataIndex].SUBCODE;
                            return context.dataset.label + ': ' + context.raw + ' (' + tooltip + ')';
                        }
                    }
                }
            },
            scales: {
                x: {
                    barPercentage: 0.5,
                    categoryPercentage: 0.7
                },
                y: {
                    beginAtZero: true,
                    min: 0,
                    //max: 120,
                    ticks: {
                        stepSize: 10
                    }
                }
            },
            onHover: (event, elements) => {
                if (elements.length > 0) {
                    event.native.target.style.cursor = 'pointer';
                } else {
                    event.native.target.style.cursor = 'default';
                }
            },
            onClick: (evt, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    for (let i = 0; i < colors.length; i++) {
                        colors[i] = 'rgba(0, 30, 255, 0.8)';
                    }
                    colors[index] = 'rgba(255, 0, 0, 0.8)';
                    evt.chart.update();

                    GetChart3Data(data[index].SUBCODE, data[index].MODELCODE);
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}

function GetChart3Data(shopcode, ModelCode) {
    //const factoryText = $("#ddlSite option:selected").text();

    const data = {
        strShop: shopcode,
        txtdatefrom: $("#txtdatefrom").val(),
        strModel: ModelCode,
        ddlSite: $("#ddlSite").val(),
    };

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/VQMS_DPR/GetDefectWiseReport",
        data: data,
        success: function (data) {
            console.log(data)
            if (barChart3) barChart3.destroy();
            barChart3 = bindChart3(data);
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function (xhr) {
            sweetAlert("Oops...", "Something went wrong! " + xhr.responseText, "error");
        }
    });
}
function bindChart3(data) {
    const ctx = document.getElementById('CatWiseChart').getContext('2d');
    const disc = data.map(x => x.DISC);
    const labels = data.map(x => x.DEFECTDESCRIPTION);
    const values = data.map(x => x.COUNT);
    const subcode = data.map(x => x.SUBCODE);
    const colors = values.map(() => 'rgba(0, 30, 255, 0.6)');
    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Defects',
                data: values,
                backgroundColor: colors,   // array of colors
                //borderColor: 'rgba(54, 162, 235, 1)',
                //borderWidth: 1,
                barPercentage: 0.5,
                categoryPercentage: 0.8
            }]
        },
        options: {
            responsive: true,
            plugins: {
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    color: 'black',
                    font: { weight: 'bold', size: 10 },
                    formatter: function (value) {
                        return value;
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let tooltip = data[context.dataIndex].SUBCODE;
                            return context.dataset.label + ': ' + context.raw + ' (' + tooltip + ')';
                        }
                    }
                }
            },
            scales: {
                x: {
                    barPercentage: 0.5,
                    categoryPercentage: 0.7
                },
                y: {
                    beginAtZero: true,
                    min: 0,
                    ticks: {
                        stepSize: 1
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}