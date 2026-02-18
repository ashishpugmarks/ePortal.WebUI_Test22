function ShowMessage(message, messagetype) {
    var cssclass;
    switch (messagetype) {
        case 'Success':
            cssclass = 'alert-success'
            break;
        case 'Error':
            cssclass = 'alert-danger'
            break;
        case 'Warning':
            cssclass = 'alert-warning'
            break;
        default:
            cssclass = 'alert-info'
    }
    $('#div_message').empty();
    if (messagetype == 'Error') {
        $('#div_message').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><span class="glyphicon glyphicon-hand-right"></span><span style="font-weight:bold; font-size:15px;"><strong>' + ' ' + messagetype + ':  </strong></span><span>' + message + '</span></div>');
    }
    else if (messagetype == 'Success') {
        $('#div_message').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><span class="glyphicon glyphicon-ok"></span><span style="font-weight:bold; font-size:15px;"><strong>' + ' ' + messagetype + ' :  </strong></span><span>' + message + '</span></div>');
    }
    else if (messagetype == 'Warning') {
        $('#div_message').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><span class="glyphicon glyphicon-record"></span><span style="font-weight:bold; font-size:15px;"><strong>' + ' ' + messagetype + ' :  </strong></span><span>' + message + '</span></div>');
    }
    else {
        $('#div_message').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><span class="glyphicon glyphicon-info-sign"></span><span style="font-weight:bold; font-size:15px;"><strong> Info :  </strong></span><span>' + message + '</span></div>');
    }
}