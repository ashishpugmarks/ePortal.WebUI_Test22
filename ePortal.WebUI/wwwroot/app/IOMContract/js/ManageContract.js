$('.homeContent').removeClass('homeContent');
$(document).ready(function () {
   

    $("#history").on("click", function (e) {
        window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequestHistory.aspx?id=20"; //Fixed URL
    });

    $('#grdiomrequest').on('click', '.linkedit', function (e) {
        var iomId = $(this).closest('tr').data('iomid');
        linkedit_click(iomId);
    });
    $('#grdiomrequest').on('click', '.LINKUSERACK', function (e) {
        var iomId = $(this).closest('tr').data('iomid');
        LINKUSERACK_CLICK(iomId);
    });
    $('#grdiomrequest').on('click', '.LINKFINALDOC', function (e) {
        var iomId = $(this).closest('tr').data('iomid');
        LINKFINALDOC_CLICK(iomId);
    });

    $('#grdiomrequest').on('click', '.linkCommEdit', function (e) {
        var iomId = $(this).closest('tr').data('iomid');
        linkCommEdit_Click(iomId);
    });

    $('#grdiomrequest').on('click', '.LinkCommView', function (e) {
        var iomId = $(this).closest('tr').data('iomid');
        LinkCommView_Click(iomId);
    });
    try {
        $.fn.dataTable.ext.errMode = 'none';
        $('#grdiomrequest').DataTable({
            //dom: 't',
            searching: false,
            lengthChange: false,
            paging: true,
            pageLength: 10,
            ordering: false,
            autoWidth: false,
            info: false,
            stripeClasses: [],
            columnDefs: [
                { targets: '_all', defaultContent: '' }
            ]
        });

        $('#grdiomrequest').on('click', '.view_iom_details', function (e) {
            e.preventDefault();
            var iomId = $(this).data('iomid');
            var url = '/iomcontract/ViewIOMDetail?IOMID=' + iomId;
            openPopup(url, 820, 590);
        });
    } catch { }
   
});

function linkedit_click(iomId) {
    window.location.href = "IOMEditRequestForm?IOMID=" + iomId;
}
function LINKUSERACK_CLICK(iomId) {
    window.location.href = "UserAcknowledgement?IOMID=" + iomId;
}
function LINKFINALDOC_CLICK(iomId) {
    window.location.href = "SubmitFinalDocument?IOMID=" + iomId;

}

function linkCommEdit_Click(iomId) {
    window.location.href = "UserCommunication?IOMID=" + iomId;
}

function LinkCommView_Click(iomId) {
    window.location.href = "UserCommunication?IOMID=" + iomId;
}
function openPopup(url, width, height) {
    var left = (screen.width / 2) - (width / 2);
    var top = (screen.height / 2) - (height / 2);
    window.open(url, '_blank', 'width=' + width + ',height=' + height + ',top=' + top + ',left=' + left + ',resizable=yes,scrollbars=yes');
}
