window.addEventListener('DOMContentLoaded', () => {
    // document.querySelectorAll('.homeContent div').forEach(el => {
    //     el.style.removeProperty('padding');
    // });
    for (let sheet of document.styleSheets) {
        try {
            for (let i = 0; i < sheet.cssRules.length; i++) {
                let rule = sheet.cssRules[i];
                if (rule.selectorText === '.homeContent div') {
                    rule.style.removeProperty('padding');
                }
                if (rule.selectorText === '.homeContent') {
                    rule.style.removeProperty('padding');
                }
            }
        } catch (e) {
            // Some stylesheets may be cross-origin and inaccessible
            console.warn("Could not access stylesheet:", e);
        }
    }
});


document.addEventListener("mouseover", function (e) {
    if (e.target.classList.contains("emp-tooltip")) {
        const el = e.target;
        ShowTooltip(
            el.dataset.name,
            el.dataset.op,
            el.dataset.div,
            el.dataset.dept,
            el.dataset.sec,
            el.dataset.img,
            e
        );
    }
});


document.addEventListener("mouseout", function (e) {
    if (e.target.classList.contains("emp-tooltip")) {
        HideTooltip();
    }
});


function ShowTooltip(name, operation, division, department, section, strUrl, event) {
    var img = document.getElementById("uImage");
    if (img) {
        img.src = strUrl;
    }
    document.getElementById("td0").innerText = name;
    document.getElementById("td1").innerText = operation;
    document.getElementById("td2").innerText = division;
    document.getElementById("td3").innerText = department;
    document.getElementById("td4").innerText = section;
    var x = event.clientX + document.body.scrollLeft;
    var y = event.clientY + document.body.scrollTop + 10;
    Popup.style.display = "block";
    Popup.style.left = x + "px";
    Popup.style.top = y + "px";
}

function HideTooltip() {
    Popup.style.display = "none";
}


$(document).ready(function () {
    initializeTables();
});

function initializeTables() {
    var $Confirmations = $('#ConfirmationsTable');
    //var $details = $('#detailTable');

    if ($.fn.DataTable.isDataTable($Confirmations)) $Confirmations.DataTable().destroy();
    //if ($.fn.DataTable.isDataTable($details)) $details.DataTable().destroy();

    $Confirmations.DataTable({ pageLength: 10, lengthChange: true, bFilter: true });
    //$details.DataTable({ lengthChange: false,bFilter: false });
}