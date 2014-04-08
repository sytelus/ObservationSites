namespace GotDotNet.UI.Components.Navigation
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Text;
	using System.Configuration;
	using GotDotNet.UI.Components.Navigation;

	/// <summary>
	///	The MenuCollapsing UserControl is a uplevel and downlevel menu control driven by an Xml file.
	/// </summary>
	public class MenuCollapsing : System.Web.UI.UserControl
	{
        #region Constants
        const string menuName = "RightNavigationMenu";
        const string javaScriptVoid = "javascript:void(0)";
        #endregion
        
        #region Fields
        protected Panel Menu;
		protected HtmlTable LeftNavigation;
		protected string arrowRightUrl = ConfigurationSettings.AppSettings["ArrowRightPath"];
		protected string arrowDownUrl = ConfigurationSettings.AppSettings["ArrowDownPath"];
		protected string arrowNoneUrl = ConfigurationSettings.AppSettings["ArrowNonePath"];
        protected string newImagePath = ConfigurationSettings.AppSettings["NewImagePath"];
        protected string externalImagePath = ConfigurationSettings.AppSettings["ExternalImagePath"];
		protected string MenuClientScriptSource = ConfigurationSettings.AppSettings["MenuClientScriptSource"];
        protected System.Web.UI.WebControls.Label MainScriptLabel;
	    private DataSet dataSource;
	    #endregion
        
        #region Properties
        /// <summary>
        /// Gets or sets the DataSet value to populate the Menu.
        /// </summary>
        public DataSet DataSource
		{
			get 
			{
				return dataSource;
			}
			set
			{
				dataSource = value; 
			}		
		}
        #endregion

        #region Methods

        /// <summary>
        /// Binds the Menu to the DataSet data source specified in the DataSource property.
        /// </summary>
		public override void DataBind()
		{
			if (DataSource != null)
			{
				Menu.Controls.Clear();
				BuildMenu();
			}
			else
			{
				throw new Exception("DataSource property must be set.");
			}
		}

        /// <summary>
        /// Builds either a downlevel or uplevel Menu by loading MenuSection User Controls for each section contained in the data source.
        /// </summary>
		private void BuildMenu()
		{
			Trace.Write("loading menu...");
			bool isUpLevelBrowser = IsUpLevelBrowser(this.Context);
			this.ID = menuName;

            DataTable sections = DataSource.Tables["Section"];
			int totalSectionCount = sections.Rows.Count;

			if (totalSectionCount > 0)
			{
				DataRow sectionRow;
				for(int i = 0; i < totalSectionCount; i++)
				{
					//create instance of MenuSection for this section
					MenuSection menuSection = new MenuSection();
					menuSection = (MenuSection)LoadControl("MenuSection.ascx");
					Menu.Controls.Add(menuSection);
					menuSection.ID = "MenuSection" + i;
	
					//get data for this section
					sectionRow = sections.Rows[i];
					
					//get items for this section
                    DataView sectionItems = new DataView(DataSource.Tables["Item"],"SectionId = " + i,"SectionId, Caption", DataViewRowState.CurrentRows);


					//find controls from MenuSection control
					Label sectionHeaderLabel = (Label)menuSection.FindControl("SectionHeader");
					Label sectionArrowLabel = (Label)menuSection.FindControl("SectionArrow");
					Panel sectionPanel = (Panel)menuSection.FindControl("SectionPanel");
					Repeater SectionItemsRepeater = (Repeater)menuSection.FindControl("SectionItemsRepeater");
					//make header image and add to header label
					System.Web.UI.WebControls.Image sectionHeaderImage = new System.Web.UI.WebControls.Image();
					sectionHeaderImage.ImageUrl = sectionRow["Image"].ToString();
					sectionHeaderImage.AlternateText = sectionRow["Name"].ToString();
					sectionHeaderImage.Attributes["border"] = "0";

					//make arrow image and add to arrow label
					System.Web.UI.WebControls.Image sectionHeaderArrowImage = new System.Web.UI.WebControls.Image();
					sectionHeaderArrowImage.ID = "ArrowImage";

					//set section panel id
					sectionPanel.ID = "SectionPanel";

					//bind items to MenuSection repeater
					SectionItemsRepeater.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.SectionItemRepeater_ItemDataBound);
					SectionItemsRepeater.DataSource = sectionItems;
					SectionItemsRepeater.DataBind();

					if (isUpLevelBrowser) 
					{
						//uplevel code
						//add link to header label
						HyperLink sectionHeaderLink = new HyperLink();
						sectionHeaderLink.Attributes["href"] = javaScriptVoid;
						sectionHeaderLink.Attributes["onClick"] = "expandit('" + sectionPanel.ClientID + "')";						sectionHeaderLabel.Controls.Add(sectionHeaderLink);

						//add image to link
						sectionHeaderLink.Controls.Add(sectionHeaderImage);

						//add link to arrow label
						HyperLink sectionArrowLink = new HyperLink();
						sectionArrowLink.Attributes["href"] = javaScriptVoid;
						sectionArrowLink.Attributes["onClick"] = "expandit('" + sectionPanel.ClientID + "')";
						sectionArrowLabel.Controls.Add(sectionArrowLink);

						//add arrow image to link
						sectionArrowLink.Controls.Add(sectionHeaderArrowImage);

						//determine if the user has ever been to the site during this session
                        int pageLoadedCount = 0;
                        if (Session["PageLoadedCount"] != null)
                            pageLoadedCount = Convert.ToInt16(Session["PageLoadedCount"]);

                        //on the first visit this session, open the first menu section
						if (i == 0 && pageLoadedCount == 0)
						{
							sectionPanel.Attributes["style"] = "display:inline";
							sectionHeaderArrowImage.ImageUrl = arrowDownUrl;
						}
						else
						{
							sectionPanel.Attributes["style"] = "display:none";
							sectionHeaderArrowImage.ImageUrl = arrowRightUrl;
							Session["PageLoadedCount"] = 1;
						}
        			}
					else
					{
						//downlevel code
						//add image to header
						sectionHeaderLabel.Controls.Add(sectionHeaderImage);

						//set image Url for downlevel browsers
						sectionHeaderArrowImage.ImageUrl = arrowNoneUrl;
						sectionArrowLabel.Controls.Add(sectionHeaderArrowImage);
						MainScriptLabel.Visible = false;
					}
				}
                //add JavaScript setup script
                HtmlGenericControl script = new HtmlGenericControl("script");
                script.Attributes["language"] = "JavaScript";
                script.ID = "SetupScript";
                StringBuilder scriptText = new StringBuilder();
                scriptText.Append("\n<!--\n");
                scriptText.Append("\tvar sectionCount = ");
                scriptText.Append(totalSectionCount);
                scriptText.Append(";\n");
                scriptText.Append("\tvar menuClientId = \"");
                scriptText.Append(Menu.ClientID);
                scriptText.Append("\";\n");
                scriptText.Append("\tvar menuId = \"");
                scriptText.Append(Menu.ID);
                scriptText.Append("\";\n");
                scriptText.Append("\tvar arrowDown = new Image();\n");
                scriptText.Append("\tarrowDown.src = \"");
                scriptText.Append(arrowDownUrl);
                scriptText.Append("\";\n");
                scriptText.Append("\tvar arrowRight = new Image();\n");
                scriptText.Append("\tarrowRight.src = \"");
                scriptText.Append(arrowRightUrl);
                scriptText.Append("\";\n");
                scriptText.Append("\n-->");
                script.InnerHtml = scriptText.ToString();
                Menu.Controls.Add(script);
			}
		}

		///<summary>
		/// Handles the binding of items to the repeater in a MenuSection control
		///</summary>	
		public void SectionItemRepeater_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
				HyperLink sectionItem = (HyperLink)e.Item.FindControl("SectionItem");
				DataRow dr = ((DataRowView)e.Item.DataItem).Row;
                sectionItem.NavigateUrl = dr["Url"].ToString();
				sectionItem.Text = dr["Caption"].ToString();

				//if Item node in XML has attribute External="true"
                //display the new image defined in newImagePath field
				if (Convert.ToBoolean(dr["New"].ToString()))
				{
					System.Web.UI.WebControls.Image newImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("NewImage");
					newImage.ImageUrl = newImagePath;
					newImage.Attributes["border"] = "0";
					newImage.Visible = true;
				}

				//if Item node in XML has attribute New="true"
                //display the external link image defined in externalImagePath field
				if (Convert.ToBoolean(dr["External"].ToString()))
				{
					System.Web.UI.WebControls.Image externalImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("ExternalImage");
					externalImage.ImageUrl = externalImagePath;
					externalImage.Attributes["border"] = "0";
					externalImage.Visible = true;
				}
			}
		}

		///<summary>
		/// Returns true if the user's browser is IE 4 or higher, Netscape 6 or higher AND javascript is enabled AND is on Windows
		///</summary>	
		public bool IsUpLevelBrowser(HttpContext context)
		{
			bool isUpLevelBrowser = false;
			HttpBrowserCapabilities  bc = context.Request.Browser;

            if (bc.MajorVersion >= 4 && bc.JavaScript && bc.Platform.IndexOf("Win") > -1)
			{
				if ((bc["browser"] == "IE" && bc.MajorVersion >= 5) || (bc["browser"] == "Netscape" && bc.MajorVersion >= 6))
					return true;
			}
			Trace.Write("isUpLevelBrowser",isUpLevelBrowser.ToString());
			return isUpLevelBrowser;
		}
        #endregion
	}
}
