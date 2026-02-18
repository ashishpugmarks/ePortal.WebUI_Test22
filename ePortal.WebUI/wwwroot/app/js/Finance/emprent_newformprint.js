 $(document).ready(function () {

        $("[data-ctrl-id='btnprint']").on('click', function () {

            window.print();
        })
             $("[data-ctrl-id='btncancel']").on('click', function () {

        window.close();
             })
          }
    )