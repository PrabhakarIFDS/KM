<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FrequentlyUsedDocsWebPart.ascx.cs" Inherits="IFDS.KMPortal.Webparts.FrequentlyUsedDocsWebPart.FrequentlyUsedDocsWebPart" %>

<script type="text/javascript">
    //<![CDATA[
    var _noDataMessage = '<% =NoDataMessage %>';
    //]]>
</script>
<div id="frequentlyUsedWidget" class="widgetWrapperHeight">
    <div class="panel-heading tb-border">
        <div class="sub-caption">
            <span class="faqicon"></span>
            <span class="panel-title"><%= _FrequentlyUsedWidgetTitle%></span>

            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true"
                title='<%= _webPartToolTip%>'></a>

            <a href="javascript:RedirectFromFrequent();" class="more-globl" id="">View All</a>
        </div>
    </div>
    <div class="panel-bgcolor fu-docs outer frq">
        <div class="scrolldiv">
            <div id="frequentItems_container">
            </div>
        </div>
    </div>
</div>

<script>
    //<![CDATA[


    var SiteURL;
    var siteURLfdoc;

    siteURLfdoc = _spPageContextInfo.webAbsoluteUrl;

    if (siteURLfdoc !== undefined || siteURLfdoc !== null) {
        SiteURL = siteURLfdoc;
    }
    else {
        SiteURL = '<% =siteURL %>';
    }

    var redirectFUDUrl = SiteURL + "/_Layouts/15/IFDS.KMPortal/ViewAllRecentDocs.aspx";

    var ItemCount = '<% =itemCount %>';
    var errorMessage = '<% =_ErrMsg %>';
    var currentLoggedInUser = "";
    var userid = _spPageContextInfo.userId;
    var webTitle = _spPageContextInfo.webTitle;
    var webPartName = "FrequentlyUsedDocuments";

    function AddParameter(form, name, value) {
        var $input = $("<input />").attr("type", "hidden")
                                .attr("name", name)
                                .attr("value", value);
        form.append($input);
    }

    function RedirectFromFrequent() {
        //Create a Form        
        var $form = $("<form/>").attr("id", "data_form_frequent")
                        .attr("action", redirectFUDUrl)
                        .attr("target", "_blank")
                        .attr("method", "post");
        $("body").append($form);

        //Append the values to be send
        AddParameter($form, "SiteURL", SiteURL);
        AddParameter($form, "IsRecent", '0');
        AddParameter($form, "ErrMsg", errorMessage);
        AddParameter($form, "WebTitle", webTitle);

        //Send the Form
        $form[0].submit();
    }

    (function ($) {

        //$(document).ready(function () {
        $(window).load(function () {
            GetFrequentlyUsedData();
        });

        function GetFrequentlyUsedData() {
            var cacheName = webTitle + "_" + webPartName;
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetFrequentlyUsedDocuments",
                data: '{ siteURL: \'' + SiteURL + '\', itemCount: \'' + ItemCount + '\', cacheName:\'' + cacheName + '\'  , errMSG:\'' + errorMessage + '\' }',
                success: function (msg) {
                    BindFrequentlyUsedData(JSON.parse(msg.d), '#frequentItems_container');
                },
                error: function (msg) {
                    //alert('error :' + msg);
                }
            });
        }

        function BindFrequentlyUsedData(jsonData, controlID) {
            var html = [];
            var errorMessage = jsonData[1].Message;
            if (errorMessage.length > 0) {
                $(controlID).html(errorMessage);
            }
            else if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                $(controlID).append(_noDataMessage);
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

    })(jQuery);
    //]]>
</script>
