using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ObservationSites
{
	/// <summary>
	/// Summary description for UIHelper.
	/// </summary>
	public class WebUIHelper
	{
		public static String GetSelectedListItemValue(ListControl vlstListControl)
		{
			String sSelectedItemValue;
			if (vlstListControl.SelectedItem != null)
				sSelectedItemValue = vlstListControl.SelectedItem.Value;
			else
				sSelectedItemValue = String.Empty;
			
			return sSelectedItemValue;	
		}
		public static void SetSelectedListItemSafe(ListControl vlstListControl, String vsListItemKeyValue, bool vbIsSelected)
		{
			ListItem lsiSelectedListItem = vlstListControl.Items.FindByValue(vsListItemKeyValue);
			if (lsiSelectedListItem != null)
			{
				lsiSelectedListItem.Selected = vbIsSelected;
			}		
			//return lsiSelectedListItem;
		}  
		
		public static void FillListControlFromDataSet(ListControl vlstListControl, DataSet vdsDataSet, 
			String vsListItemTextColumnName, String vsListItemValueColumnName)
		{
			vlstListControl.DataSource = vdsDataSet;
			vlstListControl.DataMember = DataFunctions.DATASET_DEFAULT_TABLE;
			vlstListControl.DataTextField = vsListItemTextColumnName;
			vlstListControl.DataValueField  = vsListItemValueColumnName;
			vlstListControl.DataBind();
		}
		
		public static Hashtable GetSelectedListItems(ListControl  vlstListControl)
		{
			Hashtable  hstSelectedListItems = new Hashtable();
			for(int iListItemIndex = 0; iListItemIndex < vlstListControl.Items.Count; iListItemIndex++)
			{
				if (vlstListControl.Items[iListItemIndex].Selected == true)
					hstSelectedListItems.Add (vlstListControl.Items[iListItemIndex].Value, vlstListControl.Items[iListItemIndex].Text );
			}
			return hstSelectedListItems;
		}
		
		public static void SetSelectedListItemSafe(ListControl vlstListControl, IDictionary vdeItemsToSelect)
		{
			foreach(String sListItemToSelectKey in vdeItemsToSelect.Keys)
			{
				ListItem liItemToSelect = vlstListControl.Items.FindByValue(sListItemToSelectKey);
				if (liItemToSelect != null)
					liItemToSelect.Selected = true;
			}
		}

		public static void SetSelectedListItemSafe(ListControl vlstListControl, DataSet vdsItemsToBeSelectedData, String vsColumnNameContainingItemValue)
		{
			int iRowCount = vdsItemsToBeSelectedData.Tables[DataFunctions.DATASET_DEFAULT_TABLE].Rows.Count;
			for(int iDataItemIndex = 0; iDataItemIndex < iRowCount; iDataItemIndex++)
			{
				String sListItemValueToBeSelected =(String) vdsItemsToBeSelectedData.Tables[DataFunctions.DATASET_DEFAULT_TABLE].Rows[iDataItemIndex][vsColumnNameContainingItemValue];
				vlstListControl.Items.FindByValue(sListItemValueToBeSelected).Selected=true;
			}
		}		
		public static void SelectAllListItems(ListControl vlstListControl, bool vbIsItemsToBeSelected)
		{
			foreach(ListItem liListItem in vlstListControl.Items)
			{
				liListItem.Selected = vbIsItemsToBeSelected;
			}
		}
	}
}
