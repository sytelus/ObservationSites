using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.Caching;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace ObservationSites
{
	/// <summary>
	/// Summary description for SySimpleMenu.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:SySimpleMenu runat=server></{0}:SySimpleMenu>")]
	public class SySimpleMenu : System.Web.UI.WebControls.WebControl
	{
		private string m_sText;
		private string m_sMenuXMLFileName = "SySimpleMenu.xml";
		private bool m_bIsMenuBuilt = false;
		private string m_sCurrentActiveMenu = String.Empty;
		private string m_sCurrentActiveSubMenu = String.Empty;

		[Bindable(true), 
		Category("Custom"), 
		DefaultValue("SiteMenu.xml")] 
		public string MenuXMLFileName
		{
			get
			{
				return m_sMenuXMLFileName;
			}

			set
			{
				m_sMenuXMLFileName = value;
				m_bIsMenuBuilt = false;
				
				//Remove old cache if it exist
				String sCacheKey = GetCacheKey();
				DataSet oOldNavigationDataSet =(DataSet) this.Context.Cache.Get(sCacheKey);
				if (oOldNavigationDataSet != null)
				{
					this.Context.Cache.Remove(sCacheKey);
				}
			}
		}

		private String GetCacheKey()
		{
			return "SySimpleMenu_" + m_sMenuXMLFileName;
		}

		public String BuildMenu()
		{
			//Get the XML from cache
			String sCacheKey = GetCacheKey();
			
			Hashtable oMenuInfo = (Hashtable) this.Context.Cache.Get(sCacheKey);
			DataSet oNavigationDataSet;
			String sNavigationContainerHtml;
			String sParentMenuNormalHtml;
			String sParentMenuActiveHtml;
			String sSubMenuNormalHtml;
			String sSubMenuActiveHtml;
			String sSubMenuIndicatorNormalHtml;
			String sSubMenuIndicatorActiveHtml;

			if (oMenuInfo == null)
			{
				String sXmlFullFileName = Context.Server.MapPath(m_sMenuXMLFileName);
				
				XmlDocument oNavigationXmlDocument = new  XmlDocument();
				oNavigationXmlDocument.Load(sXmlFullFileName);
				sNavigationContainerHtml = ExtractNavigationHtml(oNavigationXmlDocument, "menuContainerHtml", String.Empty);
				sParentMenuNormalHtml = ExtractNavigationHtml(oNavigationXmlDocument, "parentNormalMenuItemHtml", String.Empty);
				sParentMenuActiveHtml = ExtractNavigationHtml(oNavigationXmlDocument, "parentActiveMenuItemHtml", String.Empty);
				sSubMenuNormalHtml = ExtractNavigationHtml(oNavigationXmlDocument, "subNormalMenuItemHtml", String.Empty);
				sSubMenuActiveHtml = ExtractNavigationHtml(oNavigationXmlDocument, "subActiveMenuItemHtml", String.Empty);
				sSubMenuIndicatorNormalHtml = ExtractNavigationHtml(oNavigationXmlDocument, "subMenuNormalIndicator", String.Empty);
				sSubMenuIndicatorActiveHtml = ExtractNavigationHtml(oNavigationXmlDocument, "subMenuActiveIndicator", String.Empty);

				//Read menu data XML
				oNavigationDataSet = new DataSet();
				oNavigationDataSet.ReadXml(new StringReader(oNavigationXmlDocument.GetElementsByTagName("menuStructure")[0].OuterXml));
								
				oMenuInfo = new Hashtable(8);
				oMenuInfo.Add("NavigationDataSet", oNavigationDataSet);
				oMenuInfo.Add("NavigationContainerHtml", sNavigationContainerHtml);
				oMenuInfo.Add("ParentMenuNormalHtml", sParentMenuNormalHtml);
				oMenuInfo.Add("ParentMenuActiveHtml", sParentMenuActiveHtml);
				oMenuInfo.Add("SubMenuNormalHtml", sSubMenuNormalHtml);
				oMenuInfo.Add("SubMenuActiveHtml", sSubMenuActiveHtml);
				oMenuInfo.Add("SubMenuIndicatorNormalHtml", sSubMenuIndicatorNormalHtml);
				oMenuInfo.Add("SubMenuIndicatorActiveHtml", sSubMenuIndicatorActiveHtml);
				
				oNavigationXmlDocument = null;
				
				//Put it in cache
				this.Context.Cache.Insert(sCacheKey, oMenuInfo, new CacheDependency(sXmlFullFileName));
			}
			else
			{
				oNavigationDataSet = (DataSet) oMenuInfo["NavigationDataSet"];
				sNavigationContainerHtml = (String) oMenuInfo["NavigationContainerHtml"];
				sParentMenuNormalHtml =  (String) oMenuInfo["ParentMenuNormalHtml"];
				sParentMenuActiveHtml =  (String) oMenuInfo["ParentMenuActiveHtml"];
				sSubMenuNormalHtml =  (String) oMenuInfo["SubMenuNormalHtml"];
				sSubMenuActiveHtml =  (String) oMenuInfo["SubMenuActiveHtml"];
				sSubMenuIndicatorNormalHtml =  (String) oMenuInfo["SubMenuIndicatorNormalHtml"];
				sSubMenuIndicatorActiveHtml =  (String) oMenuInfo["SubMenuIndicatorActiveHtml"];
			}
			
			//Get all parent menus
			DataTable oAllMenuTable = oNavigationDataSet.Tables["menu"];
			
			string sFilterForParentMenus;
			bool bIsAnySubMenuExist ;
			if (oAllMenuTable.Columns.Contains("menu_id_0")==true)
			{
				bIsAnySubMenuExist = true;
				sFilterForParentMenus = "menu_id_0 Is null";
			}
			else
			{
				bIsAnySubMenuExist = false;
				sFilterForParentMenus = String.Empty;
			};
			DataView oParentMenuView = new DataView(oAllMenuTable, sFilterForParentMenus, "sortOrder", DataViewRowState.CurrentRows);
			
			String sMenuInnerHtml = String.Empty;
			bool bIsActiveMenuAlreadyFound = false;
			
			//Paint parent menus
			foreach(DataRowView oParentMenuRow in oParentMenuView)
			{
				String sThisParentMenuHtml;
				bool bIsThisParentMenuActive = false;
				sThisParentMenuHtml = BuildBasicHtmlForMenuItem(oParentMenuRow,sParentMenuNormalHtml, 
					(bIsActiveMenuAlreadyFound==true)?sParentMenuNormalHtml:sParentMenuActiveHtml,true, ref bIsThisParentMenuActive);
				if (bIsThisParentMenuActive==true) bIsActiveMenuAlreadyFound = true;
				
				bool bIsSubMenuExistForThisMenu;
				DataView oSubMenuView;
				if (bIsAnySubMenuExist==true)
				{
					int iMenuId = (int) oParentMenuRow["menu_id"];
					oSubMenuView = new DataView(oAllMenuTable, "menu_id_0 = " + iMenuId.ToString(), "sortOrder", DataViewRowState.CurrentRows);
					bIsSubMenuExistForThisMenu = (oSubMenuView.Count == 0);
				}
				else
				{
					bIsSubMenuExistForThisMenu = false;
					oSubMenuView = null;
				}
				
				if  (bIsSubMenuExistForThisMenu == false)
				{
					sThisParentMenuHtml = CommonFunctions.ReplaceString(sThisParentMenuHtml, "[sub_menu_active_indicator]", String.Empty, false);
					sThisParentMenuHtml = CommonFunctions.ReplaceString(sThisParentMenuHtml, "[sub_menu_normal_indicator]", String.Empty, false);
					sThisParentMenuHtml = CommonFunctions.ReplaceString(sThisParentMenuHtml, "[sub_menu_html]", String.Empty, false);					
				}
				else
				{
					sThisParentMenuHtml = CommonFunctions.ReplaceString(sThisParentMenuHtml, "[sub_menu_active_indicator]", sSubMenuIndicatorNormalHtml, false);
					sThisParentMenuHtml = CommonFunctions.ReplaceString(sThisParentMenuHtml, "[sub_menu_normal_indicator]", sSubMenuIndicatorActiveHtml, false);
					if (bIsThisParentMenuActive==true)
					{
						string sSubMenuInnerHtml = String.Empty;
						//Build sub menu HTML
						foreach(DataRowView oSubMenuRow in oSubMenuView)
						{
							bool bIsThisSubMenuActive = false;
							sSubMenuInnerHtml += BuildBasicHtmlForMenuItem(oSubMenuRow,sSubMenuNormalHtml,sSubMenuActiveHtml,false,ref bIsThisSubMenuActive);	
						}
						sThisParentMenuHtml = CommonFunctions.ReplaceString(sThisParentMenuHtml, "[sub_menu_html]", sSubMenuInnerHtml, false);
					}					
				}
				 
				sMenuInnerHtml += sThisParentMenuHtml;
			}
			
			String sMenuOuterHtml = sNavigationContainerHtml;
			sMenuOuterHtml = CommonFunctions.ReplaceString(sMenuOuterHtml, "[menu_html]", sMenuInnerHtml, false);

			m_bIsMenuBuilt = true;
			
			return sMenuOuterHtml;
		}

		private string BuildBasicHtmlForMenuItem(DataRowView voMenuItemDatarow, string vsNormalMenuTemplate, 
			string vsActiveMenuTemplate, bool vbIsParentMenu, ref bool rbIsThisMenuActive)
		{
			String sMenuCaption = (String) voMenuItemDatarow["caption"];
			String sMenuTitle = (String) voMenuItemDatarow["title"];
			String sMenuUrl = (String) voMenuItemDatarow["url"];
			String sMenuName = (String) voMenuItemDatarow["name"];

			bool bIsThisMenuActive = IsActiveMenu(sMenuName, sMenuUrl, vbIsParentMenu);
			string sThisMenuHtml;
			if (bIsThisMenuActive==true)
			{
				sThisMenuHtml = vsActiveMenuTemplate;
			}
			else
			{
				sThisMenuHtml = vsNormalMenuTemplate;
			}

			sThisMenuHtml = CommonFunctions.ReplaceString(sThisMenuHtml, "[menu_caption]", sMenuCaption, false);
			sThisMenuHtml = CommonFunctions.ReplaceString(sThisMenuHtml, "[menu_title]", sMenuTitle, false);
			sThisMenuHtml = CommonFunctions.ReplaceString(sThisMenuHtml, "[menu_url]", sMenuUrl, false);
			
			rbIsThisMenuActive = bIsThisMenuActive;
			return sThisMenuHtml;
		}
		
		private bool IsActiveMenu(string vsMenuName, string vsMenuUrl, bool vbIsParentMenu)
		{
			bool bIsActiveMenu = false;
			if (vbIsParentMenu == true)
			{
				if (m_sCurrentActiveMenu == String.Empty)
				{
					bIsActiveMenu = ((Page.Request.ApplicationPath + "/" +  vsMenuUrl).ToLower() == Page.Request.FilePath.ToLower());
				}
				else
				{
					bIsActiveMenu = (m_sCurrentActiveMenu.ToLower() == vsMenuName.ToLower());
				}
			}
			else
			{
				if (m_sCurrentActiveSubMenu == String.Empty)
				{
					bIsActiveMenu =  ((Page.Request.ApplicationPath + "/" +  vsMenuUrl).ToLower() == Page.Request.FilePath.ToLower());
				}
				else
				{
					bIsActiveMenu = (m_sCurrentActiveSubMenu.ToLower() == vsMenuName.ToLower());
				}			
			}
			return bIsActiveMenu;
		}
		
		private string ExtractNavigationHtml(XmlDocument voXmlDocument, string vsElementTagName, string vsDefaultHtml)
		{
			string sNavigationHtml = String.Empty;
			XmlNodeList oNavigationFormattingHtmlNodes = voXmlDocument.GetElementsByTagName(vsElementTagName);
			if (oNavigationFormattingHtmlNodes.Count==0)
			{
				sNavigationHtml = vsDefaultHtml;	
			}
			else if (oNavigationFormattingHtmlNodes.Count==1)
			{
				sNavigationHtml = oNavigationFormattingHtmlNodes[0].InnerXml;
			}
			else
			{
				throw new Exception("There are multiple nodes for navigation formatting xml nodes with tag '"+ vsElementTagName +"'. Only one such tag is allowed for menu xml.");
			}
			return sNavigationHtml;
		}
		
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("")] 
		public string Text 
		{
			get
			{
				if (m_bIsMenuBuilt==false) 
				{
					m_sText = BuildMenu();
				}
				return m_sText;
			}
		}

		[Bindable(true), 
		Category("Custom"), 
		DefaultValue("")] 
		public string CurrentActiveMenu 
		{
			get
			{
				return m_sCurrentActiveMenu;
			}
			set
			{
				m_sCurrentActiveMenu = value;
			}
		}

		[Bindable(true), 
		Category("Custom"), 
		DefaultValue("")] 
		public string CurrentActiveSubMenu 
		{
			get
			{
				return m_sCurrentActiveSubMenu ;
			}
			set
			{
				m_sCurrentActiveSubMenu = value;
			}
		}


		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			output.Write(Text);
		}
	}
}
