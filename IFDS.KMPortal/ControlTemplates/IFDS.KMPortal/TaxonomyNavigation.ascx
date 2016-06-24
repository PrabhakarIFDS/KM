<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaxonomyNavigation.ascx.cs" Inherits="IFDS.KMPortal.ControlTemplates.IFDS.KMPortal.TaxonomyNavigation" %>

<div class="tree" >
    
    <div class="backcolor">
    <h4> <% =WebpartTitle %> </h4>
    </div>
    <asp:Label ID="ErrorMessage" runat="server" Text=""></asp:Label>
    <div id="<% =HTMLTreeDivId %>" "></div>
    <script type="text/javascript">
        //<![CDATA[
        var <%=JSONOutputJSVariable %> = [<% =JSONFinaloutput %>];
        //]]>
</script>
</div>
