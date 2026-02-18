$(document).ready(function(){
	$(window).load(function(){
		pendingRequestHeaderHt();
		
		$(window).resize(function(){
			setTimeout(function(){
				pendingRequestHeaderHt();
			}, 500);			
		});
	});
	
	$(window).on("load",function(){
		$.mCustomScrollbar.defaults.theme="light-2"; 
		$(".demo-x").mCustomScrollbar({
					axis:"x",
					advanced:{autoExpandHorizontalScroll:true}
				});
				
	});
	
	$('#tourDetails .panel-heading').on('click',function() {
		if($(this).hasClass('active')){
			$(this).removeClass('active').siblings('#collapseOne').slideUp();
		}
		else {
			$(this).addClass('active').siblings('#collapseOne').slideDown();
		}
	});
	
	$('.accordion').on('click',function(e) {
		e.preventDefault();
		var par_elem =$(this).parent().hasClass('custom-panel1');
		if($(window).width() < 768){
			if($(this).hasClass('activePanel')){
				$(this).removeClass('activePanel').siblings().slideUp();				
				if(par_elem ==true){
					$(this).parent().css('padding-bottom','0');
				};
			}
			else {
				$(this).parents('.approvalContent').find('.accordion').removeClass('activePanel').siblings().slideUp();
				$(this).addClass('activePanel').siblings().slideDown();
				if(par_elem ==true){
					$(this).parent().css('padding-bottom','0.78em');
				};
			}
		}
	});	
	
	if($(window).width() < 768){
		$('.accordion').not('.activePanel').siblings().hide();
	}
	
	if($(window).width() > 768){
		$('.accordion').not('.activePanel').siblings().show();
	}
	
	// Right content pending requests mobile view functionality start
	$('.pending .panel-heading').on('click', function(){
		// alert("yes");
		$(this).parent().toggleClass('active');
	});
	// Right content pending requests mobile view functionality end
	
	function pendingRequestHeaderHt(){
		var _elem_settlement = $('.pending .panel-heading'),
			_elem_settlement_parentHt = _elem_settlement.parent().height();
		if($(window).width() < 768){
			_elem_settlement.width(_elem_settlement_parentHt);		
		}else{
			_elem_settlement.css('width','100%');
		}		
	}
});
	
	