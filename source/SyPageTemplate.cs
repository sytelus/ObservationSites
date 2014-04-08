using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace ObservationSites
{
	/// <summary>
	/// Summary description for SyCommonPage.
	/// </summary>
	public class SyPageTemplate: Page
	{
		
		public String HtmlTemplateFileName = "page_template.htm";
		
		public String PageTitle = String.Empty;
		public String SiteTitle = SiteTemplateSettings.SiteTitle;
		public String SiteIcon = SiteTemplateSettings.SiteIcon;
		public String PageIcon = String.Empty;
		public String PageHeading = String.Empty;
		public String SiteNavigationMenu = String.Empty;

		protected GotDotNet.UI.Components.Navigation.MenuCollapsing moLeftNavigationMenu;		
		
		private void Page_Load(Object sender, EventArgs e)
		{
			//Read the page template
			StreamReader oTemplateFileStream = File.OpenText(Server.MapPath(HtmlTemplateFileName));
			String sTemplateContent = oTemplateFileStream.ReadToEnd();
			oTemplateFileStream.Close();
			
			//Replace site specific parameters
			sTemplateContent = CommonFunctions.ReplaceString(sTemplateContent, "[site_title]", SiteTemplateSettings.SiteTitle, true);
			sTemplateContent = CommonFunctions.ReplaceString(sTemplateContent, "[site_icon]", SiteTemplateSettings.SiteIcon, true);
			
			//Replace page specific parameters
			sTemplateContent = CommonFunctions.ReplaceString(sTemplateContent, "[page_icon]", PageIcon, true);
			sTemplateContent = CommonFunctions.ReplaceString(sTemplateContent, "[page_title]", PageTitle, true);
			sTemplateContent = CommonFunctions.ReplaceString(sTemplateContent, "[page_heading]", PageHeading, true);
			
			//Divide the page in to two parts
			String[] aPageParts = CommonFunctions.SplitString(sTemplateContent, "[page_content]", true);
			if (aPageParts.Length == 2)
			{
				if (Page.Controls.Count == 3)
				{
					Page.Controls.RemoveAt(2);
					Page.Controls.RemoveAt(0);
					Page.Controls.AddAt(0,new LiteralControl( aPageParts[0]));
					Page.Controls.AddAt(2,new LiteralControl( aPageParts[1]));
				}
				else
				{
					throw new Exception("Page has more then 3 controls which is unexpected");
				} ;
			}
			else {} ; // marker doesn't exist
			
			//For each control look for left navigation menu marker
			for(int iControlIndex = Page.Controls.Count-1; iControlIndex >= 0 ; iControlIndex--)
			{
				Control oControlInPage = Page.Controls[iControlIndex];
				if (oControlInPage is LiteralControl)
				{
					LiteralControl oLiteralControlInPage  = (LiteralControl) oControlInPage;
					String[] aLiteralControlParts = CommonFunctions.SplitString(oLiteralControlInPage.Text, "[site_navigation_menu]", true);
					if (aLiteralControlParts.Length != 1)
					{
						Page.Controls.Remove(oLiteralControlInPage);
						Page.Controls.AddAt(iControlIndex, new LiteralControl(aLiteralControlParts[0]));
						SySimpleMenu oMenu = new SySimpleMenu();
						Page.Controls.AddAt(iControlIndex+1, oMenu);
						Page.Controls.AddAt(iControlIndex+2, new LiteralControl(aLiteralControlParts[1]));
					}
				}
			}
		}
		
		private void Page_PreRender(Object sender, EventArgs e)
		{
		
		}
		
		override protected void OnInit(EventArgs e)
		{
			this.Load += new EventHandler(Page_Load);
			this.PreRender += new EventHandler(Page_PreRender);
			base.OnInit(e);
		}
	}
	
	internal class SiteTemplateSettings
	{
		public static String SiteTitle
		{
			get
			{
				return ConfigurationSettings.AppSettings["SiteTitle"];
			}
		}
		public static String SiteIcon
		{
			get
			{
				return ConfigurationSettings.AppSettings["SiteIcon"] + String.Empty;
			}
		}
	}
}
