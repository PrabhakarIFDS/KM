<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductsNavigation.ascx.cs" Inherits="IFDS.KMPortal.ControlTemplates.IFDS.KMPortal.ProductsNavigation" %>

<html>

<script type="text/javascript">
    //<![CDATA[
    var _menuData = '<% =productsMenu %>';
    var _homeURL = '<% =homeURL %>';
    //]]>
</script>



<body>
    <div>
        <asp:Label ID="ErrorMessage" runat="server" Text=""></asp:Label>
    </div>

    <div class="dropdown" >
        <a id="homeLabel" role="button" class="productMenu-item"  href="<% =homeURL %>">
            <span class="menu-item-text"><% =siteTitle %> </span>
        </a>

        <%--<ul class="root ms-core-listMenu-root static">
                <li class="static" >
                    <a class="static menu-item ms-core-listMenu-item ms-displayInline ms-navedit-linkNode" href="<% =homeURL %>">
                        <span class="additional-background ms-navedit-flyoutArrow">
                            <span class="menu-item-text"><% =siteTitle %> </span>
                        </span>
                    </a>
                </li>

        </ul>--%>


        <a id="productsLabel" role="button" data-toggle="dropdown" class="productMenu-item" data-target="#" href="#">
            <span class="additional-background ms-navedit-flyoutArrow dynamic-children">
                <span class="menu-item-text">Products</span>
                <span class="caret"></span>
            </span>
        </a>

        <% =productsMenu %>
    </div>


</body>

</html>
