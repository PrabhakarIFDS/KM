<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KMPortalSearchBoxWebPartUserControl.ascx.cs" Inherits="IFDS.KMPortal.Webparts.KMPortalSearchBoxWebPart.KMPortalSearchBoxWebPartUserControl" %>

<div class=" search-bx">
    <div class="form-group">
        <input type="text" class="form-control search-box" placeholder="Enter keyword to search" id="txtSearch">
    </div>
    <div class="btns">
        <a href="#" class="btn btn-sm search-btn" id="search"><%= srchLabelText %><span class="glyphicon glyphicon-search white" aria-hidden="true"></span></a>
    </div>
</div>


<script type="text/javascript">

    $('#search').click(function () {        
        var appendURL = "<%= srchRedirect%>?k=" + $('#txtSearch').val();
        $('#search').attr('href', appendURL);
    });

    $('#txtSearch').keypress(function (e) {
        var keycode = (event.keyCode ? event.keyCode : event.which);        
        if (keycode == '13') {           
            $('#search')[0].click();
        }
    });

</script>