<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" meta:progid="SharePoint.WebPartPage.Document" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceholderID="PlaceHolderAdditionalPageHead" runat="server">
	<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>
	<PublishingWebControls:EditModePanel runat="server">
		<!-- Styles for edit mode only-->
		<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %>"
			After="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>
	</PublishingWebControls:EditModePanel>
    <SharePointWebControls:StyleBlock runat="server">
        .s4-breadcrumb{
            display:none;
        }             
    </SharePointWebControls:StyleBlock>
</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:ListProperty Property="Title" runat="server"/> - 
	<SharePointWebControls:FieldValue FieldName="Title" runat="server"/>
</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePointWebControls:FieldValue FieldName="Title" runat="server"/>
</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderTitleBreadcrumb" runat="server"> 
    <SharePointWebControls:ListSiteMapPath runat="server" SiteMapProviders="CurrentNavigationSwitchableProvider" RenderCurrentNodeAsLink="false" PathSeparator="" CssClass="s4-breadcrumb" NodeStyle-CssClass="s4-breadcrumbNode" CurrentNodeStyle-CssClass="s4-breadcrumbCurrentNode" RootNodeStyle-CssClass="s4-breadcrumbRootNode" NodeImageOffsetX=0 NodeImageOffsetY=289 NodeImageWidth=16 NodeImageHeight=16 NodeImageUrl="/_layouts/15/images/fgimg.png?rev=23" HideInteriorRootNodes="true" SkipLinkText=""/> 

</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderPageDescription" runat="server">
	<SharePointWebControls:ProjectProperty Property="Description" runat="server"/>
</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderBodyRightMargin" runat="server">
	<div height=100% class="ms-pagemargin"><IMG SRC="/_layouts/images/blank.gif" width=10 height=1 alt=""></div>
</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderMain" runat="server">
   
	<div class="welcome blank-wp">
		
		
		<div class="ms-table ms-fullWidth">
			<div class="tableCol-75">
				
                <!--Header Section -->
                <div class="cell-margin">
					<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Header%>" ID="Header"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>

                <!-- Top Section -->
				<div class="ms-table ms-fullWidth">
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_TopLeft%>" ID="TopLeftRow"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
                    <div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="Top Middle" ID="TopMiddleRow"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_TopRight%>" ID="TopRightRow"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
				</div>

                <!-- Centre Section -->
                 <div class="cell-margin">
					<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Center%>" ID="Center"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>
				
				<div class="ms-table ms-fullWidth">
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_CenterLeft%>" ID="CenterLeftColumn"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>				
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_CenterRight%>" ID="CenterRightColumn"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
				</div>	
                
                <!-- Bottom Section -->
                <div class="ms-table ms-fullWidth">
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="Bottom Left" ID="BottomLeftRow"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
                    <div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="Bottom Middle" ID="BottomMiddleRow"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="Bottom Right" ID="BottomRightRow"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
				</div>
                <div class="cell-margin">
					<WebPartPages:WebPartZone runat="server" Title="Bottom" ID="Bottom"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>

                			
			</div>	
            <div class="tableCol-25">
                <WebPartPages:WebPartZone runat="server" Title="Right Column" ID="RightColumn"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
            </div>                        		
			<SharePointWebControls:ScriptBlock runat="server">if(typeof(MSOLayout_MakeInvisibleIfEmpty) == "function") {MSOLayout_MakeInvisibleIfEmpty();}</SharePointWebControls:ScriptBlock>
		</div>


        <PublishingWebControls:EditModePanel runat="server" CssClass="edit-mode-panel title-edit">
			<SharePointWebControls:TextField runat="server" FieldName="Title"/>
		</PublishingWebControls:EditModePanel>
        <div class="welcome-content">
			<PublishingWebControls:RichHtmlField FieldName="PublishingPageContent" HasInitialFocus="True" MinimumEditHeight="400px" runat="server"/>
		</div>
	</div>
</asp:Content>
