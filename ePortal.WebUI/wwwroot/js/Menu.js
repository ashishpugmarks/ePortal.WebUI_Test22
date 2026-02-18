//Menu Code
$(document).ready(function () {
    {
        $.ajax({
            type: "GET",
            url: "/Home/GetMenu",
            dataType: "json",
            cache: false,
            success: function (data) {
                var res1 = data.SubMenuItems;
                var mainmenu = res1.filter(function (entry) { return entry.MenuParentId === 0 && entry.MenuLevel === 1 })
                var innerHTML1 = "";
                innerHTML1 += '<div class="col-sm-2 search_menu pull-right"><input class="search_input" id="txtMenuSearch" placeholder="Search for Menu" /><i data-ctrl="clear-search" class="clearable__clear">&times;</i></div>';
                innerHTML1 += '<div class="col-xs-2 close_btn pull-left"><img src=' + window.location.origin + '/images/icon_menu_close.png alt="close" title="close" class="close_img" onclick="HideMobileMenu();" /></div>';
                $.each(mainmenu, function (i, item) {
                    var var1 = res1.filter(function (entry) { return entry.MenuParentId === item.MenuId && entry.MenuLevel == 2 });

                    //If 1st level menu doesn't contain any sub menu then use class 'main_nav_list1' to remove menu drop image else use class 'main_nav_list2' to show menu drop image
                    if (var1.length == 0) {
                        innerHTML1 += '<li class="main_nav_list1">';

                        if (item.MenuParentId == 0) {
                            var url_title1 = item.URL == null ? "#" : item.URL;
                            innerHTML1 += '<a href="' + url_title1 + '" target="' + item.MenuTarget + '">' + item.Title + '</a>';
                        }
                        innerHTML1 += '</li>';
                    }
                    else {

                        innerHTML1 += '<li class="main_nav_list2">';

                        if (item.MenuParentId == 0) {
                            var url_title1 = item.URL == null ? "#" :  item.URL;
                            innerHTML1 += '<a href="' + url_title1 + '" target="' + item.MenuTarget + '">' + item.Title + '</a>';
                        }
                        var submenu1 = res1.filter(function (entry) { return entry.MenuParentId === item.MenuId && entry.MenuLevel == 2 });
                        if (submenu1 != null && submenu1.length > 0) {
                            innerHTML1 += '<ul class="col-xs-12 secondary_list" id="' + url_title1 + '">';
                            $.each(submenu1, function (i, item2) {
                                var url_title2 = item2.URL == null ? "#" : item2.URL;
                                innerHTML1 += '<li><a href="' + url_title2 + '" target="' + item2.MenuTarget + '">' + item2.Title + '</a>';

                                var submenu2 = res1.filter(function (entry) { return entry.MenuParentId === item2.MenuId && entry.MenuLevel == 3 });
                                if (submenu2 != null && submenu2.length > 0) {
                                    innerHTML1 += '<ul class="fatnav_list" id="' + url_title2 + '">';
                                    $.each(submenu2, function (i, item3) {
                                        var imgSrc = '';
                                        if (item3.MenuIcon != null) {
                                            //var binary = '';
                                            //var bytes = new Uint8Array(item3.MenuIcon);
                                            //var len = bytes.byteLength;
                                            //for (var i = 0; i < len; i++) {
                                            //    binary += String.fromCharCode(bytes[i]);
                                            //}
                                            //var dataimg = window.btoa(binary);
                                            //imgSrc = "data:" + item3.MenuContentType + ";base64," + dataimg;
                                            imgSrc = "data:" + item3.MenuContentType + ";base64," + item3.MenuIcon.trim();
                                        }
                                        else {
                                            imgSrc = window.location.origin + "/images/icon1.png";
                                        }

                                        var url_title3 = item3.URL == null ? "#" : item3.URL;
                                        innerHTML1 += '<li><img style="vertical-align:middle;width:23px;height:25px;" src="' + imgSrc + '"/>&nbsp;<span style="color:black;"><a href="' + url_title3 + '" target="' + item3.MenuTarget + '">' + item3.Title + '</a></span>';

                                        var submenu3 = res1.filter(function (entry) { return entry.MenuParentId === item3.MenuId && entry.MenuLevel == 4 });
                                        if (submenu3 != null && submenu3.length > 0) {
                                            innerHTML1 += '<ul class="fatnav_sublist" id="' + url_title3 + '">';
                                            $.each(submenu3, function (i, item4) {
                                                var url_title4 = item4.URL == null ? "#" : item4.URL;
                                                innerHTML1 += '<li><a href="' + url_title4 + '" target="' + item4.MenuTarget + '">' + item4.Title + '</a></li>';
                                            });
                                            innerHTML1 += '</ul>';
                                        }
                                        innerHTML1 += '</li>';
                                    });
                                    innerHTML1 += '</ul>';
                                }
                                innerHTML1 += '</li>';
                            });
                            innerHTML1 += '</ul>';
                        }
                        innerHTML1 += '</li>';
                    }
                });
                innerHTML1 += '</ul>';
                if (document.getElementById("ul_html"))
                    document.getElementById("ul_html").innerHTML = innerHTML1;
                $('#txtMenuSearch').on('keyup', function () {
                    GetMenuBySearchText();
                });
                $('[data-ctrl="clear-search"]').on('click', function () {
                    resetSearchText();
                });
                bindMenuLinks(res1);
            },
            error: function (xhr, status, error) {
                window.location.href = "Login";
            }
        });

        ////////////// Bind menu links for Search////////////////
        function bindMenuLinks(menuList) {
            var linksHTML = "";
            var iList = menuList.filter(function (entry) { return entry.URL != null })
            $.each(iList, function (i, itemVal) {
                var url = itemVal.URL == null ? "#" : itemVal.URL;
                linksHTML += '<li><a href="' + url + '" target="' + itemVal.MenuTarget + '">' + itemVal.Title + '</a>';
                if (document.getElementById("myUL"))
                    document.getElementById("myUL").innerHTML = linksHTML;
                return;
            });
        }
    }
});