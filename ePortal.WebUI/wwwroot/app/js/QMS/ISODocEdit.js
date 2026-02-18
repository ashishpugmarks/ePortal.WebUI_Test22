$('.homeContent').removeClass('homeContent');
$(document).ready(function () {

    $('.onpaste').on('paste', function (e) {
        e.preventDefault();
    });

    $('.numeric').on('keypress', function (e) {
        if (!/[0-9]/.test(e.key)) {
            e.preventDefault();
        }
    });


    $(document).on('keypress', '.alphanumeric', function (e) {
        if (!/[a-zA-Z0-9]/.test(e.key)) {
            e.preventDefault();
        }
    });


    $('.letters').on('keypress', function (e) {
        
        if (/[^a-zA-Z]/.test(e.key)) {
            e.preventDefault();
        }

    });

    
    // txtDocNo1_TextChanged();

    $('#btnSubmit').on('click', function (e) {
        e.preventDefault();

        if (fnValidateDate()) {
            ISODocSave();
        }

    });

    $('#btnCancel').on('click', function (e) {
        e.preventDefault();
        const redirectionTo = '/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx#QMS'; //update url
        window.location.href = redirectionTo;

        
    });

    $('#ddlformatnumber').on('change', function (e) {
        e.preventDefault();
        ddlformatnumber_TextChanged();
    });

    $('#txtDocNo9').on('change', function () {
        txtDocNo9_TextChanged();
    });

    $('#txtDocNo').on('change', function () {
        txtDocNo_TextChanged();
    });

    $('#txtDocNo1').on('change', function () {
        txtDocNo1_TextChanged();

    });

    $('#txtDocNo2').on('change', function () {
        txtDocNo2_TextChanged();
    });
    

    $('.cclick').on('change', function (e) {
        e.preventDefault();
        cclick();
    });

    $('.cclickRI').on('change', function (e) {
        e.preventDefault();
        cclickRI();
    });

    $('.cclickR').on('change', function (e) {
        e.preventDefault();
        cclickR();
    });

    $('.cclickD').on('change', function (e) {
        e.preventDefault();
        cclickD();
    });

    $('#FileList').on('click', function (e) {
        e.preventDefault();

        const filename = $(this).attr('data-filename');
        const url = "../../Uploads/ISODoc/" + filename;
        window.open(url, 'ApprovalNote', 'left=150,top=20,width=800,height=600,toolbar=1,resizable=0');

    });

    $('input[name="changeType_rdomaster"]').on('change', function () {
        const checked_id = this.id;
        if (checked_id === "rbtnAdd") {
            rbtnAdd_CheckedChanged();
        } else if (checked_id === "rbtnRev") {
            rbtnRev_CheckedChanged();
        } else if (checked_id === "rbtnDel") {
            rbtnDel_CheckedChanged();
        }
    });
    pageload_controls();

    //--------------------------------------- Methods -----------------------------------------

    function pageload_controls() {
        var checked_id = $('input[name="changeType_rdomaster"]:checked').attr('id');
        if (checked_id === "rbtnAdd")
        {
            rbtnAdd_CheckedChanged();
        }
        else if (checked_id === "rbtnRev")
        {
            rbtnRev_CheckedChanged();
        }
        else if (checked_id === "rbtnDel")
        {
            rbtnDel_CheckedChanged();
        }
    }
    function txtDocNo1_TextChanged() {
        const txtDocNo1 = $("#txtDocNo1").val();
        $("#txtRevNoA").val(txtDocNo1);

        BindTextChanged_ISODocEdit();
        $("#txtDocNo2").focus();

    }

    function txtDocNo9_TextChanged() {

        BindTextChanged_ISODocEdit();
        $("#txtDocNo").focus();
    }

    function txtDocNo_TextChanged() {
        BindTextChanged_ISODocEdit();

        $("#txtDocNo1").focus();
    }

    function txtDocNo2_TextChanged() {
        BindTextChanged_ISODocEdit();
        $("#txtDocTitle").focus();

    }

    function ddlformatnumber_TextChanged() {

        BindTextChanged_ISODocEdit();
        const txtDocNo1 = $("#txtDocNo1").val();
        $("#txtRevNoA").val(txtDocNo1);
      
        $("#txtDocNo9").focus();

    }

    //Check if the date exist for Addition
    function cclick() {
        for (var i = 0; i < document.getElementById("ddlMonthA").options.length; i++) {
            if (document.getElementById("ddlMonthA").options[i].selected == true) {
                if (document.getElementById("ddlMonthA").options[i].value == 2) {
                    for (var j = 0; j < document.getElementById("ddlDayA").options.length; j++) {
                        if (document.getElementById("ddlDayA").options[j].selected == true) {
                            if (document.getElementById("ddlDayA").options[j].value > 28) {
                                if (document.getElementById("ddlDayA").options[j].value == 29 & (document.getElementById("ddlYearA").options.value == 2008 || document.getElementById("ddlYearA").options.value == 2012 || document.getElementById("ddlYearA").options.value == 2016 || document.getElementById("ddlYearA").options.value == 2020 || document.getElementById("ddlYearA").options.value == 2024 || document.getElementById("ddlYearA").options.value == 2028)) {
                                }
                                else {
                                    document.getElementById("ddlDayA").options[27].selected = true;
                                }
                            }
                        }
                    }
                }
                if (document.getElementById("ddlMonthA").options[i].value == 4 || document.getElementById("ddlMonthA").options[i].value == 6 || document.getElementById("ddlMonthA").options[i].value == 9 || document.getElementById("ddlMonthA").options[i].value == 11) {
                    for (var j = 0; j < document.getElementById("ddlDayA").options.length; j++) {
                        if (document.getElementById("ddlDayA").options[j].selected == true) {
                            if (document.getElementById("ddlDayA").options[j].value > 30) {
                                document.getElementById("ddlDayA").options[29].selected = true;
                            }
                        }
                    }
                }
            }
        }
    }

    //Check if the date exist for Revision
    function cclickR() {
        for (var i = 0; i < document.getElementById("ddlImpMonthR").options.length; i++) {
            if (document.getElementById("ddlImpMonthR").options[i].selected == true) {
                if (document.getElementById("ddlImpMonthR").options[i].value == 2) {
                    for (var j = 0; j < document.getElementById("ddlImpDayR").options.length; j++) {
                        if (document.getElementById("ddlImpDayR").options[j].selected == true) {
                            if (document.getElementById("ddlImpDayR").options[j].value > 28) {
                                if (document.getElementById("ddlImpDayR").options[j].value == 29 & (document.getElementById("ddlImpYearR").options.value == 2008 || document.getElementById("ddlImpYearR").options.value == 2012 || document.getElementById("ddlImpYearR").options.value == 2016 || document.getElementById("ddlImpYearR").options.value == 2020 || document.getElementById("ddlImpYearR").options.value == 2024 || document.getElementById("ddlImpYearR").options.value == 2028)) {
                                }
                                else {
                                    document.getElementById("ddlImpDayR").options[27].selected = true;
                                }
                            }
                        }
                    }
                }
                if (document.getElementById("ddlImpMonthR").options[i].value == 4 || document.getElementById("ddlImpMonthR").options[i].value == 6 || document.getElementById("ddlImpMonthR").options[i].value == 9 || document.getElementById("ddlImpMonthR").options[i].value == 11) {
                    for (var j = 0; j < document.getElementById("ddlImpDayR").options.length; j++) {
                        if (document.getElementById("ddlImpDayR").options[j].selected == true) {
                            if (document.getElementById("ddlImpDayR").options[j].value > 30) {
                                document.getElementById("ddlImpDayR").options[29].selected = true;
                            }
                        }
                    }
                }
            }
        }
    }
    //Check if the date exist for Revision implementation
    function cclickRI() {
        for (var i = 0; i < document.getElementById("ddlRevMonthR").options.length; i++) {
            if (document.getElementById("ddlRevMonthR").options[i].selected == true) {
                if (document.getElementById("ddlRevMonthR").options[i].value == 2) {
                    for (var j = 0; j < document.getElementById("ddlRevDayR").options.length; j++) {
                        if (document.getElementById("ddlRevDayR").options[j].selected == true) {
                            if (document.getElementById("ddlRevDayR").options[j].value > 28) {
                                if (document.getElementById("ddlRevDayR").options[j].value == 29 & (document.getElementById("ddlRevYearR").options.value == 2008 || document.getElementById("ddlRevYearR").options.value == 2012 || document.getElementById("ddlRevYearR").options.value == 2016 || document.getElementById("ddlRevYearR").options.value == 2020 || document.getElementById("ddlRevYearR").options.value == 2024 || document.getElementById("ddlRevYearR").options.value == 2028)) {
                                }
                                else {
                                    document.getElementById("ddlRevDayR").options[27].selected = true;
                                }
                            }
                        }
                    }
                }
                if (document.getElementById("ddlRevMonthR").options[i].value == 4 || document.getElementById("ddlRevMonthR").options[i].value == 6 || document.getElementById("ddlRevMonthR").options[i].value == 9 || document.getElementById("ddlRevMonthR").options[i].value == 11) {
                    for (var j = 0; j < document.getElementById("ddlRevDayR").options.length; j++) {
                        if (document.getElementById("ddlRevDayR").options[j].selected == true) {
                            if (document.getElementById("ddlRevDayR").options[j].value > 30) {
                                document.getElementById("ddlRevDayR").options[29].selected = true;
                            }
                        }
                    }
                }
            }
        }
    }

    //Check if the date exist
    function cclickD() {
        for (var i = 0; i < document.getElementById("ddlMonthD").options.length; i++) {
            if (document.getElementById("ddlMonthD").options[i].selected == true) {
                if (document.getElementById("ddlMonthD").options[i].value == 2) {
                    for (var j = 0; j < document.getElementById("ddlDayD").options.length; j++) {
                        if (document.getElementById("ddlDayD").options[j].selected == true) {
                            if (document.getElementById("ddlDayD").options[j].value > 28) {
                                if (document.getElementById("ddlDayD").options[j].value == 29 & (document.getElementById("ddlYearD").options.value == 2008 || document.getElementById("ddlYearD").options.value == 2012 || document.getElementById("ddlYearD").options.value == 2016 || document.getElementById("ddlYearD").options.value == 2020 || document.getElementById("ddlYearD").options.value == 2024 || document.getElementById("ddlYearD").options.value == 2028)) {
                                }
                                else {
                                    document.getElementById("ddlDayD").options[27].selected = true;
                                }
                            }
                        }
                    }
                }
                if (document.getElementById("ddlMonthD").options[i].value == 4 || document.getElementById("ddlMonthD").options[i].value == 6 || document.getElementById("ddlMonthD").options[i].value == 9 || document.getElementById("ddlMonthD").options[i].value == 11) {
                    for (var j = 0; j < document.getElementById("ddlDayD").options.length; j++) {
                        if (document.getElementById("ddlDayD").options[j].selected == true) {
                            if (document.getElementById("ddlDayD").options[j].value > 30) {
                                document.getElementById("ddlDayD").options[29].selected = true;
                            }
                        }
                    }
                }
            }
        }
    }

    function fnValidateDate() {
        var errorMsg = "";
        var returnStatus = true;

        if (document.getElementById("ddlPlant").value == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Plant is required field.";
            document.getElementById("ddlPlant").focus();
            returnStatus = false;
            return false;
        }

        if (document.getElementById("ddlDept").value == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Department is required field.";
            document.getElementById("ddlDept").focus();
            returnStatus = false;
            return false;
        }
            
        if (document.getElementById("rbtnDel").checked == true) {
            if (document.getElementById("txtReason").value.trim().length < 10) {
                document.getElementById("errorpanel").style.display = "inline";
                document.getElementById("status").innerHTML = "Specify reason in detail.";
                document.getElementById("txtReason").focus();
                returnStatus = false;
                return false;
            }
        }

        
        else if (document.getElementById("rbtnRev").checked == true) {

              
            var day1 = document.getElementById("ddlImpDayR").value;
            var month1 = document.getElementById("ddlImpMonthR").value;
            var year1 = document.getElementById("ddlImpYearR").value;

            var year2 = document.getElementById("ddlRevYearR").value;
            var month2 = document.getElementById("ddlRevMonthR").value;
            var day2 = document.getElementById("ddlRevDayR").value;

            var fromDate = new Date(year1, month1, day1);
            var toDate = new Date(year2, month2, day2);

            msPerDay = 24 * 60 * 60 * 1000;
            dbd = Math.floor((toDate - fromDate) / msPerDay);

            if (dbd > 0) {
                document.getElementById("errorpanel").style.display = "inline";
                document.getElementById("status").innerHTML = "Implementation date should be greater than Revision date.";
                document.getElementById("ddlImpDayR").focus();
                returnStatus = false;
                return false;
            }
        }
        

        if (document.getElementById("txtReason").value == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Reason is required field.";
            document.getElementById("txtReason").focus();
            returnStatus = false;
            return false;
        }

        if (document.getElementById("txtFileUpload").value == "") {
            if (document.getElementById("hdnFile").value == "") {
                document.getElementById("errorpanel").style.display = "inline";
                document.getElementById("status").innerHTML = "Attachement is required field.";
                document.getElementById("txtReason").focus();
                returnStatus = false;
                return false;
            }
        }
        else {
            var value = document.getElementById("txtFileUpload").value;
            ext = value.split(".").pop();
            ext = ext.toLowerCase();
            if (ext !== "pdf") {
                document.getElementById("errorpanel").style.display = "inline";
                document.getElementById("status").innerHTML = "Please Select only Pdf File.";
                document.getElementById("txtReason").focus();
                returnStatus = false;
                return false;
            }
        }
        if (returnStatus == false) {
            return false;
        }
        else {

            return true;
        }
    }

    function BindTextChanged_ISODocEdit() {

        var formData = new FormData();

        formData.append("ddlformatnumber", $("#ddlformatnumber").val() || '');
        formData.append("txtDocNo9", $("#txtDocNo9").val() || '');
        formData.append("txtDocNo", $("#txtDocNo").val() || '');
        formData.append("txtDocNo1", $("#txtDocNo1").val() || '');
        formData.append("txtDocNo2", $("#txtDocNo2").val() || '');
        formData.append("IsChecked_rbtnAdd", $('#rbtnAdd').is(':checked'));
        formData.append("IsChecked_rbtnRev", $('#rbtnRev').is(':checked'));
        formData.append("IsChecked_rbtnDel", $('#rbtnDel').is(':checked'));

        $.ajax({
            url: '/QMS/BindTextChanged_ISODocEdit',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                var result = response.result;
                if (result && Object.keys(result).length !== 0)
                {
                    BindTextChangedData(result);
                }
            },
            complete: function () {
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseJSON?.message || 'An error occurred .');
            }
        });

    }

    function BindTextChangedData(obj) {

        debugger
        console.log(obj);

        if (obj && Object.keys(obj).length !== 0) {

            // Start
            if (obj.hasOwnProperty("IsWindowAlert") && obj.IsWindowAlert === true) {
                alert(obj.message);
            }  
            if (obj.hasOwnProperty("IsoDocId")) {
                $("#IsoDocId").val(obj.IsoDocId);
            }  

            if (obj.hasOwnProperty("ddlformatnumber")) {
                $("#ddlformatnumber").val(obj.ddlformatnumber);
            }  
            if (obj.hasOwnProperty("hdnNewRevNoR")) {
                $("#hdnNewRevNoR").val(obj.hdnNewRevNoR);

            }  
            if (obj.hasOwnProperty("txtDocNo")) {
                $("#txtDocNo").val(obj.txtDocNo);

            }  
            if (obj.hasOwnProperty("txtDocNo1")) {
                $("#txtDocNo1").val(obj.txtDocNo1);

            }  
            if (obj.hasOwnProperty("txtDocNo2")) {
                $("#txtDocNo2").val(obj.txtDocNo2);

            }  
            if (obj.hasOwnProperty("txtDocNo9")) {
                $("#txtDocNo9").val(obj.txtDocNo9);

            }  
            if (obj.hasOwnProperty("txtDocTitle")) {
                $("#txtDocTitle").val(obj.txtDocTitle);

            }  
            if (obj.hasOwnProperty("txtDocTitleReadOnly")) {
                $('#txtDocTitle').prop('readonly', obj.txtDocTitleReadOnly);

            }  
            if (obj.hasOwnProperty("txtNewRevNoR")) {
                $("#txtNewRevNoR").val(obj.txtNewRevNoR);

            }  
            if (obj.hasOwnProperty("txtRevNoA")) {
                $("#txtRevNoA").val(obj.txtRevNoA);
            }  

            if (obj.hasOwnProperty("txtCurrRevNoR")) {
                $("#txtCurrRevNoR").val(obj.txtCurrRevNoR);
            }  
            if (obj.hasOwnProperty("hdnCurrRevNoR")) {
                $("#hdnCurrRevNoR").val(obj.hdnCurrRevNoR);
            }  
           
            if (obj.hasOwnProperty("ddlRevDayR")) {
                $("#ddlRevDayR").val(obj.ddlRevDayR);
            }  
            if (obj.hasOwnProperty("ddlRevMonthR")) {
                $("#ddlRevMonthR").val(obj.ddlRevMonthR);
            }  
            if (obj.hasOwnProperty("ddlRevYearR")) {
                $("#ddlRevYearR").val(obj.ddlRevYearR);
            }  
            if (obj.hasOwnProperty("ddlRevDayREnabled")) {
                $('#ddlRevDayR').prop('disabled', !obj.ddlRevDayREnabled);
            }  
            if (obj.hasOwnProperty("ddlRevMonthREnabled")) {
                $('#ddlRevMonthR').prop('disabled', !obj.ddlRevMonthREnabled);
            }  
            if (obj.hasOwnProperty("ddlRevYearREnabled")) {
                $('#ddlRevYearR').prop('disabled', !obj.ddlRevYearREnabled);
            }  
            if (obj.hasOwnProperty("txtCurrRevD")) {
                $("#txtCurrRevD").val(obj.txtCurrRevD);
            }  
            
            if (obj.hasOwnProperty("hdnDocTitle")) {
                $("#hdnDocTitle").val(obj.hdnDocTitle);
            }  
            if (obj.hasOwnProperty("ddlDept")) {
                $("#ddlDept").val(obj.ddlDept);
            }  
            if (obj.hasOwnProperty("ddlPlant")) {
                $("#ddlPlant").val(obj.ddlPlant);
            }  
            if (obj.hasOwnProperty("ddlSection")) {
                $("#ddlSection").val(obj.ddlSection);
            }  
            if (obj.hasOwnProperty("ddlRevYearRSelectedIndex")) {

                const idx = parseInt(obj.ddlRevYearRSelectedIndex, 10);
                const $ddl = $('#ddlRevYearR');
                if (!Number.isNaN(idx) && idx >= 0 && idx < $ddl.find('option').length) {
                    $ddl.prop('selectedIndex', idx).trigger('change');
                }
            }  

            if (obj.hasOwnProperty("FilenameExist"))
            {
                $("#hdnFile").val(obj.FilenameExist);
                const hdnFile = obj.hasOwnProperty("hdnFile") === true ? obj.hdnFile : "";
                $("#hdnFile").val(hdnFile);

                const $fileList = $('#FileList');
                if ($fileList.length) {

                    if (obj.hasOwnProperty("FileListVisible") && obj.FileListVisible) {
                        $fileList.css('display', 'inline');
                        $fileList.attr('data-docid', obj.FilenameExist);

                    } else {
                        $fileList.css('display', 'none');

                    }
                }
            }
        }
       
        
    }

    function rbtnAdd_CheckedChanged() {

        $('#pnlAdditon').css('display', '');
        $('#pnlDeletion').css('display', 'none');
        $('#pnlRevison').css('display', 'none');
        $('#txtDocTitle').prop('readonly', false);
    }

    function rbtnRev_CheckedChanged() {
        $('#pnlAdditon').css('display', 'none');
        $('#pnlDeletion').css('display', 'none');
        $('#pnlRevison').css('display', '');
        $('#txtDocTitle').prop('readonly', true);


        $('#txtCurrRevNoR').prop('readonly', true);
        $('#ddlRevDayR').prop('disabled', true);
        $('#ddlRevMonthR').prop('disabled', true);
        $('#ddlRevYearR').prop('disabled', true);

    }

    function rbtnDel_CheckedChanged() {

        $('#pnlAdditon').css('display', 'none');
        $('#pnlDeletion').css('display', '');
        $('#pnlRevison').css('display', 'none');
        $('#txtDocTitle').prop('readonly', true);
        $('#txtCurrRevD').prop('readonly', true);

    }

    function ISODocSave() {
        var formData = new FormData();
        
        formData.append("ddlPlant", $("#ddlPlant").val() || '');
        formData.append("ddlDept", $("#ddlDept").val() || '');
        formData.append("ddlformatnumberSelectedText", $("#ddlformatnumber option:selected").text() || '');
        formData.append("txtDocNo9", $("#txtDocNo9").val() || '');
        formData.append("txtDocNo", $("#txtDocNo").val() || '');
        formData.append("txtDocNo1", $("#txtDocNo1").val() || '');
        formData.append("txtDocNo2", $("#txtDocNo2").val() || '');
        formData.append("txtDocTitle", $("#txtDocTitle").val() || '');
        formData.append("hdnDocTitle", $("#hdnDocTitle").val() || '');

        formData.append("ddlDayA", $("#ddlDayA").val() || '');
        formData.append("ddlMonthASelectedText", $("#ddlMonthA option:selected").text() || '');
        formData.append("ddlYearA", $("#ddlYearA").val() || '');

        formData.append("txtCurrRevNoR", $("#txtCurrRevNoR").val() || '');
        formData.append("hdnCurrRevNoR", $("#hdnCurrRevNoR").val() || '');
        formData.append("hdnNewRevNoR", $("#hdnNewRevNoR").val() || '');
        formData.append("ddlRevDayR", $("#ddlRevDayR").val() || '');
        formData.append("ddlRevMonthRSelectedText", $("#ddlRevMonthR option:selected").text() || '');
        formData.append("ddlRevYearR", $("#ddlRevYearR").val() || '');
        formData.append("ddlImpDayR", $("#ddlImpDayR").val() || '');
        formData.append("ddlImpMonthRSelectedText", $("#ddlImpMonthR option:selected").text() || '');
        formData.append("ddlImpYearR", $("#ddlImpYearR").val() || '');

        formData.append("txtCurrRevD", $("#txtCurrRevD").val() || '');
        formData.append("ddlDayD", $("#ddlDayD").val() || '');
        formData.append("ddlMonthDSelectedText", $("#ddlMonthD option:selected").text() || '');
        formData.append("ddlYearD", $("#ddlYearD").val() || '');

        // Misc
        formData.append("txtReason", $("#txtReason").val() || '');
        formData.append("lblAppAuth", $("#lblAppAuth").text() || '');

        // Radio buttons (booleans)
        formData.append("IsChecked_rbtnAdd", $("#rbtnAdd").is(":checked"));
        formData.append("IsChecked_rbtnRev", $("#rbtnRev").is(":checked"));

        // ViewState-like hidden fields
        formData.append("hdAPPEMAILID", $("#hdAPPEMAILID").val() || '');
        formData.append("hdIsoDocId", $("#IsoDocId").val() || '');
        formData.append("OldIsoDocId", $("#OldIsoDocId").val() || '');
        formData.append("hdstrAppAuth", $("#hdAPPECODE").val() || '');
        formData.append("hdFilenameExist", $("#hdnFile").val() || '');
      

        // Add files
        formData.append("txtFileUpload", $("#txtFileUpload")[0].files[0]);

        $.ajax({
            url: '/QMS/ISODocEdit',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                var result = response.result;
                if (result) {
                    if (result.Status === true) {
                        const redirectionTo = '/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx#QMS'; //update url

                        if (result.obj && result.obj.hasOwnProperty("IsWindowAlert") && result.obj.IsWindowAlert === true) {
                            alert(result.Message);
                        }
                        window.location.href = redirectionTo;
                       
                    }
                    else
                    {
                        if (result.obj && result.obj.hasOwnProperty("IsWindowAlert") && result.obj.IsWindowAlert === true) {
                            alert(result.Message);
                        }
                        else if (result.obj && result.obj.hasOwnProperty("errorpanel_Visible") && result.obj.errorpanel_Visible === true) {
                            document.getElementById('errorpanel').style.display = '';
                            $("#status").text(result.Message);
                            $("#txtup").focus();
                        }
                        else {
                            alert(result.Message);
                        }
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


});

