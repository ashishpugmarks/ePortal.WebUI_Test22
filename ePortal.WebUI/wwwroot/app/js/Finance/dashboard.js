  function openPopupwithScrol(strOpenUrl, width, height) {
            var winTop = (screen.height - height) / 2;
            var winLeft = (screen.width - width) / 2;
            var windowFeatures = "location=no,status=no,width=" + width + "px,height=" + height;
            windowFeatures = windowFeatures + "px,left = " + winLeft + "px,";
            windowFeatures = windowFeatures + "top=" + winTop + "px,resizable=yes" + ",scrollbars";
            //window.open (strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,SCROLLBARS=yes,RESIZABLE=no,LOCATION=no,DIRECTORIES=no,STATUS=yes,width="+width+",height="+height);
            window.open(strOpenUrl, "mywindow1", windowFeatures);
        }


    $(document).ready(function () {
        $("#tblTaxList").dataTable({

            ordering: false,       // keep original order
            searching: false,      // disable search
            lengthChange: false,   // hide "Show X entries" dropdown
            info: false,           // hide info text
            pageLength: 5

        });
     
$("[data-ctrl-id='btnprintinv']").on('click', function (e) {
    var id = $(this)[0];
    openPopupwithScrol($(id).attr('data-src'), '700', '500');
});
        $("[data-ctrl-id='btnOpen']").on('click', function (e) {
            var id = $(this)[0];
            window.open($(id).attr('data-src'), "_self");
            //openPopupwithScrol($(id).attr('data-src'), '700', '500');
        });
        $("[data-ctrl-id='btnOpenTax']").on('click', function (e) {
            var id = $(this)[0];
            location.href = $(id).attr('data-src');
            //openPopupwithScrol($(id).attr('data-src'), '700', '500');
        });
    });
