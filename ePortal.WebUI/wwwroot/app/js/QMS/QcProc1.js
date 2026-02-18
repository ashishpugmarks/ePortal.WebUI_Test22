$('.homeContent').removeClass('homeContent');
$(document).ready(function () {

    let QCPROCSESSION_Storage = {
        hasPageLoadData: false,
        strPlant: "",
        strdept: "",
        strPgNo: 0
    }

    const dtgrdDeptProc = $('#grdDeptProc').DataTable({
        searching: false,
        lengthChange: false,
        paging: true,
        pageLength: 10,
        ordering: false,
        autoWidth: false,
        info: false,
        columnDefs: [
            { targets: '_all', defaultContent: '' }
        ],

        stateSave: false,
        displayStart: 0   

    });


    $('.p1').on('click', function (e) {
        e.preventDefault();
        const p1 = $(this);
        const filename = p1.data('filename');
        const qcqmsdocid = p1.data('qcqmsdocid');
        let url = "";

        if (filename && filename.trim() !== '')
        {
            url = "../../Uploads/Qms/" + filename;
            window.open(url, "_blank", 'width=800,height=600,top=90,left=200', 'resizable=yes,scrollbars=yes');
        }
        else
        {

            url = "/QMS/SubQcProc?ID=" + qcqmsdocid;
            window.location.href = url;
        }

    });



    $("#ddlPlant").on("change", function () {
        $("#ddlDept").val("");

        bindTable_grdDeptProc(true);
    });

    $("#ddlDept").on("change", function () {
        bindTable_grdDeptProc(false);
    });


    QCPROCSESSION();
    bindTable_grdDeptProc(true);

    function QCPROCSESSION()
    {
        
        const qcprocession = $("#QCPROCSESSION").val();
        if (qcprocession && qcprocession.trim() !== "")
        {
            var sessionresult = qcprocession.split('#');

            QCPROCSESSION_Storage.hasPageLoadData = true;
            QCPROCSESSION_Storage.strPlant = sessionresult[2];
            QCPROCSESSION_Storage.strdept = sessionresult[3];
            QCPROCSESSION_Storage.strPgNo = sessionresult[5];

            $("#ddlPlant").val(QCPROCSESSION_Storage.strPlant);
            $("#ddlDept").val(QCPROCSESSION_Storage.strdept);
        }
    }
    function bindDdlDept(data)
    {
        
        const $ddlDept = $("#ddlDept");
        $ddlDept.empty();
        if (data) {
            data.forEach(item => {
                $ddlDept.append(new Option(item.Text, item.Value));
            });

        }

        if (QCPROCSESSION_Storage.hasPageLoadData === true) {
            $("#ddlDept").val(QCPROCSESSION_Storage.strdept);
        }
    }

    function bindGrid_grdDeptProc(data)
    {
        dtgrdDeptProc.clear();
   
        data.forEach((item, index) => {
            dtgrdDeptProc.row.add([
                `<span>${index + 1}</span>`,
                `<span>${item.Descrip}</span>`,
                `<span>${item.Plant}</span>`,
                `<span>
                    <a class='imgBtn' data-hd_deptid='${item.ADDEPARTMENTID}' data-hd_plantid='${item.PLANTID}'><img src='../../images/icon_edit.gif' /></a>
                </span>`,
            ]);
        });

        let pageIndex = 0;

        if (QCPROCSESSION_Storage.hasPageLoadData === true) {
             pageIndex = parseInt(QCPROCSESSION_Storage.strPgNo);
        }
        dtgrdDeptProc.draw(false);       
        dtgrdDeptProc.page(pageIndex).draw('page'); 

    

    }

    function bindTable_grdDeptProc(isChangedPlant)
    {
        var formData = new FormData();

        formData.append("plant", $("#ddlPlant").val() || '');
        formData.append("DEPTID", $("#ddlDept").val() || '');
        formData.append("isChangedPlant", isChangedPlant);

        $.ajax({
            url: '/QMS/GetgrdDeptProcList',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                var result = response.result;
                var ddlDept = response.ddlDept;

                if (isChangedPlant && ddlDept) {
                    bindDdlDept(ddlDept);
                }

                if (result) {
                    bindGrid_grdDeptProc(result);
                }
                QCPROCSESSION_Storage.hasPageLoadData = false;

            },
            complete: function () {
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseJSON?.message || 'An error occurred .');
            }
        });

    }

    $('#grdDeptProc').on('click', 'tbody .imgBtn', function (e) {
        e.preventDefault();
        const plantId = $(this).attr('data-hd_deptid');
        const deptid = $(this).attr('data-hd_plantid');

        set_QC_Proc1_QCPROCSESSION(plantId, deptid);
    });

    function set_QC_Proc1_QCPROCSESSION(hd_deptid, hd_plantid) {
        const currentPageIndex = (dtgrdDeptProc.page());
        var formData = new FormData();
        formData.append("DEPARTMENTID", hd_deptid  || '');
        formData.append("PLANTID", hd_plantid || '');
        formData.append("FILTERPLANT", $("#ddlPlant").val() || '');
        formData.append("FILTERDEPTID", $("#ddlDept").val() || '');
        // formData.append("DEPTNAME", $("#ddlPlant").val() || '');
        formData.append("GRIDPAGENO", currentPageIndex || '');

        $.ajax({
            url: '/QMS/QC_Proc1_QCPROCSESSION',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                window.location.href = "/QMS/DeptSectProc";
            },
            complete: function () {
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseJSON?.message || 'An error occurred .');
            }
        });

    }


});

