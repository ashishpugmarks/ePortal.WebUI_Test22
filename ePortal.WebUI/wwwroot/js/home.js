$(document).ready(function () {
    /* mega menu Script start */
    // $('.main_nav_list li').on('click', function(e){
    $(document).find('#menu_div').on('click', '.main_nav_list li', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var linkurl = $(this).find('a').attr('href')
        if (linkurl != "#") {
            if ($(this).find('a').attr('target') == '_blank') {
                window.open(linkurl, '_blank');
            }
            else {
                window.location = linkurl;
            }
        }
        if (!$(this).parent().hasClass('fatnav_list') && !$(this).hasClass('search_menu')) {
            var firstElem_id, elem_id = $(this).find('a').attr('href');
            //firstElem_id = $(elem_id).find('li a').first().attr('href');
          
            if ($(window).width() <= 767) {
                if ($(this).hasClass('active')) {
                    $(this).removeClass('active').find('li').removeClass('active');
                    $(this).find('ul').not('.fatnav_sublist').slideUp();
                }
                else {
                    $(this).addClass('active').siblings().removeClass('active');
                    $(this).children('ul').slideDown();
                    $(this).siblings().find('ul').not('.fatnav_sublist').slideUp();
                }
            }

            else {
                $(this).find('ul').eq('0').css('display', 'block');
                //$(this).find('ul').css('display', 'block').parent().siblings().find('ul').css('display', 'none');
                $(this).siblings().find('ul').css('display', 'none');
                $(this).addClass('active').siblings().removeClass('active').find('li').removeClass('active');

                //$('.secondary_list').find('ul').css('display', 'block');
                if ($(this).find('.fatnav_sublist').length > 0) {
                    $('.fatnav_sublist').css('display', 'block');
                }
                //else {
                //    $('.fatnav_sublist').css('display', 'none');
                //}
				if ($(this).parent('.secondary_list')[0] != undefined) {
                    if ($(this).parent('.secondary_list')[0].clientHeight < 44) {
                        $('.fatnav_list').css('padding-top', '1.14em')
                    }
                    else {
                        $('.fatnav_list').css('padding-top', '3.14em')
                    }
                }
            }

            ($('.fatnav_list', this).css('display') == 'block') ? $('.overlay').css('display', 'block') : $('.overlay').css('display', 'none');

            findLastLevel($(this));
        }
    });

    function findLastLevel(curElem) {
        $(curElem).children('ul').children('li').each(function () {
            if ($(this).children('ul').length == 0) {
                $(this).addClass('lastLevel');
            }
        });
    }

    $('.hamburger').on('click', function () {
        $(this).parents('.header').addClass('mob-menu');
    });

     $('.close_btn').on('click', function (e) {
        $(this).parents('.header').removeClass('mob-menu');
        e.preventDefault();
    });

    $('body').on('click', function () {
        $('.main_nav_list').find('li').removeClass('active').find('ul').removeAttr('style').find('li').removeClass('active');
        $('.overlay').css('display', 'none');
    });

    $('.search_menu span').on('click', function () {
        if ($(window).width() < 768) {
            if ($(this).hasClass('search_open')) {
                $(this).removeClass('search_open').parents('.search_menu').removeClass('col-xs-12');
                $(this).siblings('.search_input').animate({ 'width': '0' });
                $(this).parent().siblings('.close_btn').css('display', 'block');
            }
            else {
                $(this).addClass('search_open').parents('.search_menu').addClass('col-xs-12');
                $(this).siblings('.search_input').animate({ 'width': '100%' });
                $(this).parent().siblings('.close_btn').css('display', 'none');
            }
        }
    });


    /* mega menu Script end */

    $("#vision,#visionHMSI,#visionMobile,#visionHMSIMobile").owlCarousel({
        loop: false,
        margin: 10,
        dots: true,
        items: 1,
        autoplay: false,
        nav: false,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 1
            },
            1000: {
                items: 1
            }
        }
    });
    var owl = $("#announce");
    owl.owlCarousel({
        loop: true,
        margin: 10,
        dots: true,
        items: 1,
        autoplay: true,
        nav: true,
        navText: ["<img src='../../images/announ_arrow_left.png'>", "<img src='../../images/announ_arrow_right.png'>"],
        autoplayTimeout: 4000,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1,
                autoplay: true,
            },
            768: {
                items: 1,
            },
            1000: {
                items: 1
            }
        }
    });

    var interval = setInterval(function () {
        var india = moment();
        var japan = moment().add(3, 'hour').add(30, 'minutes');
        var thailand = moment().add(1, 'hour').add(30, 'minutes');
        $('.flag1 span:nth-child(2),.flagMobile .flag1 span,.defaultTime span:first').html(india.format('hh:mm:ss A') + " (IST)");
        $('.flag2 span:nth-child(2),.flagMobile .flag2 span').html(japan.format('hh:mm:ss A ') + " (JST)");
        $('.flag3 span:nth-child(2),.flagMobile .flag3 span').html(thailand.format('hh:mm:ss A ') + " (TST)");
    }, 100);

    // announcements slider play pause functionality	
    $(document).on('click', '.announcePause', function () {
        var attr_value = $(this).attr('data-attr');
        if (attr_value == 'pause') {
            owl.trigger('stop.owl.autoplay');
            $(this).attr({
                src: '/images/icon_play_anncnt_carousel.png',
                'data-attr': 'play'
            });
        }
        else {
            owl.trigger('play.owl.autoplay', [1000]);
            $(this).attr({
                src: '/images/announ_pause.png',
                'data-attr': 'pause'
            });
        }
    });

    // birthday card modal functionality
    //$(window).load(function () {
    //    $('#myBirthdayModal').modal();
    //});

    $(".closeDoodle").click(function () {
        $("#myBirthdayModal").modal('hide');
    });

    // Read more modal functionality
    $('.announceContent .readAnnounce').on('click', function (event) {
        event.preventDefault();
        $("#myBirthdayModal").modal('hide');
        id = event.target.id;
        $.ajax({
            type: "POST",
            url: "/Login/GetPopDetail",
            data: { "id": id },
            //contentType: "application/json;",
            //dataType: "json",
            success: function (msg) {
                document.getElementById("popsubject").innerHTML = msg.SUBJECT;
                document.getElementById("popupsubject1").innerHTML = msg.SUBJECT;
                document.getElementById("popdescription").innerHTML = msg.DESCRIPTION;
                document.getElementById("anudate").innerHTML = msg.START_DATE_STR;
                //document.getElementById("anudate1").innerHTML = msg.START_DATE_STR;
                document.getElementById("popwhendtl").innerHTML = msg.START_TIME_STR + " onwards on " + msg.START_DATE_STR;

                var binary = '';
                var description = msg.DESCRIPTION;


                if (description == null)
                    document.getElementById("popdescription").innerHTML = msg.BRIEF;

                var linkcontent = "<h6>Download :</h6>";
                var attachment1 = msg.ATTACHMENT1_NAME;
                var attachment2 = msg.ATTACHMENT2_NAME;
                var processname;
                if (attachment1 != null) {
                    processname = '2';
                    linkcontent = linkcontent + " <a data-ctrl='announcement-popup-att-1' data-param-1='" + id + "' data-param-2='" + processname + "'>" + msg.ATTACHMENT1_NAME + "</a>";
                }
                if (attachment2 != null) {
                    processname = '3';
                    linkcontent = linkcontent + " <a data-ctrl='announcement-popup-att-2' data-param-1='" + id + "' data-param-2='" + processname + "'>" + msg.ATTACHMENT2_NAME + "</a>";
                }

                document.getElementById("dwnlink").innerHTML = linkcontent;

                if (attachment1 == null && attachment2 == null) {
                    document.getElementById("dwnlink").innerHTML = "";
                }
                else {
                    //CSP 3.0 Compliant changes
                    $("[data-ctrl='announcement-popup-att-1']").on('click', function (e) {
                        e.preventDefault();
                        var param1 = $(this).data('param-1');
                        var param2 = $(this).data('param-2');
                        OpenDoc(param1, param2);
                    });
                    $("[data-ctrl='announcement-popup-att-2']").on('click', function (e) {
                        e.preventDefault();
                        var param1 = $(this).data('param-1');
                        var param2 = $(this).data('param-2');
                        OpenDoc(param1, param2);
                    });
                }

                if (msg.BANNER != null) {
                    var binary = '';
                    var bytes = new Uint8Array(msg.BANNER);
                    var len = bytes.byteLength;
                    for (var i = 0; i < len; i++) {
                        binary += String.fromCharCode(bytes[i]);
                    }
                    var dataimg = window.btoa(binary);;

                    document.getElementById("popupimage").src = "data:" + msg.BANNER_CONTENTTYPE + ";base64," + dataimg;
                    document.getElementById("popupimage").display = "none";
                    document.getElementById("popupimage").style.display = "block";
                }
                else {

                    document.getElementById("popupimage").src = "";
                    document.getElementById("popupimage").display = "inline";
                    document.getElementById("popupimage").style.display = "none";
                }
            }
        });
        $('#myAnnouncementsModal').modal();
    });

    $(".close-announcements-modal").click(function () {
        $("#myAnnouncementsModal").modal('hide');
    });
    $(".close-readmore-modal").click(function () {
        $("#myPresidentModal").modal('hide');
    });
    // Read more modal functionality

    $(".defaultTime").click(function () {
        $(".flagMobile").toggleClass("selectToggle");
    });
    $(".flagMobile ul li").click(function () {
        var value = $(this).find("span").html();
        var valueImg = $(this).find("img").attr("src");
        $(".defaultTime span:first").hide();
        $(".defaultTime span:last").text(value);
        $(".defaultTime img").attr("src", valueImg);
        $(".flagMobile").toggleClass("selectToggle");
    });
    $(".vision a").click(function () {
        var valuetext = $(this).attr("id");
        var president_img = $(this).attr("data-photo");
        var president_name = $(this).attr("data-presidentname");
        var president_title = $(this).attr("data-presidenttitle");
        $.ajax({
            type: "GET",
            url: "/Home/GetPresidentContent",
            data: { "id": valuetext },
            //contentType: "application/json;",
            //dataType: "json",
            success: function (msg) {
                debugger;
                $('#President_Img').attr('src', president_img);
                $('#president-name').text(president_name);
                $('#president-title').text(president_title);
                //alert(msg[0].BRIEF);
                document.getElementById("popsubjectre").innerHTML = msg[0].BRIEF;
                document.getElementById("popdescriptionre").innerHTML = msg[0].MESSAGE;
                //$('#popsubjectre').html(msg[0].BRIEF);
                //$('#popdescriptionre').html(msg[0].MESSAGE);

                var linkcontent = "<h6>Download :</h6>";
                var attachment1 = msg[0].ATTACHMENT_NAME;
                if (attachment1 != null) {
                    if (msg[0].ATTACHMENT_CONTENTTYPE != "video/mp4")
                        linkcontent = linkcontent + " <a data-ctrl='pres-media-lnk' data-param-1='" + valuetext + "' data-param-2='1'>" + msg[0].ATTACHMENT_NAME + "</a>";
                    else
                        linkcontent = linkcontent + " <a data-ctrl='pres-media-lnk' data-param-1='" + valuetext + "' data-param-2='1'>" + msg[0].ATTACHMENT_NAME + "</a>";
                }

                document.getElementById("dwnlinkPres").innerHTML = linkcontent;

                if (attachment1 == null) {
                    document.getElementById("dwnlinkPres").innerHTML = "";
                }
                else {
                    //CSP 3.0 Compliant changes
                    $("[data-ctrl='pres-media-lnk']").on('click', function (e) {
                        e.preventDefault();
                        var param1 = $(this).data('param-1');
                        var param2 = $(this).data('param-2');
                        OpenDoc(param1, param2);
                    });
                }
                $('#myPresidentModal').modal();
            },
            error: function (xhr, status, error) {
                window.location.href = "Login";
            }
        });

    });
    $('.toDos').on('click', function () {
        // alert('todo');
        $(this).toggleClass('active');
    });

    $('.quickLinks').on('click', function () {
        // alert('quickLinks');
        $(this).toggleClass('active');
    });

    function toggleTelephone($thistype, $thisbranch) {

        var select_type = $thistype.find("option:selected").val();
        var select_branch = $thisbranch.find("option:selected").val();
        $.ajax({
            type: "GET",
            url: "/Home/GetEmergencyDeptbyid",
            data: { operation: select_type, siteid: select_branch },
            dataType: "json",
            success: function (data) {
                var res = JSON.stringify(data);
                var res1 = JSON.parse(res);
                if (res1.STD_CODE!=null)
                    $('#stdcode').html('+91' + res1.STD_CODE);
                else
                    $('#stdcode').html('+91');

                if (res1.TELEPHONE_NO != null)
                    $('#tel').html(res1.TELEPHONE_NO);
                else
                    $('#tel').html("");

                if (res1.MOBILE_NO!=null)
                    $('#pdesktop').html(res1.MOBILE_NO);
                else
                    $('#pdesktop').html("");

                if (res1.STD_CODE != null)
                    $('#stdcodemobile').html('+91-' + res1.STD_CODE);
                else
                    $('#stdcodemobile').html('+91-');

                if (res1.TELEPHONE_NO != null)
                    $('#telmobile').html(res1.TELEPHONE_NO);
                else
                    $('#telmobile').html("");

                if (res1.MOBILE_NO != null)
                    $('#pmobile').html(res1.MOBILE_NO);
                else
                    $('#pmobile').html("");
            },
            error: function (xhr, status, error) {
                // $('#pmobile').html('Record not available');
                $('#stdcode').html('');
                $('#tel').html('');
                $('#pdesktop').html('');
                $('#stdcodemobile').html('');
                $('#telmobile').html('');
                $('#pmobile').html('');
            }
        })
        //$thistype.parents('.dropdown').siblings('.number').find('.tollfree').addClass('hide_select');
        //$thistype.parents('.dropdown').siblings('.number').find('.' + class_name).removeClass('hide_select');

    }
    $('select.type').on('change', function (e) {
        toggleTelephone($(this), $(this).siblings('select.branch'));
    });
    $('select.branch').on('change', function (e) {
        toggleTelephone($(this).siblings('select.type'), $(this));
    });
    //$('select.type').trigger('change');

    /* mega menu Script end */

    // Resize functionality
    var oldWidth = $(window).width();

    $(window).resize(function () {
        if (!$('.search_menu span').hasClass('search_open')) {
            $('.search_menu').find('label.onlyIE').css('display', 'none');
        }
        var newWidth = $(window).width();

        if (oldWidth > 767 && newWidth < 768) {//alert('desk to mob');
            if ($('.overlay').css('display') == 'block') {
                $('.overlay').css('display', 'none');
            }
            $('.search_menu span').removeClass('search_open');
        }
        else if (oldWidth < 768 && newWidth > 767) {//alert('mob to desk');
            if ($('.header').hasClass('mob-menu')) {
                $('.header').removeClass('mob-menu');
            }
            $('.close_btn').removeAttr('style');
            $('.search_input').removeAttr('style');
            $('.search_menu span').removeClass('search_open');
        }
        oldWidth = newWidth;
    });

    // /* PLACEHOLDER	 */
    $('.search .bgColor input[placeholder]').each(function () {
        var labelText = $(this).attr('placeholder');
        $(this).parent().css({
            position: 'relative'
        });
        $('<label class="onlyIE9">' + labelText + '</label>').insertBefore(this);
        if ($(this).val() != '') {
            $(this).prev('label').addClass('hideLabel');
        } else {
            $(this).prev('label').removeClass('hideLabel');
        }

        $(this).blur(function () {
            if ($(this).val() != '') {
                $(this).prev('label').addClass('hideLabel');
            } else {
                $(this).prev('label').removeClass('hideLabel');
            }
        });
        $(this).focus(function () {
            $(this).prev('label').addClass('hideLabel');
        });
    })

    $('label.onlyIE9').click(function () {
        $(this).next().focus();
    });
    $('.search_menu input[placeholder]').each(function () {

        var labelText = $(this).attr('placeholder');
        $(this).parent().css({
            position: 'relative'
        });
        $('<label class="onlyIE">' + labelText + '</label>').insertBefore(this);
        if ($(this).val() != '') {
            $(this).prev('label').addClass('hideLabel');
        } else {
            $(this).prev('label').removeClass('hideLabel');
        }

        $(this).blur(function () {
            if ($(this).val() != '') {
                $(this).prev('label').addClass('hideLabel');
            } else {
                $(this).prev('label').removeClass('hideLabel');
            }
        });
        $(this).focus(function () {
            $(this).prev('label').addClass('hideLabel');
        });
    })

    $('label.onlyIE').click(function () {
        $(this).next().focus();

    });

    if (document.documentMode===9) {
        console.log('s');
        $('.search_menu span').on('click', function () {
            if ($(window).width() < 768) {
                if (!$(this).hasClass('search_open')) {
                    $('.onlyIE').css('display', 'none');
                }
                else {
                    $('.onlyIE').css('display', 'block');
                }
            }
        });
        $('.search_menu input[placeholder]').focus(function () {
            $('.onlyIE').css('display', 'none');
        });
        $('.search_menu input[placeholder]').blur(function () {
            $('.onlyIE').css('display', 'block');
        });
        if (!$('.search_menu span').hasClass('search_open') && $(window).width() < 768) {
            $('.onlyIE').css('display', 'none');
        }
        else {
            $('.onlyIE').css('display', 'block');
        }
    }
    $('#btnpeoplesearch').on('click', function () {
        var winTop = ((screen.height - 500) / 2);
        var winLeft = (screen.width - 490) / 2;
        var windowFeatures = "location=no,status=no,toolbar=no,resizable=no,width= " + 500 + ",height= " + 490;
        windowFeatures = windowFeatures + ",left = " + winLeft + ",";
        windowFeatures = windowFeatures + "top=" + winTop + ",resizable=no" + ",scrollbars";
        var empid = $("#peopleSearch").val().split("-")[1];
        if (empid == "" || empid == null)
        {
            swal({
                title: "",
                text: "Please select an employee from search result to view detail.",
                type: "warning",
            });
            return;
        }
        window.open("../Home/EmployeeDetails?id=" + empid, "", windowFeatures);
    });
    $('#btnpeoplesrcmob').on('click', function () {
        var winTop = ((screen.height - 500) / 2);
        var winLeft = (screen.width - 490) / 2;
        var windowFeatures = "location=no,status=no,toolbar=no,resizable=no,width= " + 500 + ",height= " + 490;
        windowFeatures = windowFeatures + ",left = " + winLeft + ",";
        windowFeatures = windowFeatures + "top=" + winTop + ",resizable=no" + ",scrollbars";
        var empid = $("#peoplesearchmob").val().split("-")[1];
        if (empid == "" || empid == null) {
            swal({
                title: "",
                text: "Please select an employee from search result to view detail.",
                type: "warning",
            });
            return;
        }
        window.open("../Home/EmployeeDetails?id=" + empid, "", windowFeatures);
    });


    //play video on click functionality
    
    $(document).on('click', '.closeVideo', function () {
        var player = videojs(document.querySelector('.video-js'));
        $("#videoModal").modal('hide');
        player.pause();
        player.currentTime(0);

        $('#videoModal').on('mouseout', function () {
            var owl = $('#owlLeftTop');
            owl.trigger('play.owl.autoplay');
            var owlRight = $('#owlRight');
            owlRight.trigger('play.owl.autoplay');
        });

    });
    // leftTop code end
});




