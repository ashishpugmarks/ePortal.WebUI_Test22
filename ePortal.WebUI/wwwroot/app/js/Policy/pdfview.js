var message = "You can NOT right click here"; // Your no right click message here
var message1 = "PrintScreen is not allowed";
var closeWin = "1"; // Do you want to close window after message (1 for yes 0 for no)
function IE() {
    if (navigator.appName == "Microsoft Internet Explorer" && (event.button == "2" || event.button == "3")) {
        //                alert(message); 
        //                if(closeWin == "1") self.close();
        return false;
    }

}

function NS(e) {
    if (document.layers || (document.getElementById && !document.all)) {
        if (e.which == "2" || e.which == "3") {
            //                alert(message); 
            //                if(closeWin == "1") self.close();
            return false;
        }
    }
}

document.onmousedown = IE;
document.onmouseup = NS;
document.oncontextmenu = new Function("return false");

function do_err() {
    return true
}
onerror = do_err;

function no_cp() {
    clipboardData.clearData(); setTimeout("no_cp()", 100)
}
no_cp();

//var keyesMessage="YOU DONT HAVE PERMISSION";
function nokeys() {
    if (document.all) {
        //alert(keyesMessage);
        return false;
    }
}
if (document.all) {
    document.onkeydown = nokeys;
}