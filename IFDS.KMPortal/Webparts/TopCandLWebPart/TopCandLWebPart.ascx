<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopCandLWebPart.ascx.cs" Inherits="IFDS.KMPortal.Webparts.TopCandLWebPart.TopCandLWebPart" %>


<script type="text/javascript">
    //<![CDATA[    var _departmentmasterlistjsondata = '<% =DepartmentMasterListJSON %>';
    var _departmentmasterlistsiteurl = '<% =DepartmentMasterListSiteUrl %>';
    var _topcontributorsjsondata = '<% =TopContributorsListJSON %>';
    var _toplearnersjsondata = '<% =TopLearnersListJSON %>';
    var _topcontributorslistname = '<% =TopContributorsListName %>';
    var _topcontributorsalldeptlistname = '<% =TopContributorsAllDeptListName %>';
    var _toplearnerslistname = '<% =TopLearnersListName  %>';
    var _topcontributorsrowlimit = '<% =TopContributorsItemCount %>';
    var _toplearnersrowlimit = '<% =TopLearnersItemCount  %>';
    var _isglobalsitetopcandl = '<% =IsTopCandLGlobalSite  %>';
    var _topcontributorstabname = '<% =TopCandLTab1 %>';
    var _toplearnerstabname = '<% =TopCandLTab2 %>';
    var _topcandlactivetab = null;
    var _noDataMessage = '<% =NoDataMessage %>';
    var _departmentName = '<% =_deptname %>';
    var _errMessage = '<% =TopCandLErrMsg %>';
    //]]></script>

<div class="widgetWrapperHeight" id="TopCandL">
    <div class="panel-heading top-bar">
        <div class="sub-caption">
            <span class="topicon"></span>
            <!--   <img src="images/top-icon.png"> -->
            <span class="panel-title"><%=TopCandLWidgetTitle %></span>
            <a class="help-icon" href="#" data-placement="top" data-toggle="tooltip" data-container="body" data-html="true" title='<%=TopCandLToolTipMsg%>'></a>
            <%-- <div id="topcandlviewall">--%>
            <a href='<%=_initialviewallurl %>' target="_blank" class="more-globl link-bgcolor" id="topcandlviewalllink">View All</a>
            <%--</div>--%>
        </div>
        <div id="topcandldwblock">
            <span class="dptselect">Select Department</span>
            <div class="topdpmenu">
                <div class="dropdown">
                    <button class="btn btn-default dropdown-toggle dpt-btn" type="button" id="topcandlbutton" data-toggle="dropdown" data-hover="dropdown">
                        All Departments
                            <span class="caret white aerowdown"></span>
                    </button>
                    <ul class="dropdown-menu top-selectdown" role="menu" aria-labelledby="Top" id="topcandldropdown">
                    </ul>
                </div>
            </div>
        </div>

    </div>
    <div id="topcandltabblock" class="panel-bgcolor">

        <ul class="tb-border" id="topcandltab">
            <li class="active"><a data-toggle="tab" href="#contributors" id="tab1"><%=TopCandLTab1%></a></li>
            <li><a data-toggle="tab" href="#learners" id="tab2"><%=TopCandLTab2%></a></li>
        </ul>


        <div class="tab-content">
            <div id="contributors" class="tab-pane fade in active">
                <div class="outer top-scroll etratop">
                    <div class="scrolldiv forumdiv" id="topcontributorsappend">
                    </div>

                </div>
            </div>
            <div id="learners" class="tab-pane fade in">
                <div class="outer top-scroll etratop">
                    <div class="scrolldiv forumdiv" id="toplearnersappend">
                    </div>
                </div>



            </div>
        </div>
    </div>

</div>
<script>
    jQuery.noConflict();    (function ($) {
        $(document).ready(function () {

            ChangeTabColor();

            if (_isglobalsitetopcandl.toLowerCase() === 'false') {
                $("#topcandldwblock").hide();
                $('#TopCandL').addClass("widgetWrapperHeight");
            }

            getDepartmentsDropdown();

            getTopContributors();

            $('a[href="#contributors"]').click(function (e) {
                var tabNameTopcontrib = $(this).text();
                updateviewallurl(tabNameTopcontrib);
            });

            $('a[href="#learners"]').click(function (e) {
                if (_isglobalsitetopcandl.toLowerCase() === 'true') {
                    getTopLearnersAjax();
                } else {
                    getTopLearners();
                }
                var tabNameTopLearn = $(this).text();
                updateviewallurl(tabNameTopLearn);

            });

            $(".dropdown-menu li a").click(function () {
                $(this).parents(".dropdown").find('.btn').html($(this).text());
                $(this).parents(".dropdown").find('.btn').val($(this).data('value'));
                $("#topcandlbutton").change();
            });

            $("#topcandlbutton").change(function () {

                var topcandlactivetab = $("ul#topcandltab li.active").text();
                updateviewallurl(topcandlactivetab);

                getTopContributorsAjax();

                getTopLearnersAjax();

            });

        });

        /**
        * Populate Departments dropdown
        *
        */
        function ChangeTabColor() {
            if (_isglobalsitetopcandl == 'True') {
                $("#topcandltab").addClass('nav nav-tabs tb-border');
            }
            else {
                $("#topcandltab").addClass('nav nav-tabs dpttab');
            }
        }

        /**
        * Populate Departments dropdown
        *
        */
        function getDepartmentsDropdown() {
            var htmlDept = [];
            $.each(eval(_departmentmasterlistjsondata), function (index, element) {
                htmlDept = htmlDept + '<li role="presentation"><a role="menuitem" tabindex="-1" href="#">' + element.Department + '</a></li>';
            });

            $("#topcandldropdown").append(htmlDept);
        }


        /**
        * Retrieve Top Contrinbutors from Custom List
        *
        */
        function getTopContributors() {
            var errorMessage = eval(_toplearnersjsondata)[1].Message;
            if (errorMessage.length > 0) {
                $("#topcontributorsappend").append(errorMessage);
            }
            else
                if (eval(_topcontributorsjsondata)[0].Results.length === 0) {
                    $("#topcontributorsappend").html('');
                    $("#topcontributorsappend").append(_noDataMessage);
                } else {
                    var htmltopCon = [];
                    $.each(eval(_topcontributorsjsondata)[0].Results, function (index, element) {
                        htmltopCon = htmltopCon + '<div class="docinfo"><div class="usrbox-top"><div class="topuser-detail"><table><th></th><tbody class="topctb"><tr>'
                         + '<td class="skyblue user-txt"><a href="' + element.personalurl + '">' + element.Member + '</a></td><td class="countxt"><span class="count">' + element.NoOfDiscussions + '</span> Discussions</td>'
                         + '</tr><tr><td class="blue"><span class="msg"></span><span>' + element.emailaddress + '</span></td><td class="countxt"><span class="count">' +
                         element.NoOfReplies + '</span> Replies</td></tr><tr><td><span class="mbl"></span><span>' + element.phonenumber + '</span> </td><td class="countxt"><span class="replies"></span><span>' + element.GiftedBadgeText + '</span></td>'
                         + '</tr></tbody></table></div></div></div>';
                    });
                    $("#topcontributorsappend").append(htmltopCon);
                }
            var tabNameTopcontribLoad = $("ul#topcandltab li.active").text();
            updateviewallurl(tabNameTopcontribLoad);
        }

        /**
        * Retrieve Top Learners from External List.
        *
        * @version 1.0.0
        */
        function getTopLearners() {
            var htmlTopLearn = [];

            var errorMessage = eval(_toplearnersjsondata)[1].Message;

            if (errorMessage.length > 0) {
                $("#toplearnersappend .mCSB_container").empty().append(errorMessage);
            }
            else
                if (eval(_toplearnersjsondata)[0].Results === undefined || eval(_toplearnersjsondata)[0].Results === null || eval(_toplearnersjsondata)[0].Results == 0) {
                    $("#toplearnersappend .mCSB_container").empty().append(_noDataMessage);
                } else {
                    var htmlTopLearn = [];
                    $.each(eval(_toplearnersjsondata)[0].Results, function (index, element) {
                        htmlTopLearn = htmlTopLearn + '<div class="docinfo"><div class="usrbox-top"><div class="topuser-detail"><table><th></th><tbody class="topctb"><tr>'
                         + '<td class="skyblue user-txt"><a href="' + element.personalurl + '">' + element.displayname + '</a></td><td class="countxt"><span class="count">' + element.column1 + '</span> Discussions Views</td>'
                         + '</tr><tr><td class="blue"><span class="msg"></span><span>' + element.emailaddress + '</span></td><td class="countxt"><span class="count">' +
                         element.column2 + '</span> FAQ Views </td></tr><tr><td><span class="mbl"></span><span>' + element.phonenumber + '</span> </td><td class="countxt"></td>'
                         + '</tr></tbody></table></div></div></div>';
                    });
                    $("#toplearnersappend .mCSB_container").empty().append(htmlTopLearn);
                }
        }

        /**
        * Retrieve Top Contrinbutors from Custom List.
        *
        * @version 1.0.0
        */
        function getTopContributorsAjax() {

            var htmlTopContributors = [];

            var department = $("#topcandlbutton").text().trim();

            deptobj = getObjects(eval(_departmentmasterlistjsondata), 'Department', department);

            if (department.toLowerCase() === 'all departments') {
                this.listtoquery = _topcontributorsalldeptlistname;
            }
            else {
                this.listtoquery = _topcontributorslistname;
            }

            var topcquery = { siteurl: deptobj[0].siteurl, topcontributorslistname: this.listtoquery, rowlimit: _topcontributorsrowlimit, propErrorMessage: _errMessage };

            $.ajax({
                type: "POST",
                async: true,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(topcquery),
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetTopContributors",
                success: function (msg) {
                    var temp = JSON.parse(msg.d);
                    var errorMessage = temp[1].Message;
                    if (errorMessage.length > 0) {
                        $("#topcontributorsappend .mCSB_container").empty().append(errorMessage);
                    }
                    else
                        if (temp[0].Results === undefined || temp[0].Results === null || temp[0].Results == 0) {
                            $("#topcontributorsappend .mCSB_container").empty().append(_noDataMessage);
                        } else {
                            $.each(temp[0].Results, function (index, element) {
                                htmlTopContributors = htmlTopContributors + '<div class="docinfo"><div class="usrbox-top"><div class="topuser-detail"><table><th></th><tbody class="topctb"><tr>'
                                 + '<td class="skyblue user-txt"><a href="' + element.personalurl + '">' + element.Member + '</a></td><td class="countxt"><span class="count">' + element.NoOfDiscussions + '</span> Discussions</td>'
                                 + '</tr><tr><td class="blue"><span class="msg"></span><span>' + element.emailaddress + '</span></td><td class="countxt"><span class="count">' +
                                 element.NoOfReplies + '</span> Replies</td></tr><tr><td><span class="mbl"></span><span>' + element.phonenumber + '</span> </td><td class="countxt"><span class="replies"></span><span>' + element.GiftedBadgeText + '</span></td>'
                                 + '</tr></tbody></table></div></div></div>';
                            });
                            $("#topcontributorsappend .mCSB_container").empty().append(htmlTopContributors);
                        }
                },
                error: function (msg) {
                    //alert("error in getContributors" + msg);
                }
            });


        }

        /**
        * Retrieve Top Learners from External List.
        *
        * @version 1.0.0
        */
        function getTopLearnersAjax() {
            var htmlTopLearners = [];
            var department = $("#topcandlbutton").text().trim();
            var toplquery = { siteurl: _departmentmasterlistsiteurl, toplearnerslistname: _toplearnerslistname, rowlimit: _toplearnersrowlimit, departmentname: department, propErrorMessage: _errMessage };
            $.ajax({
                type: "POST",
                async: true,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(toplquery),
                url: "/_Layouts/15/IFDS.KMPortal/WebMethodHelper.aspx/GetTopLearners",
                success: function (msg) {
                    var temp = JSON.parse(msg.d);
                    var errorMessage = temp[1].Message;
                    if (errorMessage.length > 0) {
                        $("#toplearnersappend .mCSB_container").empty().append(errorMessage);
                    }
                    else
                        if (temp[0].Results === undefined || temp[0].Results === null || temp[0].Results == 0) {
                            $("#toplearnersappend .mCSB_container").empty().append(_noDataMessage);
                        } else {
                            $.each(temp[0].Results, function (index, element) {
                                htmlTopLearners = htmlTopLearners + '<div class="docinfo"><div class="usrbox-top"><div class="topuser-detail"><table><th></th><tbody class="topctb"><tr>'
                                 + '<td class="skyblue user-txt"><a href="' + element.personalurl + '">' + element.displayname + '</a></td><td class="countxt"><span class="count">' + element.column1 + '</span> Discussions Views</td>'
                                 + '</tr><tr><td class="blue"><span class="msg"></span><span>' + element.emailaddress + '</span></td><td class="countxt"><span class="count">' +
                                 element.column2 + '</span> FAQ Views</td></tr><tr><td><span class="mbl"></span><span>' + element.phonenumber + '</span> </td><td class="countxt"></td>'
                                 + '</tr></tbody></table></div></div></div>';

                            });
                            $("#toplearnersappend .mCSB_container").empty().append(htmlTopLearners);
                        }
                },
                error: function (msg) {
                    //alert("error in getTopLearners" + msg);
                }
            });
        }


        /**
        * update view all link of top learners and top contributors.
        *
        * @version 1.0.0
        */
        function updateviewallurl(tabName) {
            var department;
            // var _topcandlactivetab = $("ul#topcandltab li.active").text();
            var _topcandlactivetab = tabName;
            if (_isglobalsitetopcandl == 'True') {
                department = $("#topcandlbutton").text();
            }
            else {
                department = _departmentName;
            }
            var siteurltemp = getObjects(eval(_departmentmasterlistjsondata), 'Department', department.trim());
            if (_topcandlactivetab === _topcontributorstabname) {
                var siteurltoredirect = siteurltemp[0].siteurl + "Lists/Members/MembersAllItems.aspx";
                if (department.trim().toLowerCase() === "all departments") {
                    siteurltoredirect = _departmentmasterlistsiteurl + "Lists/TopContributorsGlobal/AllItems.aspx";
                }
                $('#topcandlviewalllink').attr("href", siteurltoredirect);
            } else if (_topcandlactivetab === _toplearnerstabname) {
                var siteurltctoredirect = _departmentmasterlistsiteurl + "pages/TopLearnersFilter.aspx" + "?web=" + department.trim();

                if (department.trim().toLowerCase() === "all departments") {
                    siteurltctoredirect = _departmentmasterlistsiteurl + "Lists/" + _toplearnerslistname + "/KMP_Z_TopLearnersRead%20List.aspx";
                }
                $('#topcandlviewalllink').attr("href", siteurltctoredirect);
            }
        }


        /**
        * parse json data and return objects.
        *
        * @version 1.0.0
        */
        function getObjects(obj, key, val) {
            var objects = [];
            for (var i in obj) {
                if (!obj.hasOwnProperty(i)) continue;
                if (typeof obj[i] == 'object') {
                    objects = objects.concat(getObjects(obj[i], key, val));
                } else if (i == key && obj[key] == val) {
                    objects.push(obj);
                }
            }
            return objects;
        }


    })(jQuery);
</script>
