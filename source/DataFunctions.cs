using System;
using System.Data;
using System.Data.OleDb;

namespace ObservationSites
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class DataFunctions
	{
		public const String DATASET_DEFAULT_TABLE = "DefaultTable";
		public static DataSet GetDataSet(String vsSelectQuery, String[] vaParameterNames, Object[] vaParameterValues)
		{
			OleDbCommand cmdSelect = new OleDbCommand(vsSelectQuery , new OleDbConnection(MyAppSettings.ConnectionString ));
			cmdSelect.CommandType = CommandType.StoredProcedure;
			
			if (vaParameterNames != null)
			{
				for (int lParameterIndex = 0;lParameterIndex < vaParameterNames.Length;lParameterIndex++)
				{
					cmdSelect.Parameters.Add(vaParameterNames[lParameterIndex], vaParameterValues[lParameterIndex]);
				}
			}
			OleDbDataAdapter adpSelect = new OleDbDataAdapter();
			adpSelect.SelectCommand = cmdSelect;

			DataSet dsReturn = new DataSet() ;
			adpSelect.Fill(dsReturn, DATASET_DEFAULT_TABLE);
			cmdSelect.Connection.Close();

			return dsReturn;
		}
		public static bool IsDataSetEmpty(DataSet vdsDataSet)
		{
			return (vdsDataSet.Tables[DATASET_DEFAULT_TABLE].Rows.Count == 0);
		}
	}
}
