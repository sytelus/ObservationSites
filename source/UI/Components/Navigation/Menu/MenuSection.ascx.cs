namespace GotDotNet.UI.Components.Navigation
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	/// The MenuSection User Control is one section of a menu.  
	/// </summary>
	public class MenuSection : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label SectionHeader;
		protected System.Web.UI.WebControls.Panel SectionPanel;
		protected System.Web.UI.WebControls.Image ArrowImage;
		protected System.Web.UI.WebControls.LinkButton LinkButton1;
		protected System.Web.UI.WebControls.Label SectionArrow;
		protected System.Web.UI.WebControls.Repeater SectionItemsRepeater;
	}
}
