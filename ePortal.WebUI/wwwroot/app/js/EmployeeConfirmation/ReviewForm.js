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

// recomputes total score on client (simpler than WebForms version)
function getRating() {
    var totalScore = 0;
    var selects = document.querySelectorAll("select[data-rating='true']");

    for (var i = 0; i < selects.length; i++) {
        var val = selects[i].value;
        if (val && val !== "-1") {
            totalScore += parseInt(val);
        }
    }

    var hiddenTotal = document.getElementById("Hdn_TotalScore");
    if (hiddenTotal) hiddenTotal.value = totalScore;

    var lblTotalScore = document.getElementById("lblTotalScore");
    if (lblTotalScore) lblTotalScore.textContent = totalScore;
    // If you want, you can also compute percentage & average here,
    // or continue to compute them on the server in your POST action.
}

function WindowSettings_WithoutMenu(str1, str2, width, height) {
    var winTop = (screen.height / 2) - 325;
    var winLeft = (screen.width / 2) - 425;
    var windowFeatures = "location=no,status=no,width=" + width + ",height=" + height;
    windowFeatures = windowFeatures + ",left=" + winLeft + ",";
    windowFeatures = windowFeatures + "top=" + winTop + ",resizable=no,scrollbars";
    window.open(str1, str2, windowFeatures);
}