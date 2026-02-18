$('.homeContent').removeClass('homeContent'); 

$(document).ready(function () {
    $("#btnSave").on("click", function (e) {
        e.preventDefault(); 
        btnSave_Click(); 
    });

    $("#btnBack").on("click", function (e) {
        e.preventDefault();
        btnBack_Click();
    });
}); 
function btnBack_Click() {
    window.location.href = '/TourRequest/TourRequestList';
}

function btnSave_Click() {
    const payload = {
        Tid: tid,
        Remarks: $('#txtRemarks').val(),
        NewRecAuthority: $('#cboRecAuthority').val(),
        NewAppAuthority: $('#cboAppAuthority').val(),
        NewSpAppAuthority: $('#cboSpAppAuthority').val()
    };
    $.ajax({
        url: '/TourRequest/BtnSave_Click',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify({ payload }),
        success: function (res) {
            if (!res || res.ok !== true) {
                $('#errorpanel').show();
                $('#status').text((data && data.message) || 'An error occurred.');
                return;
            }
            window.location.href = '/TourRequest/TourRequestList'; 

        },
        error: function (xhr, status, err) {
            console.error('AJAX error:', status, err);
        }
    });
}

function cmdSubmit_Click() {
    var urlParams = new URLSearchParams(window.location.search);
    var requestId = urlParams.get('id') || '';
    var empCode = urlParams.get('ecode') || '';

    var $trSP = $('#trSP');
    if ($trSP.is(':visible')) {
        var spAuthVal = $('#cboSPAppAuthority').val();
        if (spAuthVal === '0') {
            $('#cboSPAppAuthority').focus();
            $('#lblsplmag').text('Select Approval Auth.');
            return;
        }
    }

    var authType = $('#hfAuthType').val();

    var appstatus, strStatus, remarks;
    if (authType === 'R') {
        appstatus = $('input[name="rblRecomend"]:checked').val();
        if (appstatus === '1') strStatus = 'recommended';
        else if (appstatus === '2') strStatus = 'rejected';
        else strStatus = 'Return for Modification';

        remarks = $('#txtRecRemarks').val();
    } else {
        appstatus = $('input[name="rblApprovalStatus"]:checked').val();
        if (appstatus === '1') strStatus = 'approved';
        else if (appstatus === '2') strStatus = 'rejected';
        else strStatus = 'Return for Modification';

        remarks = $('#txtAPPRemarks').val();
    }

    var appAuthCode = '';
    var spAppAuthCode = '';
    if (authType.trim() === 'R' && appstatus === '1') {
        appAuthCode = $('#cboAppAuthority').val() || '';
    }
    else if (authType.trim() === 'A' && $('input[name="rblSPList"]:checked').val() === '1' && appstatus === '1') {
        spAppAuthCode = $('#cboSPAppAuthority').val() || '';
    }

    var intthamount = 10000;
    var totalRequired = parseFloat($('#hdntotalrequiredamount').val() || '0');
    var approxTicket = parseFloat(($('#lblapproxticketamt').text() || '0').replace(/[, ]/g, ''));
    var balanceBudget = parseFloat(($('#lblbalancebudget').text() || '0').replace(/[, ]/g, ''));


    var dir2EmpCode = $('#hfDir2EmpCode').val() || '';
    var reqOperationId = $('#hdnReqoperationid').val() || '';

    var data = {
        requestId: requestId,
        empCode: empCode,
        authType: authType,
        appstatus: appstatus,
        strStatus: strStatus,
        remarks: remarks,
        appAuthCode: appAuthCode,
        spAppAuthCode: spAppAuthCode,
        intthamount: intthamount,
        totalRequiredAmount: totalRequired,
        approxTicketAmount: approxTicket,
        balanceBudget: balanceBudget,
        dir2EmpCode: dir2EmpCode,
        reqOperationId: reqOperationId
    };

    $.ajax({
        url: '/TourRequest/CmdSubmit_Click_TRA',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () {
            $('#lblMsg').show().text('Processing...');
        },
        success: function (res) {
            var data = res && res.d ? res.d : res;
            if (!data) {
                $('#lblMsg').text('Unexpected empty response.');
                return;
            }
            if (data.error && data.error.trim() !== '') {
                $('#lblMsg').text(data.error);
                return;
            }

            if (data.budgetError && data.budgetError !== '') {
                $('#errorbudget').show();
                $('#lblbudget').text(data.budgetError);
                $('#trbalancebudget').attr('bgcolor', 'Red');
                $('#lblMsg').text('');
                return;
            } else {
                $('#errorbudget').hide();
                $('#lblbudget').text('');
                $('#trbalancebudget').removeAttr('bgcolor');
            }

            if (data.redirectToManage) {
                window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
            } else if (data.nextReqId && data.nextReqId !== '') {
                var target = 'TourRequestApproval.aspx?id=' + encodeURIComponent(data.nextReqId) +
                    '&ecode=' + encodeURIComponent(empCode);
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