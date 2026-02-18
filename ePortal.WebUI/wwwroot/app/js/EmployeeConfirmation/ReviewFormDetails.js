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

// Simple tab behavior to replace AjaxControlToolkit TabContainer
(function () {
    const headerButtons = document.querySelectorAll('.tab-header-btn');
    const panels = document.querySelectorAll('.tab-panel');
    const activeTabField = document.getElementById('ActiveTab');

    function setActiveTab(tabKey) {
        headerButtons.forEach(btn => {
            btn.classList.toggle('active', btn.getAttribute('data-tab') === tabKey);
        });
        panels.forEach(panel => {
            panel.classList.toggle('active', panel.getAttribute('data-tab-panel') === tabKey);
        });
        if (activeTabField) {
            activeTabField.value = tabKey;
        }
    }

    headerButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const tabKey = this.getAttribute('data-tab');
            setActiveTab(tabKey);
        });
    });
})();
