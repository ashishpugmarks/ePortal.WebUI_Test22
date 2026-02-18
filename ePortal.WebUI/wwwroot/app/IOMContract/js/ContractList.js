$('.homeContent').removeClass('homeContent');
$(document).ready(function () {

    //$('#grdcontractlist').DataTable({
    //    searching: false,
    //    lengthChange: false,
    //    paging: true,
    //    pageLength: 10,
    //    ordering: false,
    //    autoWidth: false,
    //    info: true,
    //});


    $("#txtStarttDate").datepicker({
        showOn: "both",
        buttonImage: "../../images/cal.gif",
        buttonImageOnly: true,
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        onSelect: function (selected) {
            $("#txtEndtDate").datepicker("option", "minDate", selected);
        }
    });

    $("#txtEndtDate").datepicker({
        showOn: "both",
        buttonImage: "../../images/cal.gif",

        buttonImageOnly: true,
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        onSelect: function (selected) {
            $("#txtStarttDate").datepicker("option", "maxDate", selected);
        }
    });

    $("#cmdReset").on("click", function (e) {
        e.preventDefault();

        $("#ddlcontracttype").val('');
        $("#txtvendorname").val('');
        $("#txtStarttDate").val('');
        $("#txtEndtDate").val('');

    })

    $('#grdcontractlist').on('click', '.imgplus', function (e) {
       e.preventDefault();
       const $currentTr = $(this).closest('tr');
       const $imgminus = $currentTr.find('.imgminus');

       $(this).hide();
       $imgminus.show();

       const hdagreementid = $currentTr.data('hdagreementid');
       const $targetDiv = $currentTr.next('tr').find('.trdynsubcontractList').first();  
       loadContractListPartialView(hdagreementid, $targetDiv);
    });

    $('#grdcontractlist').on('click', '.imgminus', function (e) {
        e.preventDefault();

        const $currentTr = $(this).closest('tr');
        const $imgplus = $currentTr.find('.imgplus');

        $(this).hide();
        $($imgplus).show();

        const $targetDiv = $currentTr.next('tr').find('.trdynsubcontractList').first();  
        if ($targetDiv.length) {
            $targetDiv[0].innerHTML = "";
            $targetDiv.hide();
        } 
    });

    $(document).on('click', '.view_iom_details', function (e) {
        e.preventDefault();               
        var iomId = $(this).data('iomid');
        var url = '/iomcontract/ViewIOMDetail?IOMID=' + iomId;
        openPopup(url, 820, 590);
    });

    $(document).on('click', '.subcontracter_finaldoclink', function (e) {
        e.preventDefault();
        var finaldoc = $(this).data('finaldoc');
        var url = '../../Uploads/IOM/' + finaldoc;
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');

    });

    $('#grdcontractlist').on('click', '.finaldoclink', function (e) {
    //$(document).on('click', '.finaldoclink', function (e) {

        e.preventDefault();
        var finaldoc = $(this).data('finaldoc');
        var url = '../../Uploads/IOM/' + finaldoc;
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');

    });

    $('#grdcontractlist').on('click', '.imgbtnclose', function (e) {
    //$(".imgbtnclose").on("click", function (e) {
        e.preventDefault();
        const $currentTr = $(this).closest('tr');

        const hdagreementid = $currentTr.data('enchdagreementid');
        window.location.href = "/IOMContract/CloseContract?AGREEMENTID=" + hdagreementid;
    });

    $('#grdcontractlist').on('click', '.imgbtnrenewal', function (e) {
    //$(".imgbtnrenewal").on("click", function (e) {
        e.preventDefault();
        const $currentTr = $(this).closest('tr');

        const hdagreementid = $currentTr.data('enchdagreementid');
        window.location.href = "/IOMContract/ContractRenewal?AGREEMENTID=" + hdagreementid;
    });

    $('#grdcontractlist').on('click', '.imgbtnamendment', function (e) {
    //$(".imgbtnamendment").on("click", function (e) {
        e.preventDefault();
        const $currentTr = $(this).closest('tr');

        const hdagreementid = $currentTr.data('enchdagreementid');
        window.location.href = "/IOMContract/ContractAmendment?AGREEMENTID=" + hdagreementid;
    });

    $('#grdcontractlist').on('click', '.lbl_contracttype', function (e) {
//    $(".lbl_contracttype").on("click", function (e) {
        e.preventDefault();
        const $currentTr = $(this).closest('tr');

        const hdagreementid = $currentTr.data('hdagreementid');
        const url = "/IOMContract/ViewAgreement?AGREEMENTID=" + hdagreementid;

        window.open(url, "_blank", 'width=520,height=295,top=90,left=200', 'resizable=yes,scrollbars=yes');

    });


    $(document).on('click', '[data-ctrl="paging-btn"]', function (e) {
        e.preventDefault();
        $('#hfPage').val($(this).data('page'));
        $('#frmContractList').trigger('submit');
    });



});


function loadContractListPartialView(hdagreementid, $targetDiv)
{
    const url = "/IOMContract/loadContractListPartialView?HDAGREEMENTID=" + hdagreementid;
    fetch(url)
        .then(response => response.text())
        .then(html => {
            if ($targetDiv.length) {
                $targetDiv.show();
                $targetDiv[0].innerHTML = html;
            } 
        })
        .catch(error => console.error("Error loading partial view:", error))
        .finally(() => {
        });
}


function openPopup(url, width, height) {
    var left = (screen.width / 2) - (width / 2);
    var top = (screen.height / 2) - (height / 2);
    window.open(url, '_blank', 'width=' + width + ',height=' + height + ',top=' + top + ',left=' + left + ',resizable=yes,scrollbars=yes');
}




