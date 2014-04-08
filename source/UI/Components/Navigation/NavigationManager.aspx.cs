using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Web.Caching;

namespace GotDotNet.UI.Components.Navigation
{
	/// <summary>
	/// The NavigationManager class handles reading and caching of navigation data
	/// </summary>
	public class NavigationManager
	{
		protected DataSet ItemsData = new DataSet();
		static int CacheExpirationTime = Convert.ToInt16(ConfigurationSettings.AppSettings["MenuCacheExpirationMinutes"]);
		
        public NavigationManager()
		{

		}

		public DataSet GetMenuData(string xmlDataFilePath) 
		{
			string cacheKey = xmlDataFilePath;
			object sectionItemsCache = HttpContext.Current.Cache[cacheKey];
			sectionItemsCache = null;
			DataSet ds = new DataSet();
			if (sectionItemsCache == null)
			{
				HttpContext.Current.Trace.Write("Attempting to data read from " + xmlDataFilePath);
				ds.ReadXml(HttpContext.Current.Server.MapPath(xmlDataFilePath));
				HttpContext.Current.Trace.Write("MenuData.xml read successfully");
				HttpContext.Current.Cache.Insert(cacheKey, ds, new CacheDependency(System.Web.HttpContext.Current.Server.MapPath(xmlDataFilePath)));
			}
			else
			{
				ds = (DataSet)sectionItemsCache;
			}
			return ds;
		}
	}
}
