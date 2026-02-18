
$(document).ready(function () {
    $('.homeContent').removeClass('homeContent');


    const chartColourPlate = [
        '#4C78A8', '#F28E2B', '#E15759', '#76B7B2', '#59A14F', '#A0A0A0', '#EDC948', '#B07AA1', '#4992C4',
        '#FF7F0E', '#2CA02C', '#D62728', '#9467BD', '#8C564B', '#E377C2', '#7F7F7F', '#BCBD22', '#17BECF',
        '#1F77B4', '#FFBB78', '#98DF8A', '#FF9896', '#C5B0D5', '#C49C94', '#F7B6D2', '#C7C7C7', '#DBDB8D', '#9EDAE5'
    ];


    const dtWipDetail = $('#grdwipdetail').DataTable({
        searching: false,
        lengthChange: false,
        paging: true,
        pageLength: 7,
        ordering: false,
        autoWidth: false,
        info: false,
    });

    const dtRenPending = $('#grdrenpending').DataTable({
        searching: false,
        lengthChange: false,
        paging: true,
        pageLength: 5,
        ordering: false,
        autoWidth: false,
        info: false,
        columnDefs: [
            { targets: '_all', defaultContent: '' } 
        ]
    });

    const dtDashDetail = $('#grddashdetail').DataTable({
        searching: false,
        lengthChange: false,
        paging: true,
        pageLength: 10,
        ordering: false,
        autoWidth: false,
        info: false,
        columnDefs: [
            { targets: '_all', defaultContent: '' }
        ]
    });

    const dtPending = $('#grdPendingcount').DataTable({
        searching: false,
        lengthChange: false,
        paging: true,
        pageLength: 5,
        ordering: false,
        autoWidth: false,
        info: false,
        columnDefs: [
            { targets: '_all', defaultContent: '' }
        ]
    });


    // 2) Delegated click handlers (correct for dynamic redraws)
    $('#grdwipdetail').on('click', 'tbody .linkvendorname', function (e) {
        e.preventDefault();
        const iomid = $(this).closest('tr').data('iomid');
        const url = '/IOMContract/ViewIOMDetail?IOMID=' + iomid;
        window.open(url, "_blank", 'width=790,height=550,top=80,left=140', 'resizable=yes,scrollbars=yes');
    });



    $('#grdPendingcount').on('click', 'tbody .linkcontract', function (e) {
        e.preventDefault();
        document.getElementById('divcontdetail').style.display = '';

        const contracttypeid = $(this).data('contracttypeid');
        const HDREQUESTTYPE = "Completed";
        const HDCONTRACTPAGID = "COMPLETED#" + contracttypeid;

        $("#HDCONTRACTTYPEID").val(contracttypeid);
        $("#HDREQUESTTYPE").val(HDREQUESTTYPE);
        $("#HDCONTRACTPAGID").val(HDCONTRACTPAGID);

        GetIOMDashboardContractDetailsData(contracttypeid, "", true);
    });
    $('#grdrenpending').on('click', 'tbody .linkcontracts', function (e) {
        e.preventDefault();
        document.getElementById('divcontdetail').style.display = '';

        const contracttypeid = $(this).data('contracttypeid');
        const HDREQUESTTYPE = "Pending";
        const HDCONTRACTPAGID = "RENEWAL#" + contracttypeid;

        $("#HDCONTRACTTYPEID").val(contracttypeid);
        $("#HDREQUESTTYPE").val(HDREQUESTTYPE);
        $("#HDCONTRACTPAGID").val(HDCONTRACTPAGID);

        GetIOMDashboardContractDetailsData(contracttypeid, "", true);
    });

    $("#ddlcontracttype").on("change", function (e) {
        e.preventDefault();
        const ddlcontracttype = $(this).val();
        document.getElementById('divcontdetail').style.display = 'none';
        GetIOMDashboardProgressChartData(ddlcontracttype);

    });


    $("#ddlvendorlist").on("change", function (e) {
        e.preventDefault();

        const contracttypeid = $("#HDCONTRACTTYPEID").val();
        var venderName = $(this).val();
        GetIOMDashboardContractDetailsData(contracttypeid, venderName, false);

    });



    
    $('#grddashdetail').on('click', 'tbody .LINKFINALDOC', function (e) {
        e.preventDefault();
        const finaldoc = $(this).data('finaldoc');
        const url = '../../Uploads/IOM/' + finaldoc;
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });

    // 3) Load the data
    GetIOMDashboardProgressChartData("");


    function GetIOMDashboardProgressChartData(ddlcontracttype) {

        var formData = new FormData();
        formData.append("hdoperationid", $("#hdoperationid").val() || '');
        formData.append("hddivision", $("#hddivision").val() || '');
        formData.append("hddepartment", $("#hddepartment").val() || '');
        formData.append("ddlcontracttype", ddlcontracttype || '');

        $.ajax({
            url: '/IOMContract/GetIOMDashboardProgressChartData',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            success: function (response) {
                var result = response.result;
                console.log(result);
                if (result) {
                    const grdPendingcount = result.grdPendingcount || [];
                    const grdrenpending = result.grdrenpending || [];

                    BindgrdPendingcount(grdPendingcount);
                    Bindgrdrenpending(grdrenpending);

                    // dataItems: [{ label, value, color }, ...]
                    const dataItems_chart_completedcont_stats = $.map(grdPendingcount, function (item, i) {
                        return {
                            label: item.ContractType,
                            value: Number(item.Cnt) || 0,
                            color: chartColourPlate[i % chartColourPlate.length] // cycle through palette
                        };
                    });

                    const dataItems_chart_pendingcont_stats = $.map(grdrenpending, function (item, i) {
                        return {
                            label: item.ContractType,
                            value: Number(item.Cnt) || 0,
                            color: chartColourPlate[i % chartColourPlate.length] // cycle through palette
                        };
                    });


                    chart_completedcont_stats(dataItems_chart_completedcont_stats);
                    chart_pendingcont_stats(dataItems_chart_pendingcont_stats);
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');

            },
            error: function (xhr, status, error) {
                console.error(xhr.responseJSON?.message || 'An error occurred .');
            }
        });
    }


    function GetIOMDashboardContractDetailsData(contracttypeid, vendername, isAllowToBind_ddlvendorlist) {

        vendername = vendername == null ? '' : vendername;

        var formData = new FormData();
        formData.append("contracttypeid", contracttypeid);
        formData.append("hdoperationid", $("#hdoperationid").val() || '');
        formData.append("HDREQUESTTYPE", $("#HDREQUESTTYPE").val() || '');
        formData.append("VENDORNAME", vendername);

        $.ajax({
            url: '/IOMContract/GetIOMDashboardContractDetailsData',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                debugger
                var result = response.result;
                console.log(result);
                if (result) {
                    Bindgrddashdetail(result.grddashdetail);
                    if (isAllowToBind_ddlvendorlist === true)
                    {
                        const ddlvendorlist = result.ddlvendorlist;
                        $('#ddlvendorlist').empty();
                        ddlvendorlist.forEach(item => {
                            $('#ddlvendorlist').append(`<option value="${item.Value}">${item.Text}</option>`);
                        });

                    }
                }
            },
            complete: function () {
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseJSON?.message || 'An error occurred .');
            }
        });
    }


    // 4) Expose bind functions that UPDATE the DTs instead of re-init
    window.BindgrdPendingcount = function (data) {
        dtPending.clear();
        data.forEach(item => {
            dtPending.row.add([
                `<a class="linkcontract" data-contracttypeid="${item.contracttypeid}">${item.ContractType}</a>`,
                `<div style="text-align: center;vertical-align: middle;"><span class="datastyle" >${item.Cnt}</span></div>`
            ]);
        });

        dtPending.draw(true);
    };

    window.Bindgrdrenpending = function (data) {
        dtRenPending.clear();
        data.forEach(item => {
            dtRenPending.row.add([
                `<a class="linkcontracts" data-contracttypeid="${item.contracttypeid}">${item.ContractType}</a>`,
                `<div style="text-align: center;vertical-align: middle;"><span class="datastyle" >${item.Cnt}</span></div>`
            ]);
        });

        dtRenPending.draw(true);

    };


    window.Bindgrddashdetail = function (data) {
        dtDashDetail.clear();

        data.forEach(item => {
            dtDashDetail.row.add([
                `<div style="text-align: center;vertical-align: middle;"><span class="lblcategory">${item.ContractType ?? ''}</span></div>`,
                `<span class="datastyle lblact" style="line-height:14px;">${item.vendorname ?? ''}</span>`,
                `<div style="text-align: center;vertical-align: middle;"><span class="lblrequirement">${item.dateofagreement ?? ''}</span></div>`,
                `<div style="text-align: center;vertical-align: middle;"><span class="lblexpdate">${item.dateofexpiry ?? ''}</span></div>`,
                `<div style="text-align: center;vertical-align: middle;"><a class="LINKFINALDOC" data-finaldoc="${item.finaldoc ?? ''}">  <img src="/images/icon_topic.gif" /></a></div></div>`
            ]);
        });

        dtDashDetail.draw(true); 
    };



    // ============================================================================================== chart ==============================================================
    
    function chart_completedcont_stats(data)
    {
       
       const dataItems = data;
       
        const labels = dataItems.map(x => x.label);
        const values = dataItems.map(x => x.value);
        const colors = dataItems.map(x => x.color);

        const canvas = document.getElementById('chart_completedcont_stats');
        const existing = Chart.getChart(canvas);  // Chart.js v4/v3

        if (existing) existing.destroy();


       const ctx = document.getElementById('chart_completedcont_stats').getContext('2d');
       const gradients = makeGradients(ctx, colors);


       


        Chart.register(ChartDataLabels);

        const chart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels,
                datasets: [{
                    data: values,
                    backgroundColor: gradients,
                    borderColor: '#ffffff',
                    borderWidth: 1,
                    hoverOffset: 6
                }]
            },
            options: {
                responsive: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'right',
                        align: 'midle',  
                        labels: {
                            boxWidth: 14,  
                            padding: 12, 
                            usePointStyle: false
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: (tt) => `${tt.label}: ${tt.parsed}`
                        }
                    },
                    datalabels: {
                        color: '#fff',
                        font: { weight: 'bold', size: 14 },
                        formatter: value => value > 0 ? value : '',
                        textStrokeColor: 'rgba(0,0,0,0.35)',
                        textStrokeWidth: 3,
                        clip: true
                    }
                },
                animation: { duration: 600 }
            },
            plugins: [{
                id: 'sliceShadow',
                beforeDatasetDraw(chart) {
                    const { ctx } = chart;
                    ctx.save();
                    ctx.shadowColor = 'rgba(0,0,0,0.25)';
                    ctx.shadowBlur = 12;
                    ctx.shadowOffsetX = 4;
                    ctx.shadowOffsetY = 6;
                },
                afterDatasetDraw(chart) {
                    chart.ctx.restore();
                }
            }]
        });
        function shade(hex, percent) {
            const f = parseInt(hex.slice(1), 16),
                t = percent < 0 ? 0 : 255,
                p = Math.abs(percent) / 100,
                R = f >> 16, G = f >> 8 & 0x00FF, B = f & 0x0000FF;
            const newR = Math.round((t - R) * p) + R;
            const newG = Math.round((t - G) * p) + G;
            const newB = Math.round((t - B) * p) + B;
            return `#${(0x1000000 + (newR << 16) + (newG << 8) + newB).toString(16).slice(1)}`;
        }

        function makeGradients(ctx, baseColors) {
            const grads = [];
            baseColors.forEach(color => {
                const g = ctx.createLinearGradient(0, 0, 0, 360);
                g.addColorStop(0, color);
                g.addColorStop(1, shade(color, -15));
                grads.push(g);
            });
            return grads;
        }

    }

    function chart_pendingcont_stats(data) {

       const dataItems = data;

        const labels = dataItems.map(x => x.label);
        const values = dataItems.map(x => x.value);
        const colors = dataItems.map(x => x.color);

        const canvas = document.getElementById('chart_pendingcont_stats');
        const existing = Chart.getChart(canvas);  // Chart.js v4/v3

        if (existing) existing.destroy();


        const ctx = document.getElementById('chart_pendingcont_stats').getContext('2d');
        const gradients = makeGradients(ctx, colors);

        Chart.register(ChartDataLabels);

        const chart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels,
                datasets: [{
                    data: values,
                    backgroundColor: gradients,
                    borderColor: '#ffffff',
                    borderWidth: 1,
                    hoverOffset: 6
                }]
            },
            options: {
                responsive: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'right',
                        align: 'midle',
                        labels: {
                            boxWidth: 14,
                            padding: 12,
                            usePointStyle: false
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: (tt) => `${tt.label}: ${tt.parsed}`
                        }
                    },
                    datalabels: {
                        color: '#fff',
                        font: { weight: 'bold', size: 14 },
                        formatter: value => value > 0 ? value : '',
                        textStrokeColor: 'rgba(0,0,0,0.35)',
                        textStrokeWidth: 3,
                        clip: true
                    }
                },
                animation: { duration: 600 }
            },
            plugins: [{
                id: 'sliceShadow',
                beforeDatasetDraw(chart) {
                    const { ctx } = chart;
                    ctx.save();
                    ctx.shadowColor = 'rgba(0,0,0,0.25)';
                    ctx.shadowBlur = 12;
                    ctx.shadowOffsetX = 4;
                    ctx.shadowOffsetY = 6;
                },
                afterDatasetDraw(chart) {
                    chart.ctx.restore();
                }
            }]
        });
        function shade(hex, percent) {
            const f = parseInt(hex.slice(1), 16),
                t = percent < 0 ? 0 : 255,
                p = Math.abs(percent) / 100,
                R = f >> 16, G = f >> 8 & 0x00FF, B = f & 0x0000FF;
            const newR = Math.round((t - R) * p) + R;
            const newG = Math.round((t - G) * p) + G;
            const newB = Math.round((t - B) * p) + B;
            return `#${(0x1000000 + (newR << 16) + (newG << 8) + newB).toString(16).slice(1)}`;
        }

        function makeGradients(ctx, baseColors) {
            const grads = [];
            baseColors.forEach(color => {
                const g = ctx.createLinearGradient(0, 0, 0, 360);
                g.addColorStop(0, color);
                g.addColorStop(1, shade(color, -15));
                grads.push(g);
            });
            return grads;
        }

    }


});
