function GENINFOCLICK() {
    
    $("#GENINFO").css("background-color", "#BFE0A4");
    $("#BANKDET, #TAXDET, #WTHOLDING").css("background-color", "#DADADA");

    $("#GENINFODIV").show();
    $("#BANKDETDIV, #TAXDETDIV, #HOLDINGDIV").hide();
}

function BANKDETCLICK() {
    
    $("#BANKDET").css("background-color", "#BFE0A4");
    $("#GENINFO, #TAXDET, #WTHOLDING").css("background-color", "#DADADA");

    $("#BANKDETDIV").show();
    $("#GENINFODIV, #TAXDETDIV, #HOLDINGDIV").hide();
}

function TAXDETCLICK() {
    $("#TAXDET").css("background-color", "#BFE0A4");
    $("#GENINFO, #BANKDET, #WTHOLDING").css("background-color", "#DADADA");

    $("#TAXDETDIV").show();
    $("#GENINFODIV, #BANKDETDIV, #HOLDINGDIV").hide();
}

function HOLDINGTAXDATACLICK() {
    $("#WTHOLDING").css("background-color", "#BFE0A4");
    $("#GENINFO, #BANKDET, #TAXDET").css("background-color", "#DADADA");

    $("#HOLDINGDIV").show();
    $("#GENINFODIV, #BANKDETDIV, #TAXDETDIV").hide();
}

$(function () {
    // Section tabs
    $("#GENINFO").on("click", GENINFOCLICK);
    $("#BANKDET").on("click", BANKDETCLICK);
    $("#TAXDET").on("click", TAXDETCLICK);
    $("#WTHOLDING").on("click", HOLDINGTAXDATACLICK);

    // Unified handler for all preview/document links
    $(document).on("click", ".docpreview, .doc-link", function (e) {
        e.preventDefault();

        var href = $(this).attr("href");
        if (href && href.trim() !== "") {
            var target = $(this).attr("target") || "_blank";
            OpenFile(href, target);
        } else {
            alert("No file attached for preview.");
        }
    });
});

function OpenFile(href, target) {
    window.open(href, target, "left=150,top=20,width=800,height=600,toolbar=0,resizable=0");
}

