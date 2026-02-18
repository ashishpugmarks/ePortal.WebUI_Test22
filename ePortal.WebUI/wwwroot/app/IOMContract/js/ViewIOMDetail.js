$('.homeContent').removeClass('homeContent');
$(document).ready(function () {

   
    $("#prevappnotelink").on("click", function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200','resizable=yes,scrollbars=yes');
    });
    $("#prevfinaldoclink").on("click", function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });

    $("#agreementlink").on("click", function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200','resizable=yes,scrollbars=yes');
    });

    $("#antibriberylink").on("click", function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });

    $(".linkagreement").on("click", function (e) {
        e.preventDefault();
        debugger;
        var attachmentpath = $(this).data('attachmentpath');
        var url = '../../Uploads/IOM/'+attachmentpath;
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });


});

