using System;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using CoreApi.HelperCodes.Miscellaneous;
using System.ComponentModel;
using System.Text;
using CoreApi.HelperCodes.Enumerate;

namespace CoreApi.HelperCodes.AdoNet
{
    public static class DatabaseHelper
    {
        public static DataTable PullData(string query)
        {
            DataTable dataTable = new DataTable();
            string connectionString = AppSettingsHelper.GetConnectionString();
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(connectionString));

            try
            {
                cmd.Connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                    cmd.Connection.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
                cmd.Dispose();
            }

            return dataTable;
        }

        public static bool BulkInsertData(DataTable dt, string tableName, string updateTempTableQuery = "")
        {
            int rnd = RandomHelper.RealRandom(999, 100);
            string tempTableName = "#TempBulkInsertTable_" + rnd;

            if (!string.IsNullOrEmpty(updateTempTableQuery))
            {
                updateTempTableQuery.Replace("#tempTableName#", tempTableName);
            }

            StringBuilder createTempTableQuerySb = new StringBuilder("CREATE TABLE " + tempTableName + " (");
            StringBuilder insertQuerySb = new StringBuilder("INSERT INTO " + tableName + " (");
            StringBuilder selectTempTableQuerySb = new StringBuilder("SELECT ");

            bool isFirstAppendQuery = true; 
            foreach (DataColumn dc in dt.Columns)
            {
                string mappingType = GetMappingTypeName(dc.DataType);
                string columnName = dc.ColumnName;

                createTempTableQuerySb.Append((!isFirstAppendQuery ? ", " : "") + columnName + " " + mappingType);
                selectTempTableQuerySb.Append((!isFirstAppendQuery ? ", " : "") + columnName);
                insertQuerySb.Append((!isFirstAppendQuery ? ", " : "") + columnName);

                isFirstAppendQuery = false;
            }

            createTempTableQuerySb.Append(" );");

            insertQuerySb.Append(", StatusType, AddDate) ");
            insertQuerySb.Append(selectTempTableQuerySb.ToString() + ", " + (int)Enums.StatusTypeEnum.Active + ", GetDate() ");
            insertQuerySb.Append("FROM " + tempTableName + "; ");
            insertQuerySb.Append("DROP TABLE " + tempTableName + ";");

             bool isOk = CommitToDatabaseWithTempTable(dt, createTempTableQuerySb.ToString(), updateTempTableQuery, insertQuerySb.ToString(), tempTableName); 

            return isOk;
        }

        public static bool BulkUpdateData(DataTable dt, string tableName, string updateTempTableQuery = "", string updateKey = "UniqueId")
        {
            return BulkUpdateData(dt, tableName, updateKey, updateTempTableQuery, false);
        }
        public static bool BulkDeleteData(DataTable dt, string tableName, string updateTempTableQuery = "", string updateKey = "UniqueId")
        {
            return BulkUpdateData(dt, tableName, updateKey, updateTempTableQuery, true);
        }

        private static bool BulkUpdateData(DataTable dt, string tableName, string updateKey, string updateTempTableQuery = "", bool isDeleted = false)
        {
            int rnd = RandomHelper.RealRandom(999, 100);
            string tempTableName = "#TempBulkUpdateTable_" + rnd;

            StringBuilder createTempTableQuerySb = new StringBuilder("CREATE TABLE " + tempTableName + " (");
            StringBuilder updateQuerySb = new StringBuilder("UPDATE T SET "); 

            bool isFirstTempQuery = true, isFirstUpdateQuery = true; 
            foreach (DataColumn dc in dt.Columns)
            {
                string mappingType = GetMappingTypeName(dc.DataType);
                string columnName = dc.ColumnName;

                createTempTableQuerySb.Append((!isFirstTempQuery ? ", " : "") + columnName + " " + mappingType);
                isFirstTempQuery = false;

                if (!isDeleted && dc.ColumnName != "Id")
                {
                    updateQuerySb.Append((!isFirstUpdateQuery ? ", " : "") + "T." + columnName + " = " + "Temp." + columnName);
                    isFirstUpdateQuery = false;
                }
            }

            createTempTableQuerySb.Append(" )");

            if (isDeleted) 
                updateQuerySb.Append("T.StatusType = " + (int)Enums.StatusTypeEnum.Deleted + ", DeleteDate = GetDate() ");  
            else 
                updateQuerySb.Append(", T.UpdateDate = GetDate() ");   


            updateQuerySb.Append("FROM " + tableName + " T ");
            updateQuerySb.Append("INNER JOIN " + tempTableName + " Temp ON T." + updateKey + " = Temp." + updateKey + "; ");
            updateQuerySb.Append("DROP TABLE " + tempTableName + ";");

            bool isOk = CommitToDatabaseWithTempTable(dt, createTempTableQuerySb.ToString(), updateTempTableQuery, updateQuerySb.ToString(), tempTableName); 

            return isOk;
        }

        public static bool CommitToDatabaseWithTempTable(DataTable dataTable, string createTempTableQuery, string updateTempTableQuery, string insertOrUpdateQuery, string tempTableName)
        {
            bool isOk = true;
            using (SqlConnection conn = new SqlConnection(AppSettingsHelper.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = createTempTableQuery;
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                        {
                            bulkCopy.BatchSize = 1000000;
                            bulkCopy.BulkCopyTimeout = 59000;
                            bulkCopy.DestinationTableName = tempTableName;
                            bulkCopy.WriteToServer(dataTable);
                            bulkCopy.Close();
                        }

                        if (!string.IsNullOrEmpty(updateTempTableQuery))
                        {
                            // Update temp table
                            command.CommandTimeout = 300;
                            command.CommandText = updateTempTableQuery;
                            command.ExecuteNonQuery();
                        }

                        // Insert or update destination table and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = insertOrUpdateQuery;
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        // Handle exception properly
                        isOk = false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            } 
            return isOk;
        }

        public static SqlDbType GetDbType(Type theType)
        {
            SqlParameter sqlParameter = new SqlParameter();
            TypeConverter typeConverter = TypeDescriptor.GetConverter(sqlParameter.DbType);

            if (typeConverter.CanConvertFrom(theType))
            {
                // ReSharper disable once PossibleNullReferenceException
                sqlParameter.DbType = (DbType)typeConverter.ConvertFrom(theType.Name);
            }
            else
            {
                //Try brute force
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    sqlParameter.DbType = (DbType)typeConverter.ConvertFrom(theType.Name);
                }
                catch
                {
                    //Do Nothing; will return NVarChar as default
                }
            }


            return sqlParameter.SqlDbType;
        }

        public static string GetMappingTypeName(Type dataType)
        {
            SqlDbType sqlDbType = GetDbType(dataType);
            string mappingType;
            if (sqlDbType == SqlDbType.NVarChar)
            {
                mappingType = sqlDbType + "(Max) COLLATE Turkish_CI_AS";
            }
            else if (sqlDbType == SqlDbType.Decimal)
            {
                mappingType = sqlDbType + "(18,2)";
            }
            else
            {
                mappingType = sqlDbType.ToString();
            }

            return mappingType;
        }
    }
}