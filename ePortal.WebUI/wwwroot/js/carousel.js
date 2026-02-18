$(document).ready(function () {

	$('label').hide();
	$("#owlLeftTop").owlCarousel({
		loop:true,
		margin:0, 
		//responsive: true, 
		dots: false,	
		items:1,	
		video:true,
		autoplay:true,
		nav:false,
	    autoplayTimeout: 3000
 	});
	$("#owlLeftBottom").owlCarousel({
		loop:false,
		margin:0, 
		//responsive: true,
		items:7,	
		dots: false,
		slideBy: 7,
		video:true,
		nav:true,
		navText: ["<img src='images/left_arrow_banner.png'>","<img src='images/right_arrow_banner.png'>"]
	});
	$("#owlRight").owlCarousel({
			loop:true,
			margin:15, 
			//responsive: true,
			items:1,	
			nav:false,
			autoplay:true,
			autoplayTimeout: 8000
	});
	$("#owlRightBottom").owlCarousel({
			loop:true,
			margin:15, 
			//responsive: true,
			items:1,	
			nav:false,
			autoplay:true,
			autoplayTimeout: 9000
	});

	
	// Read more modal functionality start
	$('.right_carousel a').on('click', function (event) {
	    event.preventDefault();
	    id = event.target.id;
	    $.ajax({
	        type: "POST",
	        url: "/Login/GetPopDetail",
	        data: { "id": id },
	        //contentType: "application/json;",
	        //dataType: "json",
	        success: function (msg) {
	            document.getElementById("popsubject").innerHTML = msg.SUBJECT;
	            document.getElementById("popdescription").innerHTML = msg.DESCRIPTION;

	            var binary = '';
	            var bytes = new Uint8Array(msg.POPUPHEADER);
	            var len = bytes.byteLength;
	            for (var i = 0; i < len; i++) {
	                binary += String.fromCharCode(bytes[i]);
	            }
	            var datapopupimg = window.btoa(binary);;

	            document.getElementById("modelheader").style.backgroundImage = "url('data:" + msg.POPUPHEADER_CONTENTTYPE + ";base64," + datapopupimg + "')";

	            var description = msg.DESCRIPTION;

                
	            if (description==null)
	                document.getElementById("popdescription").innerHTML = msg.BRIEF;

	            var linkcontent = "<h6>Download :</h6>";
	            var attachment1 = msg.ATTACHMENT1_NAME;
	            var attachment2 = msg.ATTACHMENT2_NAME;
	            if (attachment1 != null)
	                linkcontent = linkcontent + " <a onclick='javascript:OpenDoc(" + id + ")'>" + msg.ATTACHMENT1_NAME + "</a>";
	            if (attachment2 != null)
	                linkcontent = linkcontent + " <a onclick='javascript:OpenAttachement2(" + id + ")'>" + msg.ATTACHMENT2_NAME + "</a>";
	                //linkcontent = linkcontent + " <a href=#>" + msg.ATTACHMENT2_NAME + "</a>";

	            document.getElementById("dwnlink").innerHTML = linkcontent;

	            if (attachment1 == null && attachment2 == null) {
	                document.getElementById("dwnlink").innerHTML = "";
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
	            }
	            else {
	                document.getElementById("popupimage").src = "";
	            }
	        }
	    });


	    $('#myReadMoreModal').modal('show');
	});
	
	$(".close-readmore-modal").click(function(){
		$("#myReadMoreModal").modal('hide');
	});
	
	$(window).on("load",function(){
		
		$("#modal-scroll").mCustomScrollbar({
			theme:"minimal"
		});

	});
	// Read more modal functionality end

	//placeholder functionality
	 $('.input_wrap input[placeholder]').each(function() {
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

         $(this).blur(function() {
             if ($(this).val() != '') {
                 $(this).prev('label').addClass('hideLabel');
             } else {
                 $(this).prev('label').removeClass('hideLabel');
             }
         });
         $(this).focus(function() {
             $(this).prev('label').addClass('hideLabel');
         });
     })

     $('label.onlyIE9').click(function() {
         $(this).next().focus()
     });
	//placeholder functionality
	
	// leftTop code start for sync and carousel pause on video play
	$("#owlLeftBottom .bottomImg").click(function() {
		var owl = $('#owlLeftTop');
		owl.trigger('stop.owl.autoplay');

		// Video synchronization functionality
		var i = $(this).parent('.owl-item').index();
		$('#owlLeftTop').find('.owl-item').removeClass('active');
		$('#owlLeftTop').find('.owl-item').eq(i).addClass('active');
		$('#owlLeftTop').trigger('to.owl.carousel', i);
	});
	
	
	$(".leftTop .owl-item, .bannerVideo").on('mouseover',function(){
		var owl = $('#owlLeftTop');
		owl.trigger('stop.owl.autoplay');
	});
	$(".leftTop .owl-item, .bannerVideo").on('mouseout',function(){
		var owl = $('#owlLeftTop');
		owl.trigger('play.owl.autoplay');
	});
	
	// Display video button on hover functionality
	$('.left').on('mouseenter',function(){
		$( '.leftBottom' ).animate({
			marginTop: "-7em"
		}, 500);
		
		$('.leftTop').addClass('show-overlay');
		$(".bannerVideo").addClass('show');
	
	});
	
	$('.left').on('mouseleave',function() {
		$( '.leftBottom' ).animate({
			marginTop: "-0.25em"
		}, 600);
		
		$('.leftTop').removeClass('show-overlay');
		$(".bannerVideo").removeClass('show');
	});
	
	
	//play video on click functionality
	var player   = videojs(document.querySelector('.video-js'));				
			
	$('.leftTop').on('click', '.bannerVideo', function () {
	    var objvalue = document.getElementById("hdnapiaddress").value;
	    var videoPath = $('#owlLeftTop .owl-item.active').find('.overlay_image');
	    var videoSrc = videoPath.data('src');
	    var sources = [{ "type": "video/mp4", "src": objvalue + videoPath.attr("ID") }];
	    player.src(sources);
		//player.controls = "controls";
		player.load();
		player.play();		
		$('#videoModal').modal('show');

		$("#videoModal").on('mouseover',function(){
			var owl = $('#owlLeftTop');
			owl.trigger('stop.owl.autoplay');
			var owlRight = $('#owlRight');
			owlRight.trigger('stop.owl.autoplay');
		});
		
	});
	
	$(document).on('click', '.closeVideo', function(){
		$("#videoModal").modal('hide');
		player.pause();
		player.currentTime(0);

		$('#videoModal').on('mouseout',function(){
			var owl = $('#owlLeftTop');
			owl.trigger('play.owl.autoplay');
			var owlRight = $('#owlRight');
			owlRight.trigger('play.owl.autoplay');
		});
				
	});
	// leftTop code end
	
// leftBottom code start
	$(".leftBottom #owlLeftBottom .bottomImg:even").on('mouseover', function(){
		$(this).children('span').css({"backgroundColor":"transparent","text-align":"left","margin-left":"0.41em"}).stop(true,false).animate({bottom:"79%"},1000);
	});
	
	$(".leftBottom #owlLeftBottom .bottomImg:even").on('mouseout',function(){
		$(this).children('span').css({"backgroundColor":"rgba(24,172,232,0.7)","text-align":"center","margin-left":"0em"}).animate({bottom:"0%"},1000);
	});
	
	$(".leftBottom #owlLeftBottom .bottomImg:odd").on('mouseover', function(){
		$(this).children('span').css({"backgroundColor":"transparent","text-align":"left","margin-left":"0.41em"});
	});
	
	$(".leftBottom #owlLeftBottom .bottomImg:odd").on('mouseout', function(){
		$(this).children('span').css({"backgroundColor":"rgba(24,172,232,0.7)","text-align":"center","margin-left":"0em"});
	});
	
	$(".leftBottom .bottomImg").on('mouseover', function(){
		$(this).children('.thumbVideo').css("display","block");
		});
	$(".leftBottom .bottomImg").on('mouseout', function(){
		$(this).children('.thumbVideo').css("display","none");
	});
	
	$(".leftBottom").hover(function(){
		$(".leftBottom .owl-theme .owl-nav .owl-prev").show();
		$(".leftBottom .owl-theme .owl-nav .owl-next").show();
		},function(){
			$(".leftBottom .owl-theme .owl-nav .owl-prev").hide();
			$(".leftBottom .owl-theme .owl-nav .owl-next").hide();
	});
	// leftBottom code end	
	
});