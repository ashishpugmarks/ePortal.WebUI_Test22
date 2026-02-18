$('.homeContent').removeClass('homeContent'); 

$(document).ready(function () {
    $('#errorbudget').hide();
    $("#cmdSubmit").on("click", function (e) {
        e.preventDefault();
        if (FNconfirm()) {
            cmdSubmit_Click();
        }
    });

    $("#cmdReset").on("click", function (e) {
        e.preventDefault();
        cmdReset_Click();
    });
    $("#backbtnID").on("click", function (e) {
        e.preventDefault();
        cmdBackBtn_Click();
    });
    

    $('#lnkNext').on('click', function (e) {
        e.preventDefault();
        cmdNext_Click();
    });


    $(document).on('click', '.recommendation-btn', function () {
        //$(`input[name*=rblRecomend][value="${$(this).data('status')}"]`).prop('checked', true);
        //$('input[id*=cmdSubmit]').trigger('click');
        var status = $(this).data("status");
        if (FNconfirm()) {
            cmdSubmit_Click(status);
        }
    });
    $(document).on('click', '.approval-btn', function () {
        //$(`input[name*=rblApprovalStatus][value="${$(this).data('status')}"]`).prop('checked', true);
        //$('input[id*=cmdSubmit]').trigger('click');
        var status = $(this).data("status");
        if (FNconfirm()) {
            cmdSubmit_Click(status);
        }
    });

    $('#reqDetails, #tourDetails, #prevAdvDetails, #tourDetails, #anticipatedExpenditure, #travelBudgetDetails, #approvalDetails, #approval').on('show.bs.collapse', function () {
        $(this).prev('.panel-heading').find('.caret-color').removeClass('fa-caret-right').addClass('fa-caret-down');
    });

    $('#reqDetails, #tourDetails, #prevAdvDetails, #tourDetails, #anticipatedExpenditure, #travelBudgetDetails, #approvalDetails, #approval').on('hide.bs.collapse', function () {
        $(this).prev('.panel-heading').find('.caret-color').removeClass('fa-caret-down').addClass('fa-caret-right');
    });

    setPanelWidth();
});

function FNconfirm() {
    var retval = true;
    var rowsCount = $('#gvAdvance tbody tr').length;//@((ViewBag.gvAdvanceCount ?? 0));
    if (rowsCount != '0') {
        var answer = confirm("Previous tour advance already lying with Associate ");
        return retval;
    }
    else {
        return retval;
    }
}
function cmdBackBtn_Click() {
    window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
}

function cmdReset_Click() {
    var urlParams = new URLSearchParams(window.location.search);
    var id = urlParams.get('id') || '';
    var ecode = urlParams.get('ecode') || '';
    var target = '/TourRequest/TourRequestApproval?id=' + encodeURIComponent(id) +
        '&ecode=' + encodeURIComponent(ecode);
    window.location.assign(target);
}

function cmdNext_Click() {
    var urlParams = new URLSearchParams(window.location.search);
    var id = urlParams.get('id') || '';
    var ecode = urlParams.get('ecode') || '';

    $.ajax({
        url: '/TourRequest/GetNextRequestId',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify({ id: id }),
        success: function (res) {
            var nextReqId = res.nextReqId.toString() == "" || res.nextReqId.toString() == "0" ? '' : res.nextReqId.toString();
            if (nextReqId == "") { 
                window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
            } else {
                var target = '/TourRequest/TourRequestApproval?id=' + encodeURIComponent(nextReqId) +
                    '&ecode=' + encodeURIComponent(ecode);
                window.location.assign(target);
            }
        },
        error: function (xhr, status, err) {
            console.error('AJAX error:', status, err);
            window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
        }
    });
}

function cmdSubmit_Click(appstatus = 0) {
    const urlParams = new URLSearchParams(window.location.search);
    const RequestID = urlParams.get('id') || '';
    const EmpCode = urlParams.get('ecode') || '';

    const $trSP = $('#trSP');
    const trSPVisible = $trSP.is(':visible') ? 1 : 0;

    const rblSPListEl = $('input[name="rblSPList"]:checked');
    const rblSPList = rblSPListEl.length ? rblSPListEl.val() : null;  

    const authType = $('#hfAuthType').val() || '';
    appstatus = appstatus.toString();
    let //appstatus,
        strStatus,
        remarks;
    if (authType === 'R') {
       // appstatus = $('input[name="rblRecomend"]:checked').val() || '';
        if (appstatus === '1') strStatus = 'recommended';
        else if (appstatus === '2') strStatus = 'rejected';
        else strStatus = 'Return for Modification';
        remarks = $('#txtRecRemarks').val() || '';
    } else {
        //appstatus = $('input[name="rblApprovalStatus"]:checked').val() || '';
        if (appstatus === '1') strStatus = 'approved';
        else if (appstatus === '2') strStatus = 'rejected';
        else strStatus = 'Return for Modification';
        remarks = $('#txtAPPRemarks').val() || '';
    }
     
    let appAuthCode = '';
    let spAppAuthCode = '';

    if (authType.trim() === 'R' && appstatus === '1') {
        appAuthCode = $('#cboAppAuthority').val() || '';
    } else if (authType.trim() === 'A' && rblSPList === '1' && appstatus === '1') {
        spAppAuthCode = $('#cboSPAppAuthority').val() || '';
        if (trSPVisible && spAppAuthCode === '0') {
            $('#cboSPAppAuthority').focus();
            $('#lblsplmag').text('Select Approval Auth.');
            return; 
        }
    } 
    const parseNum = (s) => {
        if (!s) return 0; 
        const t = String(s).replace(/[, ]/g, '');
        const n = parseFloat(t);
        return isNaN(n) ? 0 : n;
    };

    const intthamount = 10000;  
    const totalRequiredAmount = parseNum($('#hdntotalrequiredamount').val());
    const approxTicketAmount = parseNum($('#lblapproxticketamt').text());
    const balanceBudget = parseNum($('#lblbalancebudget').text());
    const dir2EmpCode = $('#hfDir2EmpCode').val() || '';
    const reqOperationId = $('#hdnReqoperationid').val() || '';

    //const payload = {
    //    RequestID,
    //    EmpCode,
    //    authType,
    //    appstatus,
    //    strStatus,
    //    remarks,
    //    appAuthCode,
    //    spAppAuthCode,
    //    intthamount,
    //    totalRequiredAmount,
    //    approxTicketAmount,
    //    balanceBudget,
    //    dir2EmpCode,
    //    reqOperationId,
    //    rblSPList,
    //    trSPVisible
    //};
    const payload = {
        RequestID: RequestID,
        EmpCode: EmpCode,
        authType: authType,
        appstatus: appstatus,
        strStatus: strStatus,
        remarks: remarks,

        appAuthCode: appAuthCode,
        spAppAuthCode: spAppAuthCode,

        intthamount: intthamount.toString(),
        totalRequiredAmount: totalRequiredAmount.toString(),
        approxTicketAmount: approxTicketAmount.toString(),
        balanceBudget: balanceBudget.toString(),

        dir2EmpCode: dir2EmpCode,
        reqOperationId: reqOperationId,

        rblSPList: rblSPList,
        trSPVisible: trSPVisible.toString()
    };
    console.log(JSON.stringify(payload));
    $.ajax({
        url: '/TourRequest/CmdSubmit_Click_TRA',
        type: 'POST',
        data: JSON.stringify(payload),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        beforeSend: function () {
            $('#lblMsg').show().text('Processing...');
            $('#errorbudget').hide();
            $('#lblbudget').text('');
            $('#trbalancebudget').removeAttr('bgcolor');
        },
        success: function (data) {
            if (!data) {
                $('#lblMsg').text('Unexpected empty response.');
                return;
            }

            if (data.error && data.error.trim() !== '') {
                $('#lblMsg').text(data.error);
                return;
            }

            if (data.budgetError && data.budgetError.trim() !== '') {
                $('#errorbudget').show();
                $('#lblbudget').text(data.budgetError);
                $('#trbalancebudget').attr('bgcolor', 'Red');
                $('#lblMsg').text('');
                return;
            }

            // Navigation contract: server returns nextReqId or redirectToManage
            if (data.redirectToManage) {
                window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
            } else if (data.nextReqId && data.nextReqId !== '') {
                const target = 'TourRequestApproval?id=' + encodeURIComponent(data.nextReqId)
                    + '&ecode=' + encodeURIComponent(EmpCode);
                window.location.assign(target);
            } else {
                window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
            }
        },
        error: function (xhr, status, err) {
            console.error('AJAX error:', status, err);
            $('#lblMsg').text('Error occurred while submitting. Please try again.');
        }
    });
}

$(window).resize(function () {
    setPanelWidth();
});

function setPanelWidth() {
    var btnApproveClientWidth = $('.pnl-btn .approve').prop('clientWidth');
    if (window.innerWidth < 1024) { // Mobile layout
        $('.last-row-btn .btn').css({
            'maxWidth': `auto`
        });
    }
    else {
        $('.last-row-btn .btn').css({
            'maxWidth': `${btnApproveClientWidth}px`
        });
    }
}