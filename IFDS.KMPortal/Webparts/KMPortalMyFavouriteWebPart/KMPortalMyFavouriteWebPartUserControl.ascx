<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KMPortalMyFavouriteWebPartUserControl.ascx.cs" Inherits="IFDS.KMPortal.Webparts.KMPortalMyFavouriteWebPart.KMPortalMyFavouriteWebPartUserControl" %>

<script type="text/javascript">
    //<![CDATA[
    var _noDataMessage = '<% =NoDataMessage %>';
    //]]>
</script>

<div id="myFavouriteWidget" class="widgetWrapperHeight">
    <div class="panel-heading tb-border">
        <div class="sub-caption">
            <span class="newsicon"></span>
            <span class="panel-title"><%= smyfavWidgetTitle%></span>
            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true"
                title='<%= smyfavTooTip%>'></a>
            <a href='#' class='more-globl' id='view-forum-myfav'>View All</a>
        </div>
    </div>
    <div class="panel-bgcolor">
        <div class="globalscrollnew outer">
            <div class="scrolldiv">
                <div id="divMyfavorite">
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    jQuery.noConflict();
    (function ($) {

        var siteURL;
        var itemCount = '<% =smyfavItemCount %>';
        var errorMessage = '<% =smyfavErrMsg %>';

        var myfavSiteURL = _spPageContextInfo.siteAbsoluteUrl;

        if (myfavSiteURL !== undefined || myfavSiteURL !== null) {
            siteURL = myfavSiteURL;
        }
        else {
            siteURL = '<% =smyfavSiteUrl %>';
        }

        $(document).ready(function () {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetMyFavouritesData",
                data: '{ propItemCount:\'' + itemCount + '\' , propSiteUrl:\'' + siteURL + '\', propErrorMessage:\'' + errorMessage + '\'}',
                success: function (msg) {
                    BindMyFavData(JSON.parse(msg.d), '#divMyfavorite')
                },
                error: function (msg) {
                    // alert('error' + msg);
                }
            });

        });

        function BindMyFavData(jsonData, controlID) {
            var html = [];
            var view;
            var errorMessage = jsonData[1].Message;

            if (errorMessage.length > 0) {
                view = jsonData[2].View;
                $('#view-forum-myfav').attr('href', view);
                $(controlID).html(errorMessage);
            }
            else
                if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                    view = jsonData[2].View;
                    $('#view-forum-myfav').attr('href', view);
                    $(controlID).html('');
                    $(controlID).append(_noDataMessage);
                }
                else {
                    view = jsonData[2].View;
                    $.each(jsonData[0].Results, function (index, element) {
                        html = html + '<div class="docinfo"><div class="forum-txt"><img src=' + element.IconUrl + '></img><a class="skyblue myfavwidth" href=' + element.ContentUri + '> '
                                   + '<p class="skyblue">' + element.FileName + ' </p></a></div>'
                                   + '<div class="userinfo-forum"><div class="usrinfo-box">'
                                   + '<a href="#" class="pull-left"><span class="blue user-txt-detail">' + element.Owner + '</span></a>'
                                   + '<div class="icon-time-user">'
                                   + '<span class="glyphicon glyphicon-time" aria-hidden="true"><span class="icon-txt">' + element.ModifiedDate + '</span></span>'
                                   + '<span class="blue user-txt-detail"><span> ' + element.DepartmentName + '</span></span>'
                                   + '</div></div></div></div>'

                    });
                    $('#view-forum-myfav').attr('href', view);
                    $(controlID).html(html);
                }
        }

    })(jQuery);



</script>
