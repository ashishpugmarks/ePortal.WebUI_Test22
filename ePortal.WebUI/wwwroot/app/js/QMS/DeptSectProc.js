$('.homeContent').removeClass('homeContent');
$(document).ready(function () {


    const grdDeptSectProc = $('#grdDeptSectProc').DataTable({
        searching: false,
        lengthChange: false,
        paging: true,
        pageLength: 15,
        ordering: false,
        autoWidth: false,
        info: false,
        stateSave: false,
        displayStart: 0

    });

    grdDeptSectProc.draw(true);


    $('#grdDeptSectProc').on('click', 'tbody .filename', function (e) {
        e.preventDefault();
        const filename = $(this).attr('data-filename');

       const url = "../../Uploads/ISODoc/" + filename;
       window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });


   
});

