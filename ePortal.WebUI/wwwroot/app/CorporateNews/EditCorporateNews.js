 $(document).ready(function () {
            // $('#START_DATE').datepicker({
            //     defaultDate: '@Model.START_DATE',
            //     format: 'dd-M-yyyy',
            //     showClose: true,
            //     showClear: true,
            //     toolbarPlacement: 'top'
            // });
            // $('#END_DATE').datepicker({
            //     defaultDate: '@Model.END_DATE',
            //     format: 'dd-M-yyyy',
            //     showClose: true,
            //     showClear: true,
            //     toolbarPlacement: 'top',
            //     stepping: 15
            // });

             $('.datepic').datepicker({
                format: 'dd-M-yyyy',
                 autoclose: true,
                toolbarplacement: 'top',
            });

            $("#Upload_Banner").change(function () {
                var _URL = window.URL || window.webkitURL;
                var file = $(this)[0].files[0];
                var image = new Image();
                image.src = _URL.createObjectURL(file);
                var extension = $(this).val().split('.').pop().toLowerCase();
                var validFileExtensions = ['gif', 'png', 'jpg', 'jpeg'];
                if ($.inArray(extension, validFileExtensions) == -1) {
                    alert("Sorry!! Upload only 'gif', 'png', 'jpg', 'jpeg' file")
                    $(this).replaceWith($(this).val('').clone(true));
                    $('#Apply').prop('disabled', true);
                } else {
                    if ($(this).get(0).files[0].size > (3000000)) {
                        alert("Sorry!! Max allowed file size is 3 MB");
                        $(this).replaceWith($(this).val('').clone(true));
                        $('#Apply').prop('disabled', true);
                    }
                    image.onload = function () {
                        var height = this.height;
                        var width = this.width;
                        if (height < 150 && width < 200) {
                            alert("Sorry!! Width and Height must be between 200px - 250px & 150px - 200px.");
                            $("#Upload_Banner").replaceWith($("#Upload_Banner").val('').clone(true));
                        }
                        else if (height > 200 && width > 250) {
                            alert("Sorry!! Width and Height must be between 200px - 250px & 150px - 200px.");
                            $("#Upload_Banner").replaceWith($("#Upload_Banner").val('').clone(true));
                        }
                        else {
                            $('#Apply').prop('disabled', false);
                        }
                    };
                }
            });

            $("#Upload_Attachment1").change(function () {
                var extension = $(this).val().split('.').pop().toLowerCase();
                var validFileExtensions = ['pdf', 'doc', 'docx', 'ppt', 'pptx', 'gif', 'png', 'jpg', 'jpeg'];
                if ($.inArray(extension, validFileExtensions) == -1) {
                    alert("Sorry!! Upload only 'pdf', 'doc', 'docx', 'ppt', 'pptx', 'gif', 'png', 'jpg', 'jpeg' file")
                    $(this).replaceWith($(this).val('').clone(true));
                    $('#Apply').prop('disabled', true);
                } else {
                    if ($(this).get(0).files[0].size > (5000000)) {
                        alert("Sorry!! Max allowed file size is 5 mb");
                        $(this).replaceWith($(this).val('').clone(true));
                        $('#Apply').prop('disabled', true);
                    } else {
                        $('#Apply').prop('disabled', false);
                    }
                }
            });

            $("#Upload_Attachment2").change(function () {
                var extension = $(this).val().split('.').pop().toLowerCase();
                var validFileExtensions = ['pdf', 'doc', 'docx', 'ppt', 'pptx', 'gif', 'png', 'jpg', 'jpeg'];
                if ($.inArray(extension, validFileExtensions) == -1) {
                    alert("Sorry!! Upload only 'pdf', 'doc', 'docx', 'ppt', 'pptx', 'gif', 'png', 'jpg', 'jpeg' file")
                    $(this).replaceWith($(this).val('').clone(true));
                    $('#Apply').prop('disabled', true);
                } else {
                    if ($(this).get(0).files[0].size > (5000000)) {
                        alert("Sorry!! Max allowed file size is 5 mb");
                        $(this).replaceWith($(this).val('').clone(true));
                        $('#Apply').prop('disabled', true);
                    } else {
                        $('#Apply').prop('disabled', false);
                    }
                }
            });
        //@*$('#@Model.ATTACHMENTID').click(function () {
        //        debugger;
        //        if(@Model.STATUS==2)
        //        {
        //            var cnfrm = confirm('Are you sure want to delete?');
        //            if(cnfrm != true)
        //            {
        //                return false;
        //            }
        //        }

        //    });*@
            //$('#form').submit(function (evt) {
            //    debugger;
            //    evt.preventDefault();
            //    window.history.back();
     //});

            $('#btnEdit').click(function (e) {
                debugger;
                e.preventDefault();
                var data = new FormData(this.form);
                $.ajax({
                    type: "POST",
                    url: "/CorporateNews/EditCorporateNews",
                    data: data,
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        debugger;
                        $('#myModal').modal('hide');
                        var msg = "";
                        var actionType = "";
                        if (response == "success") {
                            msg = "Your Data has been Updated Successfully.";
                            actionType = "success";
                        }
                        else if (response == "failed") {
                            msg = "Your Data not Updated";
                            actionType = "warning";
                        }
                        else {
                            msg = response;
                            actionType = "error";
                        }
                        swal({
                            title: "",
                            text: msg,
                            type: actionType,
                        }, function (isConfirm) {
                            if (isConfirm) {
                                window.location.href = "/CorporateNews/CorporateNewsMaster";
                            }
                        });
                    },
                    error: function (response) {
                        sweetAlert("Oops...", "Something went wrong!", "error");
                    }
                });
            });

        });

        function DeleteCorporateNewsDocument(current) {
            //$('#confirmModal').modal('hide');
            var attachmentId = current.id;
            var attachmentName = current.name;
            var data = {
                ATTACHMENTID: attachmentId,
                type: attachmentName
            }
            $.ajax({
                type: "PUT",
                url: "/CorporateNews/DeleteDocument",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                success: function (data) {
                    var a = JSON.parse(data);
                    if (a.BANNER_NAME == null) {
                        $(".BANNER").css("display", "none");
                    }
                    if (a.ATTACHMENT1_NAME == null) {
                        $(".Attachment1").css("display", "none");
                    }
                    if (a.ATTACHMENT2_NAME == null) {
                        $(".Attachment2").css("display", "none");
                    }
                    //var id = "#" + data.ATTACHMENTID;
                    //window.location.href = "/CorporateNews/CorporateNewsMaster";
                    sweetAlert("", "Document Deleted Successfully", "success");
                    //alert("Document Deleted Successfully");
                },
                error: function () {
                    sweetAlert("Oops...", "Something went wrong!", "error");
                }
            });
        }