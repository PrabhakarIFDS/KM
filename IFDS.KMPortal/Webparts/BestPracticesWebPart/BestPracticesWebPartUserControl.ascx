<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BestPracticesWebPartUserControl.ascx.cs" Inherits="IFDS.KMPortal.Webparts.BestPracticesWebPart.BestPracticesWebPartUserControl" %>

<script type="text/javascript">
    //<![CDATA[    
    var <%= WebPartID%>_noDataMessage = '<% =NoDataMessage %>';
    var <%= WebPartID%>_userHavePermissionBest = '<% =UserHavePermissionBest %>';
    var <%= WebPartID%>_userHavePermissionOnBoarding = '<% =UserHavePermissionOnBoarding %>';
    //]]>
</script>

<div id="bestPracticeWidget" class="widgetWrapperHeight">

    <div class="panel-heading">
        <div class="sub-caption">
            <span class="bpicon"></span>
            <span class="panel-title"><%= sBestPracticeWidgetTitle%></span>
            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true"
                title='<%= sBestPracticeToolTip%>'></a>

            <a href='#' class='more-globl' style="display: none" id='<%= WebPartID%>_view-forum-best'>View All</a>
            <a href='#' class='upload-globl link-bgcolor' style="display: none" id='<%= WebPartID%>_upload-forum-best'>Upload</a>
            <a href='#' class='more-globl' style="display: none" id='<%= WebPartID%>_view-forum-onBoard'>View All</a>
            <a href='#' class='upload-globl link-bgcolor' style="display: none" id='<%= WebPartID%>_upload-forum-onBoard'>Upload</a>


        </div>
    </div>
    <div class="tb-border-dpt panel-bgcolor">
        <ul class="nav nav-tabs dpttab tb-border">
            <li class="active"><a data-toggle="tab" href="#<%= WebPartID%>_Practices"><%= sBestPracticeTab1%></a></li>

            <li><a data-toggle="tab" href="#<%= WebPartID%>_Onboard"><%= sBestPracticeTab2%></a></li>
        </ul>
        <div class="tab-content">
            <div id="<%= WebPartID%>_Practices" class="tab-pane fade in active">
                <%-- <div class="col-lg-12 forum-scroll outer">--%>
                <div class="col-lg-12 forum-scroll outer panel-bgcolor">

                    <div class="scrolldiv ">
                        <div id="<%= WebPartID%>_BestPractices">
                        </div>
                    </div>
                </div>
            </div>
            <div id="<%= WebPartID%>_Onboard" class="tab-pane fade">
                <%--<div class="col-lg-12 globalscrollnew outer panel-bgcolor">--%>
                <div class="forum-scroll outer panel-bgcolor">
                    <div class="scrolldiv ">
                        <div id="<%= WebPartID%>_Onboarding">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</div>
<%-- remaining page will be generated dynamically--%>




<script>
    jQuery.noConflict();

    (function ($) {

        var OnboardingListName = '<% =sOnboardingListName %>';
        var PracticeListName = '<% =sBestPracticeListName %>';
        var PracticeListUrl;
        var OnboardingListURL;
        var ItemCount = '<% =sBestPracticeItemCount %>';
        var errorMessage = '<% =sBestPracticeErrorMessage%>';
        var currentLoggedInUser = "";
        var userid = _spPageContextInfo.userId;
        var webTitle = _spPageContextInfo.webTitle;
        var bestPracSiteURL = _spPageContextInfo.webAbsoluteUrl;
            //.siteAbsoluteUrl;
        if (bestPracSiteURL !== undefined || bestPracSiteURL !== null) {
            PracticeListUrl = bestPracSiteURL;
            OnboardingListURL = bestPracSiteURL;
        }
        else {
            PracticeListUrl = '<% =sBestPracticeListURL %>';
            OnboardingListURL = '<% =sOnboardingListURL %>';
        }

        $(window).load(function () {
           
            //Show-hide the upload button based on user permission
            <%= WebPartID%>_ShowHideBestPracticeUploadButton();
            <%= WebPartID%>_ShowHideOnBoardingUploadButton();

            //Bind the list data
            <%= WebPartID%>_BindBestPracticeData();
            
        });

        //$(document).ready(function () {
                        
        //});

        function BindData(jsonData, controlID) {
            var errorMessage;
            var htmlData = [];
            var view;
            var upload;
            errorMessage = jsonData[1].Message;
            upload = jsonData[2].Upload;
            view = jsonData[3].View;


            if (errorMessage.length > 0) {
                $(controlID).html(errorMessage);
            }
            else if (jsonData[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results.length == 0) {
                $(controlID).html(<%= WebPartID%>_noDataMessage);
            }
            else {
                $.each(jsonData[0].Results, function (index, element) {

                    htmlData = htmlData + '<div class="docinfo"><div class="img-txt">'
                            + '<a href=\'' + element.itemURL + '\' target="_blank">'
                            + '<img src=' + element.iconURL + ' alt="icon">'
                            + '<p class="skyblue">' + element.Title + '</p></a></div>'
                            + '<div class="userinfo-txt">'
                            + '<div class="usrinfo-box">'
                            + '<a href="#" class="pull-left">'
                            + '<span class="blue user-txt-detail">' + element.Author + '</span></a>'
                            + '<div class="icon-time-user">'
                            + '<span class="glyphicon glyphicon-time bdn" aria-hidden="true"><span class="icon-txt">' + element.modifiedDate + '</span></span>'
                            + '<a href="#" onClick="TriggerOutlook(\'' + element.itemAbsoluteURL + '\');" class="mail-icon">'
                            + '</a></div></div></div></div>'

                });
                $(controlID).html(htmlData);
            }
            var isBestPractice = jsonData[4].IsBestPractice;

            if (isBestPractice == '1') {
                $('#<%= WebPartID%>_upload-forum-best').attr('href', upload);
                $('#<%= WebPartID%>_view-forum-best').attr('href', view);

                $('#<%= WebPartID%>_upload-forum-onBoard').hide();
                $('#<%= WebPartID%>_view-forum-onBoard').hide();
                $('#<%= WebPartID%>_upload-forum-best').show();
                $('#<%= WebPartID%>_view-forum-best').show();

                <%= WebPartID%>_ShowHideBestPracticeUploadButton();

            }
            else {
                $('#<%= WebPartID%>_upload-forum-onBoard').attr('href', upload);
                $('#<%= WebPartID%>_view-forum-onBoard').attr('href', view);

                $('#<%= WebPartID%>_upload-forum-onBoard').show();
                $('#<%= WebPartID%>_view-forum-onBoard').show();
                $('#<%= WebPartID%>_upload-forum-best').hide();
                $('#<%= WebPartID%>_view-forum-best').hide();

                <%= WebPartID%>_ShowHideOnBoardingUploadButton();
            }


        }

        function <%= WebPartID%>_BindBestPracticeData() {
            
            var webPartName = "<%= WebPartID%>_BestPractice";
            //"BestPracticeDocuments";
            var cacheName = currentLoggedInUser + "_" + webTitle + "_" + webPartName;
            var dataParameters = '{ siteURL: \'' + PracticeListUrl + '\', propListName: \'' + PracticeListName + '\',propDisplayCount:\'' + ItemCount + '\',userName:\'' + currentLoggedInUser + '\' , errMSG:\'' + errorMessage + '\', cacheName:\'' + cacheName + '\'}';

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetRecentBestDocuments",
                data: dataParameters,
                success: function (msg) {
                    BindData(JSON.parse(msg.d), '#<%= WebPartID%>_BestPractices');                    
                },
                error: function (msg) {
                }
            });
            }

        function BindOnBoardingData() {
            
            var webPartName = "<%= WebPartID%>_OnBoard";            
            var cacheName = currentLoggedInUser + "_" + webTitle + "_" + webPartName;
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetRecentOnboardingDocuments",
                data: '{ siteURL: \'' + OnboardingListURL + '\', propListName: \'' + OnboardingListName + '\',propDisplayCount:\'' + ItemCount + '\' , userName:\'' + currentLoggedInUser + '\', errMSG:\'' + errorMessage + '\',cacheName:\'' + cacheName + '\'}',
                success: function (msg) {
                    BindData(JSON.parse(msg.d), '#<%= WebPartID%>_Onboarding');                    
                },
                error: function (msg) {
                    //alert('error' + msg);
                }
            });
            }

        $('a[href="#<%= WebPartID%>_Practices"]').click(function () {
            <%= WebPartID%>_BindBestPracticeData();
        });

        $('a[href="#<%= WebPartID%>_Onboard"]').click(function () {
            BindOnBoardingData();
        });


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

        function <%= WebPartID%>_ShowHideBestPracticeUploadButton() {
            // Begin - Show-Hide the upload button based on user permission on List
            if (<%= WebPartID%>_userHavePermissionBest == 'True') {
                $('#<%= WebPartID%>_upload-forum-best').css("display", "block");;
            }
            else {
                $('#<%= WebPartID%>_upload-forum-best').css("display", "none");;
            }

            // End - Show-Hide the upload button based on user permission on List
        }

        function <%= WebPartID%>_ShowHideOnBoardingUploadButton() {
            // Begin - Show-Hide the upload button based on user permission on List           

            if (<%= WebPartID%>_userHavePermissionOnBoarding == 'True') {
                $('#<%= WebPartID%>_upload-forum-onBoard').css("display", "block");;
            }
            else {
                $('#<%= WebPartID%>_upload-forum-onBoard').css("display", "none");;
            }
            // End - Show-Hide the upload button based on user permission on List
        }


    })(jQuery);

</script>


