$('.homeContent').removeClass('homeContent');
$(document).ready(function () {


    const grdqmsdetail = $('#grdqmsdetail').DataTable({
        searching: true,
        lengthChange: false,
        paging: true,
        pageLength: 10,
        ordering: false,
        autoWidth: false,
        info: false,
        stateSave: false,
        displayStart: 0

    });

    grdqmsdetail.draw(true);



    $('#grdqmsdetail').on('click', 'tbody .filename', function (e) {
        e.preventDefault();
        const filename = $(this).attr('data-navigateurl');
        const url = "/QMS/Opendocument?docid=" + filename;
        window.open(url, '_blank', 'noopener');

    });

   
});

