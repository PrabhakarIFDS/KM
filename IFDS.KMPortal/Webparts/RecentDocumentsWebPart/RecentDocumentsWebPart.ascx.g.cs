﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IFDS.KMPortal.Webparts.RecentDocumentsWebPart {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    using System.CodeDom.Compiler;
    
    
    public partial class RecentDocumentsWebPart {
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebPartCodeGenerator", "12.0.0.0")]
        public static implicit operator global::System.Web.UI.TemplateControl(RecentDocumentsWebPart target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "12.0.0.0")]
        private void @__BuildControlTree(global::IFDS.KMPortal.Webparts.RecentDocumentsWebPart.RecentDocumentsWebPart @__ctrl) {
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "12.0.0.0")]
        private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            @__w.Write("\r\n\r\n<script type=\"text/javascript\">\r\n    //<![CDATA[\r\n    var _noDataMessage = \'");
                   @__w.Write(NoDataMessage );

            @__w.Write("\';\r\n    //]]>\r\n</script>\r\n\r\n<div id=\"recentDocumentsWidget\" class=\"widgetWrapperH" +
                    "eight\">\r\n    <div class=\"panel-heading tb-border\">\r\n        <div class=\"sub-capt" +
                    "ion\">\r\n            <span class=\"RcentMod\"></span>\r\n            <span class=\"pane" +
                    "l-title\">");
                              @__w.Write( _BestWidgetTitle);

            @__w.Write("</span>\r\n\r\n            <a class=\"help-icon\" href=\"#\" data-placement=\"top\" data-to" +
                    "ggle=\"tooltip\" data-container=\"body\" data-html=\"true\"\r\n                title=\'");
               @__w.Write( bestToolTip);

            @__w.Write(@"'></a>

            <a href=""javascript:RedirectFromRecent();"" class=""more-globl"" id="""">View All</a>
        </div>
    </div>

    <div class=""recent-docs outer panel-bgcolor"">
        <div class=""scrolldiv "">
            <div id=""items_container"">
            </div>
        </div>
    </div>
</div>







<script>
    //<![CDATA[

    var recentDocSiteURL;
    var ItemCount = '");
              @__w.Write(itemCount );

            @__w.Write("\';\r\n    var errorMessage = \'");
                 @__w.Write(_ErrMsg);

            @__w.Write(@"';
    var userid = _spPageContextInfo.userId;
    var currentLoggedInUser = """";
    var docSiteURL = _spPageContextInfo.webAbsoluteUrl;
    var webTitle = _spPageContextInfo.webTitle;
    var webPartName = ""RecentDocuments"";

    if (docSiteURL !== undefined || docSiteURL !== null) {
        recentDocSiteURL = docSiteURL;
    }
    else {
        recentDocSiteURL = '");
                     @__w.Write(siteURL );

            @__w.Write("\';\r\n    }\r\n\r\n\r\n    var redirectUrl = recentDocSiteURL + \"/_Layouts/15/IFDS.KMPort" +
                    "al/ViewAllRecentDocs.aspx\";\r\n\r\n    //Add Parameter to the form.\r\n    function Ad" +
                    "dParameter(form, name, value) {\r\n        var $input = $(\"<input />\").attr(\"type\"" +
                    ", \"hidden\")\r\n                                .attr(\"name\", name)\r\n              " +
                    "                  .attr(\"value\", value);\r\n        form.append($input);\r\n    }\r\n\r" +
                    "\n    //Redirect to View All Page\r\n    function RedirectFromRecent() {\r\n        /" +
                    "/Create a Form        \r\n        var $form = $(\"<form/>\").attr(\"id\", \"data_form_R" +
                    "ecent\")\r\n                        .attr(\"action\", redirectUrl)\r\n                 " +
                    "       .attr(\"target\", \"_blank\")\r\n                        .attr(\"method\", \"post\"" +
                    ");\r\n        $(\"body\").append($form);\r\n\r\n        //Append the values to be send\r\n" +
                    "        AddParameter($form, \"SiteURL\", recentDocSiteURL);\r\n        AddParameter(" +
                    "$form, \"IsRecent\", \'1\');\r\n        AddParameter($form, \"CurrentUserID\", userid);\r" +
                    "\n        AddParameter($form, \"ErrMsg\", errorMessage);\r\n        AddParameter($for" +
                    "m, \"WebTitle\", webTitle);\r\n\r\n        //Send the Form\r\n        $form[0].submit();" +
                    "\r\n    }\r\n\r\n\r\n\r\n    (function ($) {\r\n\r\n        $(document).ready(function () {\r\n " +
                    "           //$.when($.ajax(GetCurrentUser())).then(function () {\r\n\r\n            " +
                    "//Get the recently modified documents from all lists.\r\n            GetRecentlyMo" +
                    "difiedData();\r\n            //});\r\n        });\r\n\r\n        function BindData(jsonD" +
                    "ata, controlID) {\r\n            var html = [];\r\n            var errorMessage = js" +
                    "onData[1].Message;\r\n\r\n            if (errorMessage.length > 0) {\r\n              " +
                    "  $(controlID).html(errorMessage);\r\n            }\r\n            else if (jsonData" +
                    "[0].Results === undefined || jsonData[0].Results === null || jsonData[0].Results" +
                    ".length == 0) {\r\n                $(controlID).html(_noDataMessage);\r\n           " +
                    " }\r\n            else {\r\n                $.each(jsonData[0].Results, function (in" +
                    "dex, element) {\r\n\r\n                    html = html + \'<div class=\"docinfo\"><div " +
                    "class=\"img-txt\"><div><img src=\' + element.iconURL + \' alt=\"icon\"> \'\r\n           " +
                    "                     + \'<a class=\"skyblue\" href=\\\'\' + element.itemURL + \'\\\' targ" +
                    "et=\"_blank\">\' + element.Title + \'</a>\'\r\n                                + \'</div" +
                    "></div><div class=\"userinfo-txt\"><div class=\"usrinfo-box\">\'\r\n                   " +
                    "             + \'<a href=\"#\" class=\"pull-left\"><span class=\"blue user-txt-detail\"" +
                    ">\' + element.Author + \'</span></a>\'\r\n                                + \'<div cla" +
                    "ss=\"icon-time-user\"><span class=\"glyphicon glyphicon-time bdn\" aria-hidden=\"true" +
                    "\">\'\r\n                                + \'<span class=\"icon-txt\">\' + element.modif" +
                    "iedDate + \'</span></span>\'\r\n                                + \'<a href=\"#\" onCli" +
                    "ck=\"TriggerOutlook(\\\'\' + element.itemAbsoluteURL + \'\\\');\" class=\"mail-icon\"></a>" +
                    "\'\r\n                                + \'</div></div></div></div>\';\r\n              " +
                    "  });\r\n\r\n                $(controlID).html(html);\r\n            }\r\n        }\r\n\r\n\r" +
                    "\n        function GetRecentlyModifiedData() {\r\n            var cacheName = curre" +
                    "ntLoggedInUser + \"_\" + webTitle + \"_\" + webPartName;\r\n\r\n            $.ajax({\r\n  " +
                    "              type: \"POST\",\r\n                contentType: \"application/json; cha" +
                    "rset=utf-8\",\r\n                url: \"/_Layouts/15/IFDS.KMPortal/WebMethodHelper.a" +
                    "spx/GetRecentlyModifiedDocuments\",\r\n                data: \'{ siteURL: \\\'\' + rece" +
                    "ntDocSiteURL + \'\\\', itemCount: \\\'\' + ItemCount + \'\\\' , userName:\\\'\' + currentLog" +
                    "gedInUser + \'\\\',cacheName:\\\'\' + cacheName + \'\\\' , errMSG:\\\'\' + errorMessage + \'\\" +
                    "\'}\',\r\n                success: function (msg) {\r\n                    BindData(JS" +
                    "ON.parse(msg.d), \'#items_container\');\r\n                },\r\n                error" +
                    ": function (msg) {\r\n                    //alert(\'error :\' + msg);\r\n             " +
                    "   }\r\n            });\r\n        }\r\n\r\n        //Get Current logged in user name\r\n " +
                    "       function GetCurrentUser() {\r\n            var requestUri = _spPageContextI" +
                    "nfo.webAbsoluteUrl + \"/_api/web/getuserbyid(\" + userid + \")\";\r\n\r\n            var" +
                    " requestHeaders = { \"accept\": \"application/json;odata=verbose\" };\r\n\r\n           " +
                    " $.ajax({\r\n                url: requestUri,\r\n                async: false,\r\n    " +
                    "            contentType: \"application/json;odata=verbose\",\r\n                head" +
                    "ers: requestHeaders,\r\n                success: function (data) {\r\n              " +
                    "      currentLoggedInUser = data.d.LoginName;\r\n                    currentLogged" +
                    "InUser = currentLoggedInUser.replace(\"\\\\\", \"\\\\\\\\\\\\\\\\\");\r\n                },\r\n   " +
                    "             error: onError\r\n            });\r\n        }\r\n\r\n        function onEr" +
                    "ror(error) {\r\n            //alert(\"Error : \" + error.errorMessage);\r\n        }\r\n" +
                    "\r\n\r\n    })(jQuery);\r\n    //]]>\r\n\r\n\r\n</script>\r\n");
        }
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "12.0.0.0")]
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "12.0.0.0")]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "12.0.0.0")]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}
