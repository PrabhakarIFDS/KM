<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsWebpartUserControl.ascx.cs" Inherits="IFDS.KMPortal.Webparts.NewsWebpart.NewsWebpartUserControl" %>


<script type="text/javascript">
    //<![CDATA[    
    var _noDataMessage = '<% =NoDataMessage %>';
    var _userHavePermissionNews = '<% =UserHavePermission %>';
    //]]>
</script>

<div class="panel panel-default slide-panel">
    <div class="panel-heading">
        <div class="sub-caption">
            <span class="newsicon"></span>
            <span class="panel-title"><%= sNewsWidgetTitle%></span>

            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true"
                title='<%= sNewsToolTip%>'></a>
            <a href='#' class='more-globl' id='view-forum-news'>View All</a>
            <a href='#' class='post-globl link-bgcolor' id='post-forum-news'>Create News</a>
        </div>
    </div>

    <div id="NewsContent"></div>
</div>

<script>
    jQuery.noConflict();
    (function ($) {

        var NewsListName = '<% =sNewsListName %>';
        var NewsSiteURL;
        var siteURLNews;
        var ItemCount = '<% =sNewsItemCount %>';
        var ErrorMessage = '<% =sNewsErrMsg%>';
        var DefaultImageURL = '<% =sNewsDefaultImageUrl%>';
        var userid = _spPageContextInfo.userId;
        var currentLoggedInUser = "";
        var webTitle = _spPageContextInfo.webTitle;
        var webPartName = "NewsItems";

        siteURLNews = _spPageContextInfo.siteAbsoluteUrl;

        if (siteURLNews !== undefined || siteURLNews !== null) {
            NewsSiteURL = siteURLNews;
        }
        else {
            NewsSiteURL = '<% =sNewsSiteUrl %>';
        }

        $(document).ready(function () {

            //Show-Hide Create News button based on permission
            if (_userHavePermissionNews == 'True') {
                $('#post-forum-news').css("display", "block");;
            }
            else {
                $('#post-forum-news').css("display", "none");;
            }

            //$.when($.ajax(GetCurrentUser())).then(function () {
            //Get the news data from list.
            GetNewsData();
            //});
        });


        function GetNewsData() {
            var cacheName = currentLoggedInUser + "_" + webTitle + "_" + webPartName;

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetRecentNewsItems",
                data: '{ siteURL: \'' + NewsSiteURL + '\', listName: \'' + NewsListName + '\',itemCount:\'' + ItemCount + '\' , errMSG:\'' + ErrorMessage + '\',defaultImageUrl:\'' + DefaultImageURL + '\',cacheName:\'' + cacheName + '\'}',
                success: function (msg) {
                    BindData(JSON.parse(msg.d), '#NewsContent');
                },
                error: function (msg) {
                    //alert('Error : ' + msg.ErrorMessage);
                }
            });
        }

        function BindData(jsonData, controlID) {
            var view;
            var post;
            var errorMessage = jsonData[1].Message;

            if (errorMessage.length > 0) {
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    $(controlID).html(_noDataMessage);
                }
                else {

                    view = jsonData[3].View;
                    post = jsonData[2].Upload;
                    var mostRecentHTML;
                    var oSbFirst = "";
                    var oSbMiddle = "";
                    var oSbLast = "";
                    var oSbFinal = "";

                    oSbMiddle = oSbMiddle + '<div id="carousel" class="carousel slide" data-ride="carousel"><ol class="carousel-indicators carousel-indicators-numbers">'
                    var ccount = 0, ccountnext = 0;
                    oSbLast = oSbLast + '<div class="carousel-inner" role="listbox">';

                    $.each(jsonData[0].Results, function (index, element) {
                        ccountnext = ccount + 1;
                        switch (ccount) {
                            case 0:
                                oSbMiddle = oSbMiddle + '<li data-target="#carousel" data-slide-to=' + ccount + ' class="active">' + ccountnext + '</li>"';
                                break;
                            default:
                                oSbMiddle = oSbMiddle + '<li data-target="#carousel" data-slide-to=' + ccount + ' class="">' + ccountnext + '</li>"';
                                break;
                        }
                        switch (ccount) {
                            case 0:
                                oSbLast = oSbLast + '<div class="item active">';
                                break;
                            case 1:
                                oSbLast = oSbLast + '<div class="item">';
                                break;
                            default:
                                oSbLast = oSbLast + '<div class="item">';
                                break;
                        }

                        oSbLast = oSbLast + '<a href=\'' + element.itemURL + '\'>';
                        oSbLast = oSbLast + '<img src=\'' + element.imageURL + '\' ></a>';
                        oSbLast = oSbLast + '<div class="carousel-caption"></div>';
                        oSbLast = oSbLast + '<div class="">';
                        oSbLast = oSbLast + '<a href=\'' + element.itemURL + '\'>';
                        oSbLast = oSbLast + '<h5 class="slide-txt">' + element.Title + '</h5></a>';
                        oSbLast = oSbLast + '<span class="glyphicon glyphicon-time time-slide" aria-hidden="true"></span>';
                        oSbLast = oSbLast + '<span class="icon-txt">' + element.modifiedDate + '</span>';
                        oSbLast = oSbLast + '</div></div>';
                        ccount++;
                    });

                    oSbMiddle = oSbMiddle + '</ol>';
                    oSbFirst = oSbFirst + '</div></div>'
                    oSbFinal = oSbFinal + oSbMiddle;
                    oSbFinal = oSbFinal + oSbLast;
                    oSbFinal = oSbFinal + oSbFirst;
                    if (ccount > 0) {
                        //CacheHelper.SaveTocache("Global_RecentNews", oSbFinal.ToString(), DateTime.Now.AddSeconds(60.00));
                        mostRecentHTML = oSbFinal;
                    }

                    $('#view-forum-news').attr('href', view);
                    $('#post-forum-news').attr('href', post);

                    $(controlID).html(mostRecentHTML);
                }
        }


        //Get Current logged in user name
        function GetCurrentUser() {
            var requestUri = _spPageContextInfo.webAbsoluteUrl + "/_api/web/getuserbyid(" + userid + ")";

            var requestHeaders = { "accept": "application/json;odata=verbose" };

            $.ajax({
                url: requestUri,
                async: false,
                contentType: "application/json;odata=verbose",
                headers: requestHeaders,
                success: function (data) {
                    currentLoggedInUser = data.d.LoginName;
                    currentLoggedInUser = currentLoggedInUser.replace("\\", "\\\\\\\\");
                },
                error: onError
            });
        }

        function onError(error) {
            //alert("Error : " + error.errorMessage);
        }


    })(jQuery);

</script>

