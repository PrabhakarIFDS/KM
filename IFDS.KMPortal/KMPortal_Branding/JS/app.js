
$(document).ready(function () {
    $("#suiteBarRight").appendTo(".NewWelcomeMenu");
    $("#welcomeMenuBox").show();
    $(".ms-core-listMenu-item:contains('Recent')").parent().hide();
    //$('[data-toggle="tab"]').bind('shown.bs.tab', function (e) {
    //  updateviewallurl(e);
    //});


    // Taxonomy Left Navigation
    var siteurl = _spPageContextInfo.webServerRelativeUrl;
    var $checkableTree = $('#ProductTreeview').treeview({
        expandIcon: "glyphicon glyphicon-plus",
        collapseIcon: "glyphicon glyphicon-minus",
        highlightSelected: true,
        showCheckbox: true,
        data: treeview1json[0],
        multiSelect: true,
        enableLinks: true,
        onNodeChecked: function (event, node) {
            var children = node.nodes;
            if (children) {
                for (var i = 0; i < children.length; i++) {
                    var childNode = children[i];
                    var nodeId = childNode['nodeId'];
                    $checkableTree.treeview('checkNode', nodeId);
                }
            }


        },
        onNodeUnchecked: function (event, node) {
            var children = node.nodes;
            if (children) {
                for (var i = 0; i < children.length; i++) {
                    var childNode = children[i];
                    var nodeId = childNode['nodeId'];
                    $checkableTree.treeview('uncheckNode', nodeId);
                }
            }
        }       
    });

    var $checkableTree2 = $('#DepartmentTreeview').treeview({
        expandIcon: "glyphicon glyphicon-plus",
        collapseIcon: "glyphicon glyphicon-minus",
        highlightSelected: true,
        showCheckbox: true,
        data: treeview2json[0],
        multiSelect: true,
        enableLinks: true,
        onNodeChecked: function (event, node) {
            var children = node.nodes;
            if (children) {
                for (var i = 0; i < children.length; i++) {
                    var childNode = children[i];
                    var nodeId = childNode['nodeId'];
                    $checkableTree2.treeview('checkNode', nodeId);
                }
            }


        },
        onNodeUnchecked: function (event, node) {
            var children = node.nodes;
            if (children) {
                for (var i = 0; i < children.length; i++) {
                    var childNode = children[i];
                    var nodeId = childNode['nodeId'];
                    $checkableTree2.treeview('uncheckNode', nodeId);
                }
            }
        }        
    });

    $('#ProductTreeview').treeview('collapseAll', { silent: true });
    $('#DepartmentTreeview').treeview('collapseAll', { silent: true });



    //$("#carousel .carousel-inner .item").length) > 0 {
    $("#carousel").carousel();
    //}


    // faq collapse
    //$('.collapse').on('shown.bs.collapse','click', function(){
    //$(this).parent().find(".glyphicon-chevron-right").removeClass("glyphicon-chevron-right").addClass("glyphicon-chevron-down");
    //}).on('hidden.bs.collapse', function(){
    //$(this).parent().find(".glyphicon-chevron-down").removeClass(" glyphicon-chevron-down").addClass("glyphicon-chevron-right");
    //});
    $('.faq-acordian ').on('.acc-con', 'click', function (e) {
        if ($(this).find('.accordion-toggle').hasClass('collapsed')) {
            $(this).parents('.faq-acordian').find('.glyphicon').removeClass("glyphicon-chevron-right").addClass("glyphicon-chevron-down")

        }
        else {
            $(this).parents('.faq-acordian').find('.glyphicon').removeClass("glyphicon-chevron-right").addClass("glyphicon-chevron-down")
        }
    });
    // end faq collaps


    //my favs viewall
    $("#view-news").click(function () {
        $("#home").hide();
        $("#favAll").show();
    });
    //discussion forum view all

    $("#view-forum").click(function () {
        $("#home").hide();
        $("#discussAll").show();
    });

    //top contibuters view all
    $("#view-topcontributer").click(function () {
        $("#home").hide();
        $("#top-contributer").show();
    });

    //Faqs view all

    $("#view-faqs").click(function () {
        $("#home").hide();
        $("#faqsAll").show();
    });

    $("#search").click(function () {
        $("#searchResult").hide();
        $("#resultPage").show();
    });
    $("#BusinessSearch a").click(function () {
        $("#searchResult").hide();
        $("#resultPage").show();
    });

    $(".u-vmenu").vmenuModule({
        Speed: 100,
        autostart: false,
        autohide: true
    });    

    getMySiteUrl();

});


var mySiteURL;
function getMySiteUrl() {
    //Get the current user's account information
    $.ajax({
        url: _spPageContextInfo.webAbsoluteUrl + "/_api/SP.UserProfiles.PeopleManager/GetMyProperties?$select=UserUrl",
        async: false,
        method: "GET",
        headers: {
            "accept": "application/json;odata=verbose",
        },
        success: function (data) {
            var userUrl = data.d.UserUrl;
            mySiteURL = userUrl;
        },
        error: function (err) {
        }
    });
}




(function ($) {
    $.fn.vmenuModule = function (option) {
        var obj,
			item;
        var options = $.extend({
            Speed: 100,
            autostart: true,
            autohide: 0
        },
			option);
        obj = $(this);

        item = obj.find("ul").parent("li").children("i");
        item.attr("data-option", "off");

        item.unbind('click').on("click", function () {
            var i = $(this);
            if (options.autohide) {
                i.parent().parent().find("i[data-option='on']").parent("li").children("ul").slideUp(options.Speed / 1.2,
					function () {
					    $(this).parent("li").children("i").attr("data-option", "off");
					    leftscroll();
					})
            }
            if (i.attr("data-option") == "off") {
                var plft = parseInt(i.prev().css('padding-left'), 10);

                i.parent("li").children("ul").find('a').each(function () {
                    $(this).css('padding-left', plft + 10 + 'px');
                    var lft = parseInt(i.css('left'), 10);
                    $(this).next('i').css('left', lft + 10 + 'px');
                });
                i.parent("li").children("ul").slideDown(options.Speed,
					function () {
					    i.attr("data-option", "on");
					    leftscroll();
					});
            }
            if (i.attr("data-option") == "on") {
                i.attr("data-option", "off");
                i.parent("li").children("ul").slideUp(options.Speed)
            }
        });
        if (options.autostart) {
            obj.find("i").each(function () {

                $(this).parent("li").parent("ul").slideDown(options.Speed,
					function () {
					    $(this).parent("li").children("i").attr("data-option", "on");
					    leftscroll();
					})
            })
        }

    }
})(window.jQuery || window.Zepto);

// For Mail Triggering
function TriggerOutlook(itemFullUrl) {
    var subject = "Document details";
    var body = "Please follow this link to download this document. "
    body += itemFullUrl;
    window.location.href = "mailto:?subject=" + subject + "&body=" + body + "";
}


$(window).load(function () {

    /*Welcome Menu hover */
    $('[id^=zz][id$=_Menu_t]').find('span.ms-core-menu-arrow,img').css('visibility', 'hidden');
    $('[id^=zz][id$=_Menu_t]').find('a.ms-core-menu-root').attr('title', '');
    $("img.img-circle").append('<a id="userAboutMe"></a>');
    /* Ends -Welcome Menu hover */

    /* User Image redirection*/

    var aboutMeHref = mySiteURL;
    $("img.img-circle").attr('usemap', '#userImg');
    var mapHtml = '<map name="userImg"><area shape="circle" coords="0,0,42,42" alt="About Me" href="' + aboutMeHref + '" target="_blank"></map>';
    $("img.img-circle").after(mapHtml);

    /* Ends - User Image redirection*/

    $.mCustomScrollbar.defaults.theme = "light-3"; //set "inset" as the default theme
    $.mCustomScrollbar.defaults.scrollButtons.enable = true; //enable scrolling buttons by default


    $(".scrolldiv,.scrolldivnot").mCustomScrollbar();
    leftscroll();
    var pageURL = _spPageContextInfo.serverRequestPath;
    if (pageURL.indexOf("/Pages/") == -1) {
        ExecuteOrDelayUntilScriptLoaded(HitCounter, "sp.js");
    }

});
function leftscroll() {
    var mAx = 10;
    $('.left-menubox').each(function () {

        mAx = mAx + $(this).height();
    });
    // alert(mAx);
    if (mAx > 543) {

        $('.left-menubox').each(function () {
            $(this).find('ul.ulnav').mCustomScrollbar();
        });
    }
}
function killCopy() {
    return false
}
function reEnable() {
    return true
}

function HitCounter() {

    var ctx = new SP.ClientContext.get_current();
    var web = ctx.get_web();
    ctx.load(web);
    ctx.executeQueryAsync();
    user = web.get_currentUser();
    ctx.load(user);
    list = web.get_lists().getById(_spPageContextInfo.pageListId);
    ctx.load(list, 'Title', 'Id');
    ctx.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
}

function onQuerySucceeded() {

    var hitcountdata = { logurl: document.URL, listname: this.list.get_title(), member: this.user.get_loginName(), webname: _spPageContextInfo.webTitle, displayname: this.user.get_title() };
    //console.log(hitcountdata);
    //console.log(JSON.stringify(hitcountdata));
    $.ajax({
        type: "POST",
        async: false,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(hitcountdata),
        url: "/_Layouts/15/IFDS.KMPortal/WebMethodPage.aspx/LogURL",
        success: function (msg) {
            //console.log(msg);

        },
        error: function (msg) {
            console.log('error' + msg);
        }
    });
}

function onQueryFailed(sender, args) {
    console.log('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
}
