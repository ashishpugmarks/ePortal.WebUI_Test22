
$(document).ready(function () {

    //$('[data-toggle="tooltip"]').tooltip();
            
    //        $.ajax({
    //            type: "GET",
    //            url: "/Home/GetAssociateDetails",
    //            dataType: "json",
    //            success: function (msg) {

    //                var peopleSearch = [];
    //                for (var i = 0; i < msg.length; i++) {
    //                    queryStr = { "peopleSearch": msg[i]._EName + "-" + msg[i]._ECode };
    //                    peopleSearch.push(queryStr);
    //                }

    //                $("#peopleSearch").fuzzyComplete(peopleSearch);
    //                $('input').on('keyup blur', function () {
    //                    $(this).parent().find(".output").html($(this).parent().find("select").val());
    //                });
    //                $("#peoplesearchmob").fuzzyComplete(peopleSearch);
    //                $('input').on('keyup blur', function () {
    //                    $(this).parent().find(".output").html($(this).parent().find("select").val());
    //                });
    //            },
    //            error: function (xhr, status, error) {
    //                debugger;
    //                window.location.href = "Login";
    //            }
    //        });
    //        //Adding approval count of Dynamic Form Module in Total Approval Count
    //        $.ajax({
    //            type: "GET",
    //            url: "/DynamicTMPL/GetDynamicPlateformApprovalCount",
    //            //data: JSON.stringify(data),
    //            contentType: "application/json; charset=utf-8",
    //            datatype: "json",
    //            success: function (data) {

    //                if (data != "") {
    //                    var appcnt = parseInt($("#hrefappcnt")[0].innerHTML) + parseInt(data);
    //                    $("#hrefappcnt")[0].innerHTML = appcnt;
    //                }
    //                else { }
    //            },
    //            complete: function () {

    //            },
    //            error: function () {

    //            }
    //        });
    //        //Adding approval count of Dynamic Form Module in Total Approval Count
    //        $.ajax({
    //            type: "POST",
    //            url: "/DynamicTMPL/GetDynamicPlateformRequestCount",
    //            //data: JSON.stringify(data),
    //            contentType: "application/json; charset=utf-8",
    //            datatype: "json",
    //            success: function (data) {

    //                if (data != "") {
    //                    var reqcnt = parseInt($("#hrefreqcnt")[0].innerHTML) + parseInt(data);
    //                    $("#hrefreqcnt")[0].innerHTML = reqcnt;
    //                }
    //                else { }
    //            },
    //            complete: function () {

    //            },
    //            error: function () {

    //            }
    //        });

    //$.ajax({
    //    type: "GET",
    //    url: "/Home/GetEmergencyDept",
    //    dataType: "json",
    //    success: function (data) {
    //        var markup = "";
    //        var markupsite = "";
    //        var res = JSON.stringify(data);
    //        var res1 = JSON.parse(res)
    //        var list = data;
    //        var result = [];
    //        var resultsite = [];
    //        $.each(res1, function (index, item) {
    //            if ($.inArray(item["OPERATION"], result) == -1) {
    //                result.push(item["OPERATION"]);
    //            }
    //            if ($.inArray(item["SiteID"], resultsite) == -1) {
    //                resultsite.push(item["SiteID"]);
    //            }
    //        });

    //        for (var x = 0; x < result.length; x++) {
    //            markup += "<option value=" + result[x] + ">" + result[x] + "</option>";
    //        }
    //        for (var x = 0; x < resultsite.length; x++) {
    //            var filtdata = res1.filter(function (entry) { return entry.SiteID === resultsite[x] });
    //            var sitename = filtdata[0].Site;
    //            markupsite += "<option value=" + resultsite[x] + ">" + sitename + "</option>";
    //        }
    //        $('#ddl_contact_desk').html(markup).show();
    //        $('#ddl_contact_mobile').html(markup).show();
    //        $('#ddl_site_desk').html(markupsite).show();
    //        $('#ddl_site_mobile').html(markupsite).show();
    //        $('select.type').trigger('change');
    //    },
    //    error: function (xhr, status, error) {
    //        //window.location.href = "Login";
    //    }
    //})
});


//function WindowSettings_WithoutMenu(str1, str2, width, height) {
//    var winTop = (screen.height / 2) - 350;
//    var winLeft = (screen.width / 2) - 450;
//    var windowFeatures = "location=no,status=no,width= " + width + ",height= " + height;
//    windowFeatures = windowFeatures + ",left = " + winLeft + ",";
//    windowFeatures = windowFeatures + "top=" + winTop + ",resizable=no" + ",scrollbars";
//    window.open(str1, str2, windowFeatures);
//}
//function OpenDoc(index, process) {
//    if (process == "1") {
//        window.location.href = '/Home/GetPresAttach/?id=' + index;
//    }
//    else if (process == "2") {
//        window.location.href = '/Login/GetSearchResults/?id=' + index;
//    }
//    else if (process == "3") {
//        window.location.href = '/Login/GetAttachement2/?id=' + index;
//    }
//}
//function OpenVideo(index, process) {
//    var player = videojs(document.querySelector('.video-js'));
//    var videoPath = $('#owlLeftTop .owl-item.active').find('.overlay_image');
//    var videoSrc = videoPath.data('src');
//    var sources = [{ "type": "video/mp4", "src": "@ViewBag.URL" + "/api/PresidentMedia/play?f=" + index }];
//    player.src(sources);
//    player.controls = "controls";
//    player.load();
//    player.play();
//    $('#videoModal').modal({ backdrop: "static" });

//    $("#videoModal").on('mouseover', function () {
//        var owl = $('#owlLeftTop');
//        owl.trigger('stop.owl.autoplay');
//        var owlRight = $('#owlRight');
//        owlRight.trigger('stop.owl.autoplay');
//    });

//    function GetPDF(current) {
//        var _pdfName = $(current).attr('data-id');
//        window.open(_pdfName, "_blank");
//    }

//    function GetDetails(current) {
//        $("#myAnnouncementsModal").modal('hide');
//        var $buttonClicked = $(this);
//        $.ajax({
//            type: "GET",
//            url: "/Announcement/ArchiveAnnouncementNoticeDetails",
//            data: { id: current.id },
//            contentType: "application/json; charset=utf-8",
//            datatype: "json",
//            success: function (data) {
//                if (data == "error") {
//                    sweetAlert("", "An error occurred while processing your request.", "error");
//                }
//                else if (data != "") {
//                    $('#myCommunicationModal').html(data);
//                    $('#myCommunicationModal').modal({ backdrop: "static" });
//                }
//            },
//            error: function () {
//                sweetAlert("Oops...", "Something went wrong!", "error");
//            }
//        });
//    }
//}
