using System;

namespace ObservationSites
{
	/// <summary>
	/// 
	/// </summary>
	public class MyAppSettings
	{
		public static String ConnectionString
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings ["DataAccess.ConnectionString"];
			}
		}
	}
}
