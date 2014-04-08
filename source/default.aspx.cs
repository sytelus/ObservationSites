using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Configuration;

namespace ObservationSites
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public class _default : SyPageTemplate 
	{
		protected System.Web.UI.WebControls.Image Image1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			
			this.PageTitle = "Home";
			this.PageHeading = "Welcome";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
