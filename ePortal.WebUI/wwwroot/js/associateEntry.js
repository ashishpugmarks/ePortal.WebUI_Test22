$(document).ready(function(){
	var i=0;

	$(window).load(function(){
		mobileAccordion();		
		pendingRequestHeaderHt();
		
		$(window).resize(function(){
			mobileAccordion();
			setTimeout(function(){
				pendingRequestHeaderHt();
			}, 500);			
		});
	});
	
	$('.accordion .accordion-head').on('click', function(e){
		e.preventDefault();
		if($(window).width() < 768){
			if($(this).parent().hasClass('active')){
				$(this).parent().removeClass('active').children('.accordion-body').slideUp();
			}else{
				$(this).next().slideDown().parent().addClass('active').siblings().removeClass('active').children('.accordion-body').slideUp();
			}
		}			
	});
	
	
	
	
	// Right content pending requests mobile view functionality start
	$('.pending-request-heading').on('click', function(){
		$(this).parent().toggleClass('active');
	});
	// Right content pending requests mobile view functionality end
	
	
	//Dropdown Start
	$(document).on('click', '.custom-select-head', function(e){
		e.preventDefault();						
		if($(this).hasClass('open')){
			$(this).removeClass('open').next().slideUp('fast');
		}else{
			$(this).addClass('open').next().slideDown();
		}
		$('body:not(.custom-select-head)').on('click', function(){
			if($('.custom-select-head').hasClass('open')){
				$('.custom-select-head').removeClass('open').next().slideUp('fast');
			}
		});
	});
	$(document).on('click', '.input-form ul li', function(){
		$(this).parent().slideUp('fast').prev().removeClass('open').val($(this).text());
	});
	//Dropdown End
	
	// Placeholder fix for IE9
	$('.associateEntryContent').find('input, textarea').placeholder();			
	
	// DatePicker Start
	var date = new Date();
	date.setDate(date.getDate());
	
	$('#startDatePicker').datepicker({
		format: "dd/mm/yyyy",
		todayHighlight:'TRUE',
		autoclose: true,
		startDate: date,
		minDate: 0,
		maxDate: '+1Y+6M'
	}).on('changeDate', function (ev) {
		$('#endDatePicker').datepicker('setStartDate', $("#startDatePicker").val());
	});

	$('#endDatePicker').datepicker({
		format: "dd/mm/yyyy",
		todayHighlight:'TRUE',
		autoclose: true,
		startDate: date,
		minDate: 0,
		maxDate: '+1Y+6M'
	}).on('changeDate', function (ev) {
			var start = $("#startDatePicker").val();
			var end = $("#endDatePicker").val();
			var startDate = moment(start, "DD.MM.YYYY");
			var endDate = moment(end, "DD.MM.YYYY");
			var result = endDate.diff(startDate, 'days');
			$(".input-form-days input").val(result);
	});
	
	$('.journeyDatePicker').datepicker({
		format: "dd/mm/yyyy",
		todayHighlight:'TRUE',
		autoclose: true,
		startDate: date,
		minDate: 0,
		maxDate: '+1Y+6M'
	});
	
	$('.stayStartDatePicker, .stayEndDatePicker').datepicker({
		format: "dd/mm/yyyy",
		todayHighlight:'TRUE',
		autoclose: true,
		startDate: date,
		minDate: 0,
		maxDate: '+1Y+6M'
	}).on('changeDate', function (ev) {
			$('.stayEndDatePicker').datepicker('setStartDate', $(".stayStartDatePicker").val());
	});
	
	// DatePicker End
	
	
	//Validate Form on Submit Start
	$('.form-submit,.addField').on('click', function(e){
		e.preventDefault();
		var _container = $(this).closest('.form-validation');

		formValidation(_container);
		var check = formValidation(_container);
		if($(this).hasClass('addField')){
			if(check == true){
				var cloned = $(this).parents('.form').clone();
				var planNumber;
				cloned.find('.addField').addClass('editField').removeClass("addField").find('.add').attr('src','images/icon_edit_white.png').addClass('editImg');
				cloned.find('input').attr('disabled','disabled').css('background','#ebebe4');
				cloned.css('border','none');
				$(this).parents('.form').siblings('.prepend').append(cloned).css('display','block');
				if($(window).width() < 768){
					cloned.find('.accordIcon').removeClass('minus').addClass('plus');
					cloned.find('.planNum').siblings('.child-accordion').css('display','none');						
				}
				$(this).parents('.form')[0].reset();
				planNumber = $(this).parent('.child-accordion').parent('.form').find('.planNum').children('span').html();
				$(this).parent('.child-accordion').siblings('.planNum').children('span').html(++planNumber);
				$('.associateEntryContent').find('input, textarea').placeholder();			
				
				var valArray = [];
				//Edit click functionality	
				$('.ticketDetails .prepend,.stayDetails .prepend').on('click','.form .editField',function() {
					$('.journeyDatePicker').datepicker('update');
					$('.stayStartDatePicker, .stayEndDatePicker').datepicker('update');
					
					//Input values on click of edit field
					$(this).siblings('.form-group').find('input').each(function(){
						valArray[$(this).attr('data-form')] = $(this).val();
					});
					
					$(this).html("").removeClass('editField').addClass('saveDeleteField');
					$(this).html('<span class="saveField"><img src="images/icon_save_white.png" class="saved" alt="saved" title="saved"/></span><span class="deleteField"><img src="images/icon_delete_white.png" class="delete" alt="delete" title="delete"/></span> ');
					$(this).parents('.form').find('input').attr('disabled',false).css('background','');
					
					//Save click functionality	
					$('.ticketDetails,.stayDetails').on('click',' .saveField',function(){
						var _container_cloned = $(this).closest('.form-validation');
						var cloneCheck = formValidation(_container_cloned);
						if(cloneCheck == true) {
							$(this).parents('.form').find('input').attr('disabled','disabled').css('background','#ebebe4');
							var newEdit = $(this).parent().html("").removeClass('saveDeleteField').addClass('editField');
							newEdit.html('<img src="images/icon_edit_white.png" alt="add" title="add" class="add"/>');
							newEdit.find('img').addClass('editImg');
						}
					});
					
					//Delete click functionality	
					$('.ticketDetails,.stayDetails').on('click','.deleteField',function(){
						$(this).parent().siblings('.form-group').find('input').each(function(){
							var formData = $(this).attr('data-form');
							for (var k in valArray){
								if (typeof valArray[k] !== 'function') {
									 if(k == formData)
									 {
										$(this).val(valArray[k]).attr('disabled','disabled').css('background','#ebebe4');
									 }
								}
							}
						});
						var newEdit = $(this).parent().html("").removeClass('saveDeleteField').addClass('editField');
						newEdit.html('<img src="images/icon_edit_white.png" alt="add" title="add" class="add"/>');
						newEdit.find('img').addClass('editImg');
					});
				});
				
				//accordion function for form
				$('.ticketDetails,.stayDetails').off('click').on('click','.accordIcon',function() {
					if($(window).width() < 768){
						if($(this).hasClass('minus')){
							$(this).parent('.planNum').removeClass('active');
							$(this).parent('.planNum').next().slideUp();
							$(this).removeClass('minus').addClass('plus');
						} 
						else if($(this).hasClass('plus')) {
							$(this).parents('.accordion-body').find('.planNum').removeClass('active');
							$(this).parent('.planNum').addClass('active');
							$(this).parents('.accordion-body').find('.planNum').not('.active').find('.accordIcon').removeClass('minus').addClass('plus');
							$(this).removeClass('plus').addClass('minus');
							$(this).parents('.accordion-body').find('.planNum').not('.active').siblings('.child-accordion').slideUp();
							$(this).parent().next().slideDown();							
						}
					}
				});
			}
		}
	});
	//Validate Form on Submit End
				
	//Validation Start
	function formValidation(container){
		var _checkFlag = true,
			_inputWrap = container.find('.form-group'),
			_input = _inputWrap.find('.mandatory');
			
		_inputWrap.removeClass('value-required');
		container.next('.error-list').find('ul li').remove();
		container.next('.error-list').hide();
		
		_input.each(function(){	
			if($(this).val()=='' || $(this).val()==null || $(this).val()==undefined){
				_checkFlag = false;
				$(this).parents('.form-group').addClass('value-required');				
				var _errorList = $(this).data('error'),
					_errorDiv = $(this).parents('.form-validation').next('.error-list').find('ul');
				_errorDiv.append('<li>'+_errorList+'</li>').parent().show();
			}
		});
		
		return _checkFlag;
	}
	//Validation End
	$('.buttons .submit').on('click',function(event){
		$('#myTourModal').modal();
	});
	
	$('.modalEdit').on('click',function(event){
		$('#myTourModal').modal('hide');
	});
	
	function pendingRequestHeaderHt(){
		var _elem_request = $('.pending-request-section .pending-request-heading'),
			_elem_settlement = $('.pending-settlement-section .pending-request-heading'),
			_elem_request_parentHt = _elem_request.parent().height(),
			_elem_settlement_parentHt = _elem_settlement.parent().height();
		if($(window).width() < 768){
			_elem_request.width(_elem_request_parentHt);		
			_elem_settlement.width(_elem_settlement_parentHt);		
		}else{
			_elem_request.width('100%');
			_elem_settlement.width('100%');
		}		
	}
	function mobileAccordion(){
		var _elem = $('.accordion.active');
		if($(window).width() < 768){
			_elem.children('.accordion-body').show().parent().siblings().children('.accordion-body').hide();
		}
		else{
			_elem.children('.accordion-body').show().parent().siblings().children('.accordion-body').show();
		}
	}
	
});