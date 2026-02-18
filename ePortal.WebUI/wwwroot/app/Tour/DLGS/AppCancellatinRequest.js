// ------------------------------ Globals -----------------------------------------------------------
$(".homeContent").removeClass("homeContent");

$(document).ready(function () { 
	$('#btnCancel').on('click', function (e) {
        e.preventDefault();
        if (validentry()) {
            btnCancel_Click();
        }
    });

	$('#btnClose').on('click', function (e) {
        e.preventDefault();
		javascript:window.returnValue=0;
        javascript:window.close();
	});
});


function validentry()
{
	 if (document.getElementById("txtCalcellationRemarks").value == "")
	{
		alert("Cancellation Approval Remarks is required field.");
		document.getElementById("txtCalcellationRemarks").focus();
		return false;
	}
    else
    return true;
}

function Close()
{
	window.returnValue=0;
	window.close();
}

function btnCancel_Click() {
   const remarks = ($('#txtCalcellationRemarks').val() || '').trim();
  return $.ajax({
    url: '/TourDLGS/btnCancel_Click_ACR',
    type: 'POST',
    contentType: 'application/json; charset=utf-8',
    dataType: 'json',
    data:  JSON.stringify(remarks)   
  })
  .done(function (resp) {
    const err = (resp && resp.result && resp.result.err || '').trim();
    if (err) {
      $('#lblErrMsg').text(err);
    } else {
      window.returnValue = 1;
      window.close();
    }
  })
  .fail(function (xhr) {
    const msg =
      (xhr.responseJSON && (xhr.responseJSON.error || xhr.responseJSON.message)) ||
      'Something went wrong. Please try again.';
    $('#lblErrMsg').text(msg);
  });
}