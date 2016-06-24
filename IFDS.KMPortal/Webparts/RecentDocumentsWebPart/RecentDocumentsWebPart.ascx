<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentDocumentsWebPart.ascx.cs" Inherits="IFDS.KMPortal.Webparts.RecentDocumentsWebPart.RecentDocumentsWebPart" %>

<script type="text/javascript">
    //<![CDATA[
    var _noDataMessage = '<% =NoDataMessage %>';
    //]]>
</script>

<div id="recentDocumentsWidget" class="widgetWrapperHeight">
    <div class="panel-heading tb-border">
        <div class="sub-caption">
            <span class="RcentMod"></span>
            <span class="panel-title"><%= _BestWidgetTitle%></span>

            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true"
                title='<%= bestToolTip%>'></a>

            <a href="javascript:RedirectFromRecent();" class="more-globl" id="">View All</a>
        </div>
    </div>

    <div class="recent-docs outer panel-bgcolor">
        <div class="scrolldiv ">
            <div id="items_container">
            </div>
        </div>
    </div>
</div>







<script>
    //<![CDATA[

    var recentDocSiteURL;
    var ItemCount = '<% =itemCount %>';
    var errorMessage = '<% =_ErrMsg%>';
    var userid = _spPageContextInfo.userId;
    var currentLoggedInUser = "";
    var docSiteURL = _spPageContextInfo.webAbsoluteUrl;
    var webTitle = _spPageContextInfo.webTitle;
    var webPartName = "RecentDocuments";

    if (docSiteURL !== undefined || docSiteURL !== null) {
        recentDocSiteURL = docSiteURL;
    }
    else {
        recentDocSiteURL = '<% =siteURL %>';
    }


    var redirectUrl = recentDocSiteURL + "/_Layouts/15/IFDS.KMPortal/ViewAllRecentDocs.aspx";

    //Add Parameter to the form.
    function AddParameter(form, name, value) {
        var $input = $("<input />").attr("type", "hidden")
                                .attr("name", name)
                                .attr("value", value);
        form.append($input);
    }

    //Redirect to View All Page
    function RedirectFromRecent() {
        //Create a Form        
        var $form = $("<form/>").attr("id", "data_form_Recent")
                        .attr("action", redirectUrl)
                        .attr("target", "_blank")
                        .attr("method", "post");
        $("body").append($form);

        //Append the values to be send
        AddParameter($form, "SiteURL", recentDocSiteURL);
        AddParameter($form, "IsRecent", '1');
        AddParameter($form, "CurrentUserID", userid);
        AddParameter($form, "ErrMsg", errorMessage);
        AddParameter($form, "WebTitle", webTitle);

        //Send the Form
        $form[0].submit();
    }



    (function ($) {

        $(document).ready(function () {
            //$.when($.ajax(GetCurrentUser())).then(function () {

            //Get the recently modified documents from all lists.
            GetRecentlyModifiedData();
            //});
        });

        function BindData(jsonData, controlID) {
            var html = [];
            var errorMessage = jsonData[1].Message;

            if (errorMessage.length > 0) {
                $(controlID).html(errorMessage);
            }
            else if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                $(controlID).html(_noDataMessage);
            }
            else {
                $.each(jsonData[0].Results, function (index, element) {

                    html = html + '<div class="docinfo"><div class="img-txt"><div><img src=' + element.iconURL + ' alt="icon"> '
                                + '<a class="skyblue" href=\'' + element.itemURL + '\' target="_blank">' + element.Title + '</a>'
                                + '</div></div><div class="userinfo-txt"><div class="usrinfo-box">'
                                + '<a href="#" class="pull-left"><span class="blue user-txt-detail">' + element.Author + '</span></a>'
                                + '<div class="icon-time-user"><span class="glyphicon glyphicon-time bdn" aria-hidden="true">'
                                + '<span class="icon-txt">' + element.modifiedDate + '</span></span>'
                                + '<a href="#" onClick="TriggerOutlook(\'' + element.itemAbsoluteURL + '\');" class="mail-icon"></a>'
                                + '</div></div></div></div>';
                });

                $(controlID).html(html);
            }
        }


        function GetRecentlyModifiedData() {
            var cacheName = currentLoggedInUser + "_" + webTitle + "_" + webPartName;

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetRecentlyModifiedDocuments",
                data: '{ siteURL: \'' + recentDocSiteURL + '\', itemCount: \'' + ItemCount + '\' , userName:\'' + currentLoggedInUser + '\',cacheName:\'' + cacheName + '\' , errMSG:\'' + errorMessage + '\'}',
                success: function (msg) {
                    BindData(JSON.parse(msg.d), '#items_container');
                },
                error: function (msg) {
                    //alert('error :' + msg);
                }
            });
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
    //]]>


</script>
