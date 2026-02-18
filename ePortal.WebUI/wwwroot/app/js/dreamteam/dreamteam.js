var message = "You can NOT right click here";
var message1 = "PrintScreen is not allowed";
var closeWin = "1";

function IE() {
    if (navigator.appName === "Microsoft Internet Explorer" && (event.button === 2 || event.button === 3)) {
        return false;
    }
}

function NS(e) {
    if (document.layers || (document.getElementById && !document.all)) {
        if (e.which === 2 || e.which === 3) {
            return false;
        }
    }
}

document.onmousedown = IE;
document.onmouseup = NS;
document.oncontextmenu = function () { return false; };

function do_err() {
    return true;
}
window.onerror = do_err;

function no_cp() {
    try {
        if (window.clipboardData) {
            clipboardData.clearData();
        }
    } catch (e) { }
    setTimeout(no_cp, 100);
}
no_cp();

function nokeys() {
    return false;
}

if (document.all) {
    document.onkeydown = nokeys;
}