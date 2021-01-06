using dan.SqlObjectMapper.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace dan.SqlObjectMapper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Parameters are safe.")]
    public static class SqlConnectionExtensions
    {
        public static IEnumerable<T> Query<T>(this IDbConnection connection, string sql, object parameters)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.AddParametersToCommand(parameters);

            return ReadRows<T>(cmd);
        }

        public static T Scalar<T>(this IDbConnection connection, string sql, object parameters)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.AddParametersToCommand(parameters);

            return ReadScalar<T>(cmd);
        }

        public static int Execute(this IDbConnection connection, string sql, object parameters)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.AddParametersToCommand(parameters);

            return Execute(cmd);
        }

        public static int StoredProcedure(this IDbConnection connection, string storedProcName, object parameters)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParametersToCommand(parameters, allowArray: false);

            return Execute(cmd);
        }

        public static IEnumerable<T> StoredProcedure<T>(this IDbConnection connection, string storedProcName, object parameters)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParametersToCommand(parameters, allowArray: false);

            return ReadRows<T>(cmd);
        }

        internal static void AddParametersToCommand(this IDbCommand cmd, object parameters, bool allowArray = true)
        {
            if (parameters == null) return;

            foreach (var prop in parameters.GetType().GetProperties())
            {
                if (prop.PropertyType != typeof(string) && TypeMapping.IsEnumerableType(prop.PropertyType))
                {
                    if (allowArray)
                    {
                        var array = ((IEnumerable)prop.GetValue(parameters)).Cast<object>().ToArray();

                        cmd.CommandText = cmd.CommandText.Replace("@" + prop.Name, "(" + string.Join(", ", Enumerable.Range(0, array.Length).Select(i => "@" + prop.Name + i.ToString())) + ")");

                        for (int i = 0; i < array.Length; i++)
                            cmd.AddParameterWithValue("@" + prop.Name + i.ToString(), array[i]);
                    }
                }
                else
                {
                    var customHandler = TypeHandlerManager.Instance.FindHandler(prop.PropertyType);
                    if (customHandler != null)
                        cmd.AddParameterWithValue("@" + prop.Name, customHandler.Handler.SetParameterValue(prop.GetValue(parameters)));
                    else
                        cmd.AddParameterWithValue("@" + prop.Name, prop.GetValue(parameters));
                }
            }

            cmd.Prepare();
        }

        static IEnumerable<T> ReadRows<T>(IDbCommand cmd)
        {
            var state = cmd.Connection.State;

            OpenConnection(cmd.Connection);

            List<T> list = new List<T>();
            Type t = typeof(T);
            var mapping = TypeMappingCache.Instance.GetOrAdd(t);

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    List<SqlColumn> columns = Enumerable.Range(0, reader.FieldCount).Select(i =>
                    new SqlColumn { Name = reader.GetName(i), SqlType = reader.GetFieldType(i) }).ToList();

                    while (reader.Read())
                    {
                        T obj = IfError(Activator.CreateInstance<T>, $"Mapping type {typeof(T).Name} must have a parameterless constructor");

                        for (int i = 0; i < columns.Count; i++)
                        {
                            string col = columns[i].Name;
                            var prop = mapping.FindByName(col);
                            if (prop != null)
                            {
                                string[] layers = col.Split('.');

                                if (!reader.IsDBNull(i))
                                {
                                    object tempObj = obj;
                                    string tempName = layers[0];
                                    for (int n = 0; n < layers.Length - 1; n++)
                                    {
                                        var tempProp = mapping.FindByName(tempName);
                                        var tempType = tempProp.Property.PropertyType;
                                        var newObj = tempProp.Property.GetValue(tempObj);
                                        if (newObj == null)
                                        {
                                            newObj = IfError(() => Activator.CreateInstance(tempType), $"Mapping sub-type {tempType.Name} must have a parameterless constructor");
                                            tempProp.Property.SetValue(tempObj, newObj);
                                        }
                                        tempObj = newObj;
                                        tempName += "." + layers[n + 1];
                                    }

                                    var customHandler = TypeHandlerManager.Instance.FindHandler(prop.Property.PropertyType);
                                    if (customHandler != null)
                                    {
                                        switch (Type.GetTypeCode(customHandler.DbType))
                                        {
                                            case TypeCode.Boolean:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetBoolean(i)));
                                                break;
                                            case TypeCode.Int32:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetInt32(i)));
                                                break;
                                            case TypeCode.Int64:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetInt64(i)));
                                                break;
                                            case TypeCode.DateTime:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetDateTime(i)));
                                                break;
                                            case TypeCode.String:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetString(i)));
                                                break;
                                            case TypeCode.Byte:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetByte(i)));
                                                break;
                                            case TypeCode.Decimal:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetDecimal(i)));
                                                break;
                                            case TypeCode.Object:
                                            default:
                                                prop.Property.SetValue(tempObj, customHandler.Handler.ReadValue(reader.GetValue(i)));
                                                break;
                                        }
                                    }
                                    else
                                        switch (Type.GetTypeCode(prop.Property.PropertyType))
                                        {
                                            case TypeCode.Boolean:
                                                prop.Property.SetValue(tempObj, reader.GetBoolean(i));
                                                break;
                                            case TypeCode.Int32:
                                                prop.Property.SetValue(tempObj, reader.GetInt32(i));
                                                break;
                                            case TypeCode.Int64:
                                                prop.Property.SetValue(tempObj, reader.GetInt64(i));
                                                break;
                                            case TypeCode.DateTime:
                                                prop.Property.SetValue(tempObj, reader.GetDateTime(i));
                                                break;
                                            case TypeCode.String:
                                                prop.Property.SetValue(tempObj, reader.GetString(i));
                                                break;
                                            case TypeCode.Byte:
                                                prop.Property.SetValue(tempObj, reader.GetByte(i));
                                                break;
                                            case TypeCode.Decimal:
                                                prop.Property.SetValue(tempObj, reader.GetDecimal(i));
                                                break;
                                            case TypeCode.Object:
                                            default:
                                                prop.Property.SetValue(tempObj, reader.GetValue(i));
                                                break;
                                        }

                                }
                            }
                        }
                        list.Add(obj);
                    }
                }
            }
            finally
            {
                try
                {
                    if (state != ConnectionState.Open) // Keep connection open if the original state is open
                        cmd.Connection.Close();
                }
                catch { }
            }

            return list;
        }

        static int Execute(IDbCommand cmd)
        {
            var state = cmd.Connection.State;

            OpenConnection(cmd.Connection);

            try
            {
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                try
                {
                    if (state != ConnectionState.Open) // Keep connection open if the original state is open
                        cmd.Connection.Close();
                }
                catch { }
            }
        }

        static T ReadScalar<T>(IDbCommand cmd)
        {
            var state = cmd.Connection.State;

            OpenConnection(cmd.Connection);

            try
            {
                var obj = cmd.ExecuteScalar();
                if (obj == null || Convert.IsDBNull(obj))
                    return default;
                return (T)obj;
            }
            finally
            {
                try
                {
                    if (state != ConnectionState.Open) // Keep connection open if the original state is open
                        cmd.Connection.Close();
                }
                catch { }
            }
        }

        static void OpenConnection(IDbConnection conn)
        {
            if (conn.State == ConnectionState.Broken)
                conn.Close();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        static T IfError<T>(Func<T> func, string message)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                throw new Exception(message, e);
            }
        }

        static void AddParameterWithValue(this IDbCommand cmd, string name, object value)
        {
            IDbDataParameter param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}
