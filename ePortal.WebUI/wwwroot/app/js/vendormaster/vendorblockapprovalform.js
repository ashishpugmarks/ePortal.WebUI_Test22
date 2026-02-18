window.addEventListener('DOMContentLoaded', () => {
    for (let sheet of document.styleSheets) {
        try {
            for (let i = 0; i < sheet.cssRules.length; i++) {
                let rule = sheet.cssRules[i];
                if (rule.selectorText === '.homeContent div') {
                    rule.style.removeProperty('padding');
                }
                if (rule.selectorText === '.homeContent') {
                    rule.style.removeProperty('padding');
                }
            }
        } catch (e) {
            console.warn("Could not access stylesheet:", e);
        }
    }
});

document.addEventListener('DOMContentLoaded', function () {
    $('#btnsubmit').on('click', function (e) {
        //e.preventDefault();
        
        var model = {
            VendorHeaderId: $("#hdVMID").val(),
            Status: $("#ddlstatus").val(),
            Remarks: $("#txt_Remarks").val(),
            hdPosting: $("#HDPOSTINGID").val(),
            RequestorName: $("#lblemployee").text(),
            RequestorEmail: $("#HDREQEMAILID").val(),
            RequestTypeDesc: $("#lblrequesttype").text(),
            PurchasingStatus: $("#lblpurchasing").text(),
            PostingId: $("#lblposting").text(),
            RequestDate: $("#lblrequestdate").text()
        };

        // client-side quick validations using SweetAlert (v1)
        if (!model.Status) {
            swal({ title: "Validation", text: "Status is mandatory field", type: "warning" });
            return;
        }
        if (!model.Remarks) {
            swal({ title: "Validation", text: "Remarks is mandatory field", type: "warning" });
            return;
        }

        $.ajax({
            url: '/VendorMaster/ApproveVendorBlock',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(model),
            success: function (res) {
                if (res.success) {
                    // show success and redirect on confirm
                    swal({
                        title: "Success",
                        text: res.message,
                        type: "success"
                    }, function () {
                        if (res.redirect) {
                            window.location.href = res.redirect;
                        }
                    });
                } else {
                    // show error
                    swal({ title: "Error", text: res.message, type: "error" });
                }
            },
            error: function () {
                swal({ title: "Error", text: "An error occurred while processing your request.", type: "error" });
            }
        });
    });

    $('#txt_Remarks').on('keypress', function (e) {
        if ($(this).val().length >= 499) {
            e.preventDefault(); // block further typing
        }
    });

    // Prevent pasting into the textarea
    $('#txt_Remarks').on('paste', function (e) {
        e.preventDefault();
    });
    $('#btncancel').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageBlockApprovalRequest";
    });
    $('#btnbackimg, #btnbackspan').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageBlockApprovalRequest";
    });
});