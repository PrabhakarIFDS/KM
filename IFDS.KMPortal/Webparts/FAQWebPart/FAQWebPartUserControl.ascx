<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FAQWebPartUserControl.ascx.cs" Inherits="IFDS.KMPortal.Webparts.FAQWebPart.FAQWebPartUserControl" %>

<script type="text/javascript">
    //<![CDATA[
    var _noDataMessage = '<% =NoDataMessage %>';
    var _userHavePermissionFAQs = '<% =UserHavePermissionFAQ %>';
    //]]>
</script>

<div class ="widgetWrapperHeight" id="faqWidget">
    <div class="panel-heading ">
        <div class="sub-caption">
            <span class="faqicon"></span>
            <span class="panel-title"><%= sfaqWidgetTitle%></span>
            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true"
                title='<%= sfaqTooTip%>'></a>
            <a href='#' class='more-globl' id='view-forum-faq'>View All</a>
            <a href='#' class='post-globl link-bgcolor' id='post-forum-faq'>Post</a>
        </div>
    </div>

    <div class="panel-bgcolor">
        <ul id="tabscontrol" class="tb-border">
            <li class="active"><a data-toggle="tab" href="#PopularFaq"><%= sfaqTab1%></a></li>
            <li><a data-toggle="tab" href="#RecentFaq"><%= sfaqTab2%></a></li>
        </ul>

        <div class="tab-content">
            <div id="PopularFaq" role="tabpanel" class="tab-pane fade in active">
                <div class=" outer faq-scroll">
                    <div class="scrolldiv ">
                        <div class="panel-group" id="accordion">
                            <div id="MostPopularFAQ">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="RecentFaq" role="tabpanel" class="tab-pane fade">
                <div class=" outer faq-scroll">
                    <div class="scrolldiv ">
                        <div class="panel-group" id="accordion">
                            <div id="MostRecentFAQ">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>

</div>
<script>
    jQuery.noConflict();
    (function ($) {

        var mappedUrl;
        var siteURL;
        var listName = '<% =sfaqListName %>';
        var itemCount = '<% =sfaqItemCount %>';
        var errorMessage = '<% =sfaqErrMsg %>';
        var globalSite = '<% =bfaqGlobalSite %>';
        var currentLoggedInUser = "";
        var userid = _spPageContextInfo.userId;
        var webTitle = _spPageContextInfo.webTitle;
        var webPartNameFAQMostPopGlobal = "FAQMostPopularGlobal";
        var webPartNameFAQMostRecGlobal = "FAQMostRecentGlobal";
        var webPartNameFAQMostPopDept = "FAQMostPopularDept";
        var webPartNameFAQMostRecDept = "FAQMostRecentDept";
        var cacheNameFAQ;

        var faqSiteURL = _spPageContextInfo.siteAbsoluteUrl;

        if (faqSiteURL !== undefined || faqSiteURL !== null) {
            siteURL = faqSiteURL;
        }
        else {
            siteURL = '<% =sfaqSiteUrl %>';
        }

        //$(document).ready(function () {
        $(window).load(function () {

            //$.when($.ajax(GetCurrentUser())).then(function () {

                $("#view-forum-faq").css("display", "block");

                if (_userHavePermissionFAQs == 'True') {
                    $('#post-forum-faq').css("display", "block");;
                }
                else {
                    $('#post-forum-faq').css("display", "none");;
                }

                if (globalSite == 'True') {
                    $('a[href="#RecentFaq"]').css("display", "none");
                    $("#tabscontrol").addClass('nav nav-tabs tb-border');
                    cacheNameFAQ = currentLoggedInUser + "_" + webTitle + "_" + webPartNameFAQMostPopGlobal;
                    mappedUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularGlobalFAQData"
                }
                else {
                    $("#tabscontrol").addClass('nav nav-tabs dpttab');
                    cacheNameFAQ = currentLoggedInUser + "_" + webTitle + "_" + webPartNameFAQMostPopDept;
                    mappedUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularDepartmentFAQData"
                }

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: mappedUrl,
                    data: '{ propListName: \'' + listName + '\',propItemCount:\'' + itemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\',userName:\'' + currentLoggedInUser + '\', cacheName:\'' + cacheNameFAQ + '\'}',
                    success: function (msg) {
                        BindFAQData(JSON.parse(msg.d), '#MostPopularFAQ')
                    },
                    error: function (msg) {
                        // alert('error' + msg);
                    }
                });

            //});

        });

        $(window).load(function () {


            $('a[href="#PopularFaq"]').click(function () {

                $("#view-forum-faq").css("display", "block");

                if (_userHavePermissionFAQs == 'True') {
                    $('#post-forum-faq').css("display", "block");;
                }
                else {
                    $('#post-forum-faq').css("display", "none");;
                }

                if (globalSite == 'True') {
                    $("#tabscontrol").addClass('nav nav-tabs tb-border');
                    cacheNameFAQ = currentLoggedInUser + "_" + webTitle + "_" + webPartNameFAQMostPopGlobal;
                    mappedUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularGlobalFAQData"
                }
                else {
                    $("#tabscontrol").addClass('nav nav-tabs dpttab');
                    cacheNameFAQ = currentLoggedInUser + "_" + webTitle + "_" + webPartNameFAQMostPopDept;
                    mappedUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularDepartmentFAQData"
                }

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: mappedUrl,
                    data: '{ propListName: \'' + listName + '\',propItemCount:\'' + itemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\',userName:\'' + currentLoggedInUser + '\', cacheName:\'' + cacheNameFAQ + '\'}',
                    success: function (msg) {
                        BindFAQData(JSON.parse(msg.d), '#MostPopularFAQ')
                    },
                    error: function (msg) {
                        // alert('error' + msg);
                    }
                });
            });

            $('a[href="#RecentFaq"]').click(function () {

                if (globalSite == 'True') {
                    $("#tabscontrol").addClass('nav nav-tabs tb-border');
                    cacheNameFAQ = currentLoggedInUser + "_" + webTitle + "_" + webPartNameFAQMostRecGlobal;
                    $("#view-forum-faq").css("display", "none");
                    $("#post-forum-faq").css("display", "none");
                    mappedUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostRecentGlobalFAQData"
                } else {
                    $("#tabscontrol").addClass('nav nav-tabs dpttab');
                    cacheNameFAQ = currentLoggedInUser + "_" + webTitle + "_" + webPartNameFAQMostRecDept;
                    $("#view-forum-faq").css("display", "block");
                   
                    if (_userHavePermissionFAQs == 'True') {
                        $('#post-forum-faq').css("display", "block");;
                    }
                    else {
                        $('#post-forum-faq').css("display", "none");;
                    }

                    mappedUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostRecentDepartmentFAQData"
                }

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: mappedUrl,
                    data: '{ propListName: \'' + listName + '\',propItemCount:\'' + itemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\',userName:\'' + currentLoggedInUser + '\', cacheName:\'' + cacheNameFAQ + '\'}',
                    success: function (msg) {
                        if (globalSite == 'True') {
                            BindFAQGlobalRecentData(JSON.parse(msg.d), '#MostRecentFAQ')
                        } else {
                            BindFAQDeptRecentData(JSON.parse(msg.d), '#MostRecentFAQ')
                        }
                    },
                    error: function (msg) {
                        // alert('error' + msg);
                    }
                });

            });

        });

        function BindFAQData(jsonData, controlID) {
            var viewFaq;
            var postFaq;
            var html = [];

            var errorMessage = jsonData[3].Message;

            if (errorMessage.length > 0) {
                viewFaq = jsonData[1].View;
                postFaq = jsonData[2].Post;
                $('#view-forum-faq').attr('href', viewFaq);
                $('#post-forum-faq').attr('href', postFaq);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    viewFaq = jsonData[1].View;
                    postFaq = jsonData[2].Post;
                    $('#view-forum-faq').attr('href', viewFaq);
                    $('#post-forum-faq').attr('href', postFaq);
                    $(controlID).html('');
                    $(controlID).append(_noDataMessage);
                }
                else {
                    viewFaq = jsonData[1].View;
                    postFaq = jsonData[2].Post;
                    var i = 1;
                    $.each(jsonData[0].Results, function (index, element) {
                        html = html + '<div class="panel panel-default faq-acordian"><div class="panel-heading-faq">'
                                    + '<a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapsePop' + i + '">'
                                    + '<h4 class="panel-title"><i class="acc-con"><span class="glyphicon glyphicon-chevron-down"></span></i>'
                                    + '<p class="accHeadtxt"><span class="glyphicon glyphicon-time globalFAQ-time" aria-hidden="true"><span class="icon-txt-faq">' + element.LastModifiedTime + ' </span></span>'
                                    + '<a href=' + element.FileRef + ' class="title-text">'
                                    + element.Title
                                    + '</a></p></h4></a></div><div id="collapsePop' + i + '" class="panel-collapse collapse in">'
                                    + '<div class="panel-body acc-content"><p>' + element.Answer + '</p>'
                                    + '</div></div></div>'
                        i = i + 1;
                    });

                    $('#view-forum-faq').attr('href', viewFaq);
                    $('#post-forum-faq').attr('href', postFaq);
                    $(controlID).html(html);
                }
        }

        function BindFAQGlobalRecentData(jsonData, controlID) {
            var viewFaqGlobal;
            var postFaqGlobal;

            var html = [];

            var errorMessage = jsonData[3].Message;

            if (errorMessage.length > 0) {
                viewFaqGlobal = jsonData[1].View;
                postFaqGlobal = jsonData[2].Post;
                $('#view-forum-faq').attr('href', viewFaqGlobal);
                $('#post-forum-faq').attr('href', postFaqGlobal);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    viewFaqGlobal = jsonData[1].View;
                    postFaqGlobal = jsonData[2].Post;
                    $('#view-forum-faq').attr('href', viewFaqGlobal);
                    $('#post-forum-faq').attr('href', postFaqGlobal);
                    $(controlID).html('');
                    $(controlID).append(_noDataMessage);
                }
                else {
                    viewFaqGlobal = jsonData[1].View;
                    postFaqGlobal = jsonData[2].Post;
                    var k = 1;
                    $.each(jsonData[0].Results, function (index, element) {
                        html = html + '<div class="panel panel-default faq-acordian"><div class="panel-heading-faq">'
                                    + '<a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#GlobalFAQcollapseRecent' + k + '">'
                                    + '<h4 class="panel-title"><i class="acc-con"><span class="glyphicon glyphicon-chevron-down"></span></i>'
                                    + '<p class="accHeadtxt"><span class="glyphicon glyphicon-time globalFAQ-time" aria-hidden="true"><span class="icon-txt-faq">' + element.LastModifiedTime + ' </span></span>'
                                    + '<span class="blue">' + element.DepartmentName + '</span>'
                                    + '<a href=' + element.FileRef + ' class="title-text">'
                                    + element.Title
                                    + '</a></p></h4></a></div><div id="GlobalFAQcollapseRecent' + k + '" class="panel-collapse collapse in">'
                                    + '<div class="panel-body acc-content"><p>' + element.Answer + '</p>'
                                    + '</div></div></div>'
                        k = k + 1;
                    });
                    $('#view-forum-faq').attr('href', viewFaqGlobal);
                    $('#post-forum-faq').attr('href', postFaqGlobal);
                    $(controlID).html(html);
                }
        }

        function BindFAQDeptRecentData(jsonData, controlID) {
            var viewFaqDept;
            var postFaqDept;
            var html = [];

            var errorMessage = jsonData[3].Message;

            if (errorMessage.length > 0) {
                viewFaqDept = jsonData[1].View;
                postFaqDept = jsonData[2].Post;
                $('#view-forum-faq').attr('href', viewFaqDept);
                $('#post-forum-faq').attr('href', postFaqDept);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    viewFaqDept = jsonData[1].View;
                    postFaqDept = jsonData[2].Post;
                    $('#view-forum-faq').attr('href', viewFaqDept);
                    $('#post-forum-faq').attr('href', postFaqDept);
                    $(controlID).html('');
                    $(controlID).append(_noDataMessage);
                }
                else {
                    viewFaqDept = jsonData[1].View;
                    postFaqDept = jsonData[2].Post;
                    var j = 1;
                    $.each(jsonData[0].Results, function (index, element) {
                        html = html + '<div class="panel panel-default faq-acordian"><div class="panel-heading-faq">'
                                    + '<a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#DeptcollapseRecent' + j + '">'
                                    + '<h4 class="panel-title"><i class="acc-con"><span class="glyphicon glyphicon-chevron-down"></span></i>'
                                    + '<p class="accHeadtxt"><span class="glyphicon glyphicon-time globalFAQ-time" aria-hidden="true"><span class="icon-txt-faq">' + element.LastModifiedTime + ' </span></span>'
                                    + '<a href=' + element.FileRef + ' class="title-text">'
                                    + element.Title
                                    + '</a></p></h4></a></div><div id="DeptcollapseRecent' + j + '" class="panel-collapse collapse in">'
                                    + '<div class="panel-body acc-content"><p>' + element.Answer + '</p>'
                                    + '</div></div></div>'
                        j = j + 1;
                    });
                    $('#view-forum-faq').attr('href', viewFaqDept);
                    $('#post-forum-faq').attr('href', postFaqDept);
                    $(controlID).html(html);
                }
        }

    })(jQuery);



</script>

