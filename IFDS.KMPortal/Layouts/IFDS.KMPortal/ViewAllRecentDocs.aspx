<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAllRecentDocs.aspx.cs" Inherits="IFDS.KMPortal.Layouts.IFDS.KMPortal.ViewAllRecentDocs" DynamicMasterPageFile="~masterurl/default.master" %>


<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="/sites/sitecentre/Style Library/KMPortal_Branding/css/jquery-ui.css" rel="stylesheet" />
    <link href="/sites/sitecentre/Style Library/KMPortal_Branding/css/ui.jqgrid.css" rel="stylesheet" />
    <script src="/sites/sitecentre/Style Library/KMPortal_Branding/js/jquery-1.11.1.js"></script>
     <script src="/_layouts/15/sp.UserProfiles.js" ></script> 
    <script src="/_layouts/15/mQuery.js"></script>
    <script src="/_layouts/15/callout.js"></script>

    <script type="text/javascript">
        //<![CDATA[

        var myData;

        var siteURL = '<% =SiteURL %>';
        var rowLimit = 0;
        var IsRecent = '<% =IsRecent %>';
        var userid = '<% =CurrentUserID%>';
        var errMessage = '<% =ErrMessage%>';
        var webTitle = '<% =WebTitle%>';
        var currentLoggedInUser = "";

        var WebMethodURL;
        var WebMethodData;
        var GridTitle;

        jQuery.noConflict();

        (function ($) {

            $(document).ready(function () {

                if (IsRecent == 1) {
                    //$.when($.ajax(GetCurrentUser())).then(function () {
                    GetData();
                    //});
                }
                else {
                    GetData();
                }
            });

        })(jQuery);

        function GetData() {

            if (IsRecent == 1) {
                var cacheName = currentLoggedInUser + "_" + webTitle + "_" + "ViewAllRecentlyModified";

                WebMethodURL = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetRecentlyModifiedDocuments";
                WebMethodData = '{ siteURL: \'' + siteURL + '\', itemCount: \'' + rowLimit + '\', userName:\'' + currentLoggedInUser + '\',cacheName:\'' + cacheName + '\' , errMSG:\'' + errMessage + '\'}';
                GridTitle = 'Recently Modified Documents';
            }
            else {
                var cacheName = webTitle + "_" + "ViewAllFrequentlyUsed";

                WebMethodURL = "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetFrequentlyUsedDocuments";
                WebMethodData = '{ siteURL: \'' + siteURL + '\', itemCount: \'' + rowLimit + '\' , cacheName:\'' + cacheName + '\'  , errMSG:\'' + errMessage + '\'}';
                GridTitle = 'Frequently Used Documents';
            }

            $.ajax({
                type: "POST",
                async: false,
                contentType: "application/json; charset=utf-8",
                url: WebMethodURL,
                data: WebMethodData,
                success: function (data) {
                    var jsonData = JSON.parse(data.d);
                    myData = jsonData[0].Results;
                    RenderGrid();
                },
                error: function (msg) {
                    //alert('error ' + msg);
                }
            });
        }

        function RenderGrid() {

            $("#recentdoclist").jqGrid({
                colNames: ['Type', 'Title', 'Author', 'Modified Date'],
                colModel: [
                            { name: 'iconURL', index: 'iconURL', align: "center", width: 30, formatter: iconFormatter },
                            { name: 'Title', index: 'Title', align: "left", formatter: titleFormatter },
                            { name: 'Author', index: 'Author', align: "left" },
                            { name: 'Modified Date', index: 'Modified', align: "center", width: 70, formatter: dateFormatter }
                ],

                datatype: "local",
                data: myData,
                pager: jQuery('#pager'),
                rowNum: 10,
                height: '100%',
                viewrecords: true,
                caption: GridTitle,
                emptyrecords: 'No records to display',
                jsonReader: {
                    root: "rows",
                    page: "page",
                    total: "total",
                    records: "records",
                    repeatitems: false,
                    Id: "0"
                },
                autowidth: true,
                multiselect: false,
                cmTemplate: { sortable: true }
            });
        }

        function iconFormatter(cellvalue, options, rowObject) {
            var html = "<img src='" + cellvalue + "'/>";
            return html;
        }

        function titleFormatter(cellvalue, options, rowObject) {
           
            var filePreviewUrl = rowObject.itemURL;           
            var html = "<a class='skyblue' href='" + rowObject.itemURL + "' target='_blank'>" + rowObject.Title + "</a>";
            html += "   ";
            html += "<a style='float:right' class='ms-lstItmLinkAnchor ms-ellipsis-a' id='CallOutDocument' onclick='OpenItemFilePreviewCallOut(this,\"" + rowObject.Title + "\",\"" + filePreviewUrl +"\")' ";
            html += " href='#'> ";
            html += "<img class='ms-ellipsis-icon' src='/_layouts/15/images/spcommon.png?rev=23' alt='Open Menu'></a>";

            return html;
        }

        function dateFormatter(cellvalue, options, rowObject) {

            var html = "<div class='icon-time-user'><span class='glyphicon glyphicon-time bdn' aria-hidden='true'>"
                            + "<span class='icon-txt'>" + rowObject.modifiedDate + "</span></span></div>";
            return html;
        }

        function emailFormatter(cellvalue, options, rowObject) {
            var html = "<a href=\"#\" onClick=\"TriggerOutlook('" + rowObject.itemAbsoluteURL + "');\" class=\"mail-icon\"></a>";
            return html;
        }

        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }

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
            // alert("Error : " + error.errorMessage);
        }


      

        function getCallOutFilePreviewBodyContent(urlWOPIFrameSrc, pxWidth, pxHeight) {
            var callOutContenBodySection = '<div class="js-callout-bodySection">';
            callOutContenBodySection += '<div class="js-filePreview-containingElement">';
            callOutContenBodySection += '<div class="js-frame-wrapper" style="line-height: 0">';
            callOutContenBodySection += '<iframe style="width: ' + pxWidth + 'px; height: ' + pxHeight + 'px;" src="' + urlWOPIFrameSrc + '&amp;action=interactivepreview&amp;wdSmallView=1" frameborder="0"></iframe>';
            callOutContenBodySection += '</div></div></div>';
            return callOutContenBodySection;
        }

        function OpenItemFilePreviewCallOut(sender, strTitle, urlWopiFileUrl) {            
            RemoveAllItemCallouts();
            var openNewWindow = true; //set this to false to open in current window
            var urlItemUrl = urlWopiFileUrl;
            var callOutContenBodySection = getCallOutFilePreviewBodyContent(urlWopiFileUrl, 379, 252);
            var calloutActionMenu = CalloutManager.getFromLaunchPointIfExists(sender);
            if (calloutActionMenu == null) {
                calloutActionMenu = CalloutManager.createNewIfNecessary({
                    ID: 'CalloutId_' + sender.id,
                    launchPoint: sender,
                    beakOrientation: 'leftRight',
                    title: strTitle,
                    content: callOutContenBodySection,
                    contentWidth: 420
                });

                /* Open Action*/
                var customActionOpen = new CalloutActionOptions();
                customActionOpen.text = 'Open';
                customActionOpen.onClickCallback = function (event, action) {
                    if (openNewWindow) {
                        window.open(urlItemUrl);
                        RemoveItemCallout(sender);
                    } else {
                        window.location.href = urlItemUrl;
                    }
                };
                var _newCustomActionOpen = new CalloutAction(customActionOpen);
                calloutActionMenu.addAction(_newCustomActionOpen);
                /* End - Open Action*/

                /* Follow Action*/

                //var customActionFollow = new CalloutActionOptions();
                //customActionFollow.text = 'Follow';
                //customActionFollow.onClickCallback = function (event, action) {
                //    var clientContext = SP.ClientContext.get_current();
                //    var socialManager = new SP.Social.SocialFollowingManager(clientContext);
                //    var siteActorInfo = new SP.Social.SocialActorInfo();
                //    siteActorInfo.set_contentUri(urlItemUrl);
                //    siteActorInfo.set_actorType(SP.Social.SocialActorTypes.documents);
                //    // follow call
                //    socialManager.follow(siteActorInfo);
                //    // upon success, we reexecute isAlreadyFollowed to make sure it was followed - a bit lazy :)

                //    SP.SOD.executeOrDelayUntilScriptLoaded(function () {  
                //        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                //            SP.SOD.executeOrDelayUntilScriptLoaded(isAlreadyFollowed(urlItemUrl) , 'SP.UserProfiles.js');
                //            //clientContext.executeQueryAsync(Function.createDelegate(this, isAlreadyFollowed), Function.createDelegate(this, onQueryFailed));

                //        }, 'SP.js');
                //    }, 'SP.runtime.js');
                
                //};
                //var _newCustomActionFollow = new CalloutAction(customActionFollow);
                //calloutActionMenu.addAction(_newCustomActionFollow);
             

                /* End - Follow Action*/



            }
            calloutActionMenu.open();
        }


        // Check if the current page is already followed
        function isAlreadyFollowed(urlItemUrl) {
            var clientContext = SP.ClientContext.get_current();
            var socialManager = new SP.Social.SocialFollowingManager(clientContext);
            var socialActor = new SP.Social.SocialActorInfo();
            socialActor.set_contentUri(urlItemUrl);
            socialActor.set_actorType(SP.Social.SocialActorTypes.documents);

            this.result = socialManager.isFollowed(socialActor);
            clientContext.executeQueryAsync(Function.createDelegate(this, this.onCheckFollowSucceeded), Function.createDelegate(this, this.onQueryFailed));
        }

        // Toggle the star if followed or not using jQuery
        function onCheckFollowSucceeded() {
            $('#favoriteLink').toggleClass("favIconSelected", this.result.get_value());
            // set global variable
            pageIsFollowed = this.result.get_value();
        }

        // Alert error
        function onQueryFailed(sender, args) {
            alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
        }


        function RemoveAllItemCallouts() {
            CalloutManager.forEach(function (callout) {
                // remove the current callout
                CalloutManager.remove(callout);
            });
        }

        function RemoveItemCallout(sender) {
            var callout = CalloutManager.getFromLaunchPointIfExists(sender);
            if (callout != null) {
                // remove
                CalloutManager.remove(callout);
            }
        }

        function CloseItemCallout(sender) {
            var callout = CalloutManager.getFromLaunchPointIfExists(sender);
            if (callout != null) {
                // close
                callout.close();
            }
        }

        //]]>
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <table id="recentdoclist"></table>
        <div id="pager"></div>
    </div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    View All Documents
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    View All Documents
</asp:Content>


