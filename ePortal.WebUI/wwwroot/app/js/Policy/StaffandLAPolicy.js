//function WindowSettings_WithoutMenu(str1, str2) {
//    var winTop = (screen.height / 2) - 325;
//    var winLeft = (screen.width / 2) - 425;
//    var windowFeatures = "location=no,status=no,width=840,height=650";
//    windowFeatures = windowFeatures + ",left = " + winLeft + ",";
//    windowFeatures = windowFeatures + "top=" + winTop + ",resizable" + ",scrollbars";
//    window.open(str1, str2, windowFeatures);
//}


function WindowSettings_WithoutMenu(url, windowName, width, height) {
    // fall back to default size if not provided
    var w = parseInt(width, 10) || 840;
    var h = parseInt(height, 10) || 650;

    var winTop = (screen.height / 2) - (h / 2);
    var winLeft = (screen.width / 2) - (w / 2);

    var windowFeatures = "location=no,status=no";
    windowFeatures += ",width=" + w;
    windowFeatures += ",height=" + h;
    windowFeatures += ",left=" + winLeft;
    windowFeatures += ",top=" + winTop;
    windowFeatures += ",resizable,scrollbars";

    window.open(url, windowName, windowFeatures);
}

document.addEventListener("DOMContentLoaded", function () {
    var links = document.querySelectorAll(".js-policy-link");

    links.forEach(function (link) {
        link.addEventListener("click", function (e) {
            e.preventDefault();

            var url = this.getAttribute("data-url");
            var winName = this.getAttribute("data-window-name") || "policy";
            var width = this.getAttribute("data-width");
            var height = this.getAttribute("data-height");

            if (!url) {
                // nothing to open
                return;
            }

            WindowSettings_WithoutMenu(url, winName, width, height);
        });
    });
});

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