<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionListWebPartUserControl.ascx.cs" Inherits="IFDS.KMPortal.Webparts.DiscussionListWebPart.DiscussionListWebPartUserControl" %>

<script type="text/javascript">
    //<![CDATA[
    var _noDataMessage = '<% =NoDataMessage %>';
    var _userHavePermissionDiscussions = '<% =UserHavePermissionDiscussion %>';
    //]]>
</script>

<div class ="widgetWrapperHeight" id="discussionWidget">
<div class="panel-heading ">
   
        <div class="sub-caption">
            <span class="forumicon"></span>
            <span class="panel-title"><%= sDiscussionWidgetTitle%></span>
            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true" 
                title='<%= sDiscussionToolTip%>'></a>
            <a href='#' class='more-globl' id='view-forum-disc'>View All</a>
            <a href='#' class='post-globl link-bgcolor' id='post-forum-disc'>Post</a>
    </div>

</div>

<div class="panel-bgcolor">
    <ul id="discTabscontrol" class="tb-border">
	  <li class="active"><a data-toggle="tab" href="#Popular"><%= sDiscussionTab1%></a></li>
		<li><a data-toggle="tab" href="#Recent"><%= sDiscussionTab2%></a></li>
	</ul>

    <div class="tab-content">

        <div id="Popular" class="tab-pane fade in active">
            <div class="outer faq-scroll">
                <div class="scrolldiv">
                        <div id="MostPopularDiscussion">
                        </div>
                </div>
            </div>
        </div>

        <div id="Recent" class="tab-pane fade">
            <div class=" outer forum-scroll">
                <div class="scrolldiv">
                        <div id="MostRecentDiscussion">
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

        var mappedDiscUrl;
        var siteURL;
        var discussionListName = '<% =sDiscussionListName %>';
        var ItemCount = '<% =sDiscussionItemCount %>';
        var errorMessage = '<% =sDiscussionErrorMessage %>';
        var discGlobalSite = '<% =bdiscGlobalSite %>';
        var currentLoggedInUser = "";
        var userid = _spPageContextInfo.userId;
        var webTitle = _spPageContextInfo.webTitle;
        var webPartNameMostPopGlobal = "DiscussionMostPopularGlobal";
        var webPartNameMostRecGlobal = "DiscussionMostRecentGlobal";
        var webPartNameMostPopDept = "DiscussionMostPopularDepartment";
        var webPartNameMostRecDept = "DiscussionMostRecentDepartment";
        
        var cacheNamedisc;
        

        var deptSiteURL = _spPageContextInfo.siteAbsoluteUrl;
       
        if (deptSiteURL !== undefined || deptSiteURL !== null) {
            siteURL = deptSiteURL;
        }
        else {
            siteURL = '<% =sDiscussionSiteURL %>';
        }

        $(document).ready(function () {

            $("#view-forum-disc").css("display", "block");

            if (discGlobalSite == 'True') {
                $('a[href="#Recent"]').css("display", "none");
                $("#discTabscontrol").addClass('nav nav-tabs tb-border');
                cacheNamedisc = currentLoggedInUser + "_" + webTitle + "_" + webPartNameMostPopGlobal;
                mappedDiscUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularGlobalDiscussions"
            }
            else {
                $("#discTabscontrol").addClass('nav nav-tabs dpttab');
                cacheNamedisc = currentLoggedInUser + "_" + webTitle + "_" + webPartNameMostPopDept;
                mappedDiscUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularDiscussions"
            }


            if (_userHavePermissionDiscussions == 'True') {
                $('#post-forum-disc').css("display", "block");;
            }
            else {
                $('#post-forum-disc').css("display", "none");;
            }

            $.support.cors = true;
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: mappedDiscUrl,
                data: '{ propListName: \'' + discussionListName + '\',propItemCount:\'' + ItemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\',userName:\'' + currentLoggedInUser + '\', cacheName:\'' + cacheNamedisc + '\'}',
                success: function (msg) {
                    if (discGlobalSite == 'True') {
                        BindDiscussionGlobalData(JSON.parse(msg.d), '#MostPopularDiscussion');
                    }
                    else {
                        BindDiscussionData(JSON.parse(msg.d), '#MostPopularDiscussion');
                    }
                },
                error: function (msg) {
                    //alert('error' + msg);
                }
            });
        });

        $(window).load(function () {

            $('a[href="#Popular"]').click(function () {

                $("#view-forum-disc").css("display", "block");

                if (_userHavePermissionDiscussions == 'True') {
                    $('#post-forum-disc').css("display", "block");;
                }
                else {
                    $('#post-forum-disc').css("display", "none");;
                }

                if (discGlobalSite == 'True') {
                    $("#discTabscontrol").addClass('nav nav-tabs tb-border');
                    cacheNamedisc = currentLoggedInUser + "_" + webTitle + "_" + webPartNameMostPopGlobal;
                    mappedDiscUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularGlobalDiscussions"
                }
                else {
                    $("#discTabscontrol").addClass('nav nav-tabs dpttab');
                    cacheNamedisc = currentLoggedInUser + "_" + webTitle + "_" + webPartNameMostPopDept;
                    mappedDiscUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostPopularDiscussions"
                }

                

                $.support.cors = true;
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: mappedDiscUrl,
                    data: '{ propListName: \'' + discussionListName + '\',propItemCount:\'' + ItemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\',userName:\'' + currentLoggedInUser + '\', cacheName:\'' + cacheNamedisc + '\'}',
                    success: function (msg) {
                        if (discGlobalSite == 'True') {
                            BindDiscussionGlobalData(JSON.parse(msg.d), '#MostPopularDiscussion');
                        }
                        else {
                            BindDiscussionData(JSON.parse(msg.d), '#MostPopularDiscussion');
                        }
                        
                    },
                    error: function (msg) {
                        //alert('error' + msg);
                    }
                });
            });

            $('a[href="#Recent"]').click(function () {

                if (discGlobalSite == 'True') {
                    $("#discTabscontrol").addClass('nav nav-tabs tb-border');
                    cacheNamedisc = currentLoggedInUser + "_" + webTitle + "_" + webPartNameMostRecGlobal;
                    $("#view-forum-disc").css("display", "none");
                    $("#post-forum-disc").css("display", "none");
                    mappedDiscUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostRecentGlobalDiscussions"
                }
                else {
                    $("#discTabscontrol").addClass('nav nav-tabs dpttab');
                    cacheNamedisc = currentLoggedInUser + "_" + webTitle + "_" + webPartNameMostRecDept;
                    $("#view-forum-disc").css("display", "block");

                    if (_userHavePermissionDiscussions == 'True') {
                        $('#post-forum-disc').css("display", "block");;
                    }
                    else {
                        $('#post-forum-disc').css("display", "none");;
                    }

                    mappedDiscUrl = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMostRecentDiscussions"
                }


                $.support.cors = true;
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: mappedDiscUrl,
                    data: '{ propListName: \'' + discussionListName + '\',propItemCount:\'' + ItemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\',userName:\'' + currentLoggedInUser + '\', cacheName:\'' + cacheNamedisc + '\'}',
                    success: function (msg) {
                        if (discGlobalSite == 'True') {
                            BindGlobalRecentDiscussionData(JSON.parse(msg.d), '#MostRecentDiscussion');
                        }
                        else {
                            BindDiscussionData(JSON.parse(msg.d), '#MostRecentDiscussion');
                        }
                        
                    },
                    error: function (msg) {
                        //alert('error' + msg);
                    }
                });

            });

        });

        function BindDiscussionData(jsonData, controlID) {
            var view;
            var post;
            var html = [];
            var errorMessage = jsonData[3].Message;

            if (errorMessage.length > 0) {
                view = jsonData[1].View;
                post = jsonData[2].Post;
                $('#view-forum-disc').attr('href', view);
                $('#post-forum-disc').attr('href', post);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                view = jsonData[1].View;
                post = jsonData[2].Post;
                $('#view-forum-disc').attr('href', view);
                $('#post-forum-disc').attr('href', post);
                $(controlID).html('');
                $(controlID).append(_noDataMessage);
            }
            else {

                view = jsonData[1].View;
                post = jsonData[2].Post;
                $.each(jsonData[0].Results, function (index, element) {

                    html = html + '<div class="docinfo"><div class="forum-txt"><a href=' + element.Path + '> '
                                + '<p class="skyblue">' + element.Title + ' </p></a></div>'
                                + '<div class="userinfo-forum"><div class="usrinfo-box">'
                                + '<a href="#" class="pull-left"><span class="blue user-txt-detail">' + element.Author + '</span></a>'
                                + '<div class="icon-time-user">'
                                + '<span class="glyphicon glyphicon-time" aria-hidden="true"><span class="icon-txt">' + element.Created + '</span></span>'
                                + '<span class="glyphicon glyphicon-comment" aria-hidden="true"><span class="icon-txt"> ' + element.ReplyCount + ' Replies</span></span>'
                                + '</div></div></div></div>'
                });

                $('#view-forum-disc').attr('href', view);
                $('#post-forum-disc').attr('href', post);
                $(controlID).html(html);
            }
        }

        function BindDiscussionGlobalData(jsonData, controlID) {
            var view;
            var post;
            var html = [];
            var errorMessage = jsonData[3].Message;

            if (errorMessage.length > 0) {
                view = jsonData[1].View;
                post = jsonData[2].Post;
                $('#view-forum-disc').attr('href', view);
                $('#post-forum-disc').attr('href', post);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    view = jsonData[1].View;
                    post = jsonData[2].Post;
                    $('#view-forum-disc').attr('href', view);
                    $('#post-forum-disc').attr('href', post);
                    $(controlID).html('');
                    $(controlID).append(_noDataMessage);
                }
                else {

                    view = jsonData[1].View;
                    post = jsonData[2].Post;
                    $.each(jsonData[0].Results, function (index, element) {

                        html = html + '<div class="docinfo"><div class="forum-txt"><a href=' + element.Path + '> '
                                    + '<p class="skyblue">' + element.Title + ' </p></a></div>'
                                    + '<div class="userinfo-forum"><div class="usrinfo-box">'
                                    + '<a href="#" class="pull-left"><span class="blue user-txt-detail">' + element.Author + '</span></a>'
                                    + '<div class="icon-time-user">'
                                    + '<span class="glyphicon glyphicon-time" aria-hidden="true"><span class="icon-txt">' + element.Created + '</span></span>'
                                    + '<span class="glyphicon glyphicon-comment" aria-hidden="true"><span class="icon-txt"> ' + element.ReplyCount + ' Replies</span></span>'
                                    + '</div></div></div></div>'
                    });

                    $('#view-forum-disc').attr('href', view);
                    $('#post-forum-disc').attr('href', post);
                    $(controlID).html(html);
                }
        }

        function BindGlobalRecentDiscussionData(jsonData, controlID) {
            var view;
            var post;
            var html = [];
            var errorMessage = jsonData[3].Message;

            if (errorMessage.length > 0) {
                view = jsonData[1].View;
                post = jsonData[2].Post;
                $('#view-forum-disc').attr('href', view);
                $('#post-forum-disc').attr('href', post);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    view = jsonData[1].View;
                    post = jsonData[2].Post;
                    $('#view-forum-disc').attr('href', view);
                    $('#post-forum-disc').attr('href', post);
                    $(controlID).html('');
                    $(controlID).append(_noDataMessage);
                }
                else {

                    view = jsonData[1].View;
                    post = jsonData[2].Post;
                    $.each(jsonData[0].Results, function (index, element) {

                        html = html + '<div class="docinfo"><div class="forum-txt"><a href=' + element.Path + '> '
                                    + '<p class="skyblue">' + element.Title + ' </p></a></div>'
                                    + '<div class="userinfo-forum"><div class="usrinfo-box">'
                                    + '<a href="#" class="pull-left"><span class="blue user-txt-detail">' + element.Author + '</span></a>'
                                    + '<div class="icon-time-user">'
                                    + '<span class="glyphicon glyphicon-time" aria-hidden="true"><span class="icon-txt">' + element.Created + '</span></span>'
                                    + '<span class="glyphicon glyphicon-comment" aria-hidden="true"><span class="icon-txt"> ' + element.ReplyCount + ' Replies</span></span>'
                                    + '<span class="" aria-hidden="true"><span class="globalDisc-DeptName"> ' + element.DepartmentName + '</span></span>'
                                    + '</div></div></div></div>'
                    });

                    $('#view-forum-disc').attr('href', view);
                    $('#post-forum-disc').attr('href', post);
                    $(controlID).html(html);
                }
        }

    })(jQuery);



</script>



