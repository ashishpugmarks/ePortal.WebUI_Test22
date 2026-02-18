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
    var form = document.getElementById('vendorForm');
    var rdoMass = document.getElementById('rdomassvendor');
    var trVendorCode = document.getElementById('tr_vendorcode');
    var trUpload = document.getElementById('tr_upload');
    var btnContinue = document.getElementById('btncontinuetonext');
    //var btnCancel = document.getElementById('btncancel');
    var btnSubmit = document.getElementById('btnsubmit');
    //var btnCancelForm = document.getElementById('btncancelfrm');
    var fileInput = document.getElementById('fileuploader');
    var vendorCodeInput = document.getElementById('txtvendorcode');

    function updateVisibility() {
        var isMass = !!(rdoMass && rdoMass.checked);
        if (trVendorCode) trVendorCode.style.display = isMass ? 'none' : '';
        if (trUpload) trUpload.style.display = isMass ? '' : 'none';
    }

    function GetVendorDetails() {
        debugger;
        var response = $('#vendorPayload').data('payload') || {};

        if (!response) {
            alert('Failed to load vendor block details.');
            return;
        }

        $('#HDVENDORBLOCKHID').val(response.VendorHeaderId || '');
        // read values from response (all strings)
        var strreqtype = response.RequestType || '';
        var strreqcat = response.RequestCategoryDesc || '';
        var strpurc = response.BlockPurchasing || '';
        var strposting = response.BlockPosting || '';
        var processStatus = response.ProcessStatus || '';

        // radio buttons and visibility for vendor type
        if (strreqtype === "1") {
            $('#rdosinglevendor').prop('checked', true);
            $('#rdomassvendor').prop('checked', false);
            $('#tr_vendorcode').show();
            $('#tr_upload').hide();
        } else {
            $('#rdosinglevendor').prop('checked', false);
            $('#rdomassvendor').prop('checked', true);
            $('#tr_vendorcode').hide();
            $('#tr_upload').show();
        }

        // request category radios
        if (strreqcat.toLowerCase() === 'block') {
            $('#rdoblock').prop('checked', true);
            $('#rdounblock').prop('checked', false);
        } else {
            $('#rdoblock').prop('checked', false);
            $('#rdounblock').prop('checked', true);
        }

        // checkboxes
        $('#chkpurchblock').prop('checked', strpurc === "1");
        $('#chkpostingblock').prop('checked', strposting === "1");

        if (response.SendBackRemark) {
            $('#sendbackremark').html(response.SendBackRemark);
            $('#sendback_div').show();
        } else {
            $('#sendback_div').hide();
        }

        populateDeptDropdown(response.DepartmentHeads || []);
    }
    function populateDeptDropdown(list) {
        var $ddl = $('#ddlforward');
        $ddl.empty();
        list.forEach(function (i) {
            $ddl.append($('<option>').val(i.Value).text(i.Text));
        });
    }
    function continueHandler(e) {
        debugger;
        e && e.preventDefault();
        var form = document.getElementById("vendorForm");
        var formData = new FormData(form);

        // RequestType and RequestCategory are enums on server. Send matching names or numeric values.
        var rt = $("input[name='RequestType']:checked").val();
        var rc = $("input[name='Categorytype']:checked").val();
        formData.set("RequestType", rt === "SingleVendor" ? "SingleVendor" : "MassVendor");
        formData.set("RequestCategory", rc === "Block" ? "Block" : "Unblock");
        formData.set("PurchasingData", $("#chkpurchblock").is(":checked"));
        formData.set("PostingData", $("#chkpostingblock").is(":checked"));
        formData.set("VendorCode", $("#txtvendorcode").val());

        $.ajax({
            url: "/VendorMaster/ContinueToNext",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            success: function (res) {
                // Show alert messages using SweetAlert
                if (res.ErrorFlag) {
                    swal("Error", res.Message, "error");
                }

                // Apply labels
                $("#lblrequesttype").text(res.RequestTypeText || "");
                $("#lblpurchasing").text(res.LabelPurchasing || "");
                $("#lblposting").text(res.LabelPosting || "");

                // Apply visibility
                $("#TableVendorfrmdetails").toggle(!!res.ShowDetails);
                $("#TableVendorfrm1").toggle(!!res.ShowForm);
                $("#TableVendorfrm2").toggle(!!res.ShowForm);
                $("#TableVendorfrm3").toggle(!!res.ShowForm);

                // Error div
                $("#ERRMSGDIV").toggle(!!res.ErrMsgDivVisible).html(res.ErrMsgDivHtml || "");

                // Hidden vendor code list
                $("#HDVENDORCODELIST").val(res.VendorCodeList || "");

                // Bind vendor list to table
                var $tbody = $("#REPVENDORNAME tbody");
                $tbody.empty();

                if (res.VendorList && res.VendorList.length) {
                    res.VendorList.forEach(function (v) {
                        $tbody.append(`
                            <tr>
                                <td>${v.VendorCode || ""}</td>
                                <td>${v.VendorName || ""}</td>
                                <td>${v.City || ""}</td>
                                <td>${v.Region || ""}</td>
                            </tr>
                        `);
                    });
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function (xhr) {
                swal("Request Failed", "Error: " + xhr.statusText, "error");
            }
        });
    }

    function updateVendorBlock() {
        debugger;
        var rt = $("input[name='RequestType']:checked").val();
        var rc = $("input[name='Categorytype']:checked").val();
        var model = {
            RequestType: rt === "SingleVendor" ? 0 : 1,
            RequestCategory: rc === "Block" ? 0 : 1,
            PurchasingData: $("#chkpurchblock").is(":checked"),
            PostingData: $("#chkpostingblock").is(":checked"),
            VendorCode: $("#txtvendorcode").val(),
            VendorCodeList: $("#HDVENDORCODELIST").val(),
            Remarks: $("#txtvendorremarks").val(),
            ForwardTo: $("#ddlforward").val(),
            VendorHeaderId: $("#HDVENDORBLOCKHID").val()
        };

        $.ajax({
            url: '/VendorMaster/UpdateVendorBlock',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(model),
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            success: function (response) {
                if (response.ErrorFlag) {
                    swal({
                        title: "Error",
                        text: response.Message,
                        type: "error",
                        confirmButtonText: "OK"
                    });
                } else {
                    swal({
                        title: "Success",
                        text: response.Message,
                        type: "success",
                        confirmButtonText: "OK"
                    }, function () {
                        // Redirect after user clicks OK
                        window.location.href = '/VendorMaster/ManageBlockRequest';
                    });
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                swal({
                    title: "Error",
                    text: "Something went wrong while submitting request.",
                    type: "error",
                    confirmButtonText: "OK"
                });
            }
        });
    }

    //function cancelHandler(e) {
    //    e && e.preventDefault();
    //    var cancelUrl = form && form.dataset && form.dataset.cancelUrl;
    //    if (cancelUrl) window.location.href = cancelUrl;
    //    else window.history.back();
    //}

    $('#btncancelfrm').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageBlockRequest";
    });
    $('#btncancel').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageBlockRequest";
    });
    $('#btnbackimg, #btnbackspan').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageBlockRequest";
    });

    var requestTypeRadios = document.querySelectorAll('input[name="RequestType"]');
    requestTypeRadios.forEach(function (r) { r.addEventListener('change', updateVisibility); });

    btnContinue && btnContinue.addEventListener('click', continueHandler);
    //btnCancel && btnCancel.addEventListener('click', cancelHandler);
    btnSubmit && btnSubmit.addEventListener('click', updateVendorBlock);
    //btnCancelForm && btnCancelForm.addEventListener('click', cancelHandler);

    updateVisibility();
    GetVendorDetails();
});