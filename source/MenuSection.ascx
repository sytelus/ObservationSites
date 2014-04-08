<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MenuSection.ascx.cs" Inherits="GotDotNet.UI.Components.Navigation.MenuSection" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table cellSpacing="0" cellPadding="0" width="184" border="0" id="MenuSection">
	<TBODY>
		<tr>
			<td width="153"><asp:Label id="SectionHeader" runat="server" width="153" EnableViewState="False"></asp:Label></td>
			<td width="31"><asp:Label id="SectionArrow" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td background="gdn_nav_bg.gif" colspan="2"><img height="1" alt="" src="1pix.gif" width="1" border="0"><br>
				<asp:Panel id="SectionPanel" runat="server">
					<asp:Repeater id="SectionItemsRepeater" runat="server" EnableViewState="False">
						<HeaderTemplate>
							<table cellSpacing="0" cellPadding="2" border="0">
						</HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td width="2" background="1pix.gif"><img height="1" alt="" src="1pix.gif" width="2" border="0"></td>
								<td width="150" background="1pix.gif">
									<asp:HyperLink id="SectionItem" runat="server" CssClass="rollovernav" /><img src="1pix.gif" width="5" height="1" alt="" border="0"><asp:Image ID="NewImage" Runat="server" Visible="False" EnableViewState="False"></asp:Image><asp:Image ID="ExternalImage" Runat="server" Visible="False" EnableViewState="False"></asp:Image><br>
								</td>
							</tr>
						</ItemTemplate>
						<FooterTemplate>
</table>
</FooterTemplate> </asp:Repeater></asp:Panel></TD></TR>
<tr>
	<td colSpan="2" height="18"><img height="18" alt="" src="gdn_nav_bottom.gif" width="184" border="0"></td>
</tr>
</TBODY></TABLE>
