function WindowSettings_WithoutMenu(str1, str2) {
    var winTop = (screen.height / 2) - 325;
    var winLeft = (screen.width / 2) - 425;
    var windowFeatures = "location=no,status=no,width=840,height=650";
    windowFeatures = windowFeatures + ",left = " + winLeft + ",";
    windowFeatures = windowFeatures + "top=" + winTop + ",resizable" + ",scrollbars";
    window.open(str1, str2, windowFeatures);
}


document.addEventListener("DOMContentLoaded", function () {
    var links = document.querySelectorAll(".js-policy-link");

    links.forEach(function (link) {
        link.addEventListener("click", function (e) {
            e.preventDefault();

            var str1 = this.getAttribute("data-url");
            var str2 = this.getAttribute("data-window-name") || "policy";

            WindowSettings_WithoutMenu(str1, str2);
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