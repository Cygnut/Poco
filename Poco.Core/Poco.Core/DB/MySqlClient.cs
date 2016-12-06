using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using MySql.Data.MySqlClient;

namespace Poco.Core.DB
{
    class MySqlClient
    {
        public class MySqlClientException : Exception
        {
            public MySqlClientException(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        public string ConnectionString { get; private set; }
        
        public MySqlClient(string connectionString)
        {
            ConnectionString = connectionString;
        }

        T Execute<T>(string cmdText, Func<MySqlCommand, T> func)
        {
            using (var cxn = new MySqlConnection(ConnectionString))
            {
                cxn.Open();

                using (var cmd = new MySqlCommand(cmdText, cxn))
                {
                    return func(cmd);
                }
            }
        }
        
        public int ExecuteNonQuery(string q, CommandType cmdType = CommandType.Text)
        {
            return Execute(q, (MySqlCommand cmd) =>
                {
                    cmd.CommandType = cmdType;
                    try
                    {
                        return cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Failed to read result sets while running non query {0} with error {1}.", q, e);
                        throw;
                    }
                });
        }

        public MySqlParameterCollection DeriveParameters(string spName)
        {
            return Execute(spName, (MySqlCommand cmd) =>
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        MySqlCommandBuilder.DeriveParameters(cmd);
                        return cmd.Parameters;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Failed to read result sets while running non query {0} with error {1}.", spName, e);
                        throw;
                    }
                });
        }

        string StringifySPCall(string spName, Dictionary<string, object> parameters)
        {
            var s = new StringBuilder();
            s.AppendFormat("{0}(", spName);

            s.Append(
                string.Join(", ",
                    parameters.Select(p => string.Format("{0} {1}", p.Key, p.Value))
                    )
                );

            s.Append(")");
            return s.ToString();
        }

        public DataSet ExecuteStoredProcedure(string spName, Dictionary<string, object> parameters)
        {
            return Execute(spName, (MySqlCommand cmd) =>
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.AddWithValue(p.Key, p.Value);

                    try
                    {
                        DataSet ds = new DataSet();
                        var ad = new MySqlDataAdapter(cmd);
                        ad.Fill(ds);
                        return ds;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Failed to read result sets while running {0} with error {1}", spName, e);
                        throw;
                    }
                });
        }
    }
}
