$('.homeContent').removeClass('homeContent');
$(document).ready(function () {

    $('.lnkShow').on('click', function (e) {
        e.preventDefault();

        const filename = $(this).data('filename');
        const url = "/QMS/opendocument?FILENAME="+filename;
        window.open(url, '_blank', 'noopener');

    });

});

