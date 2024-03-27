using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Interfaces
{
    public abstract class DatabaseInterface
    {
        public abstract string ConnectionString { get; }

        public bool ReadRecord<T>(string procedureName, List<SqlParameter> parameters, out T data)
        {
            data = default;
            bool success = RunStoredProcedure<T>(procedureName, StatementType.Select, parameters, out List<T> dataList, out _);
            if (success)
            {
                data = dataList[0];
            }
            return success;
        }

        public bool ReadRecords<T>(string procedureName, List<SqlParameter> parameters, out List<T> dataList)
        {
            return RunStoredProcedure<T>(procedureName, StatementType.Select, parameters, out dataList, out _);
        }

        public bool UpdateRecord<T>(string procedureName, List<SqlParameter> parameters)
        {
            return RunStoredProcedure<T>(procedureName, StatementType.Update, parameters, out _, out _);
        }

        public bool DeleteRecords(string procedureName, List<SqlParameter> parameters)
        {
            return RunStoredProcedure<object>(procedureName, StatementType.Delete, parameters, out _, out _);
        }

        public bool UpdateRecord<T>(string procedureName, T updateObject, List<string> ignorableProperties = null)
        {
            List<SqlParameter> parameterDataList = new List<SqlParameter>();
            foreach (PropertyInfo prop in updateObject.GetType().GetProperties())
            {
                if (ignorableProperties != null && ignorableProperties.Contains(prop.Name))
                {
                    continue;
                }
                object parameterValue = prop.GetValue(updateObject, null);
                parameterDataList.Add(new SqlParameter($"@{prop.Name}", parameterValue));
            }
            bool success = RunStoredProcedure<T>(procedureName, StatementType.Update, parameterDataList, out List<T> dataList, out string propertyName);
            if (success)
            {
                var updatedIdentity = dataList[0];
                foreach (var property in updatedIdentity.GetType().GetProperties())
                {
                    if (property.Name != propertyName)
                    {
                        continue;
                    }
                    // this is part of the object's identity, set it on the updated record
                    property.SetValue(updateObject, property.GetValue(updatedIdentity));
                    break;
                }
            }
            return success;
        }

        private bool RunStoredProcedure<T>(string procedureName, StatementType statementType, List<SqlParameter> parameters, out List<T> dataList, out string identity)
        {
            
            identity = string.Empty;
            dataList = new List<T>();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(procedureName))
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters.ToArray());
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            bool successfullyMappedData = DataReaderMapToList<T>(dataReader, statementType, out dataList, out identity);
                            if (!successfullyMappedData)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool DataReaderMapToList<T>(SqlDataReader dataReader, StatementType statementType, out List<T> readResults, out string identity)
        {
            identity = string.Empty;
            readResults = new List<T>();
            T obj = default(T);
            while (dataReader.Read())
            {
                obj = Activator.CreateInstance<T>();
                // identity
                if ((statementType == StatementType.Insert || statementType == StatementType.Update) && dataReader.FieldCount == 1)
                {
                    var propertyToSet = obj.GetType().GetProperties().First();
                    propertyToSet.SetValue(obj, Convert.ChangeType(dataReader.GetValue(0), propertyToSet.PropertyType));
                    identity = propertyToSet.Name;
                }
                else if (statementType == StatementType.Delete)
                {
                    var rowsDeleted = (int)dataReader.GetValue(0);
                    if (rowsDeleted > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(dataReader[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dataReader[prop.Name]);
                        }
                    }
                }
                readResults.Add(obj);
            }
            return readResults.Count > 0;
        }
    }
}
