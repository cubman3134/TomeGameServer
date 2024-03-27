using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public abstract class DatabaseInterface
    {
        public abstract string ConnectionString { get; }

        public bool ReadRecord<T>(string procedureName, List<SqlParameter> parameters, out T data)
        {
            data = default(T);
            List<T> dataList = null;
            bool success = RunStoredProcedure<T>(procedureName, true, parameters, out dataList, out _);
            if (success)
            {
                data = dataList[0];
            }
            return success;
        }

        public bool ReadRecords<T>(string procedureName, List<SqlParameter> parameters, out List<T> dataList)
        {
            return RunStoredProcedure<T>(procedureName, true, parameters, out dataList, out _);
        }

        public bool UpdateRecord<T>(string procedureName, List<SqlParameter> parameters)
        {
            return RunStoredProcedure<T>(procedureName, false, parameters, out List<T> dataList, out _);
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
                object parameterValue = null;
                /*if (prop.PropertyType.BaseType == typeof(Enum))
                {
                    parameterValue = Convert.ChangeType(prop.GetValue(updateObject, null), Enum.GetUnderlyingType(prop.PropertyType));
                }
                else
                {*/
                    parameterValue = prop.GetValue(updateObject, null);
                //}
                parameterDataList.Add(new SqlParameter($"@{prop.Name}", parameterValue));
            }
            bool success = RunStoredProcedure<T>(procedureName, false, parameterDataList, out List<T> dataList, out string propertyName);
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

        private bool RunStoredProcedure<T>(string procedureName, bool readRecord, List<SqlParameter> parameters, out List<T> dataList, out string identity)
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
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters.ToArray());
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            dataList = DataReaderMapToList<T>(dataReader, readRecord, out identity);
                            if (!dataList.Any())
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

        private List<T> DataReaderMapToList<T>(SqlDataReader dataReader, bool readRecord, out string identity)
        {
            identity = string.Empty;
            List<T> list = new List<T>();
            T obj = default(T);
            while (dataReader.Read())
            {
                obj = Activator.CreateInstance<T>();
                // identity
                if (!readRecord && dataReader.FieldCount == 1)
                {
                    var propertyToSet = obj.GetType().GetProperties().First();
                    propertyToSet.SetValue(obj, Convert.ChangeType(dataReader.GetValue(0), propertyToSet.PropertyType));
                    identity = propertyToSet.Name;
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
                list.Add(obj);
            }
            return list;
        }

    }
}
