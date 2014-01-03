using ProjectFestival.writetofile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    //add references
    //system.configuration toevoegen in references
    //dan gewoon toevoegen
    public class Database
    {
        //add reference system configuration in framework
        private static ConnectionStringSettings ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["ConnectionStrings"]; }
        }

        private static DbConnection GetConnection()
        {
            DbConnection con = DbProviderFactories.GetFactory(ConnectionString.ProviderName).CreateConnection();
            con.ConnectionString = ConnectionString.ConnectionString;
            try
            {
                con.Open();
            }
            catch(SqlException e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            return con;
        }

        public static void ReleaseConnection(DbConnection con)
        {
            if (con != null)
            {
                con.Close();
                con = null;
            }
        }
        
        private static DbCommand BuildCommand(string sql, params DbParameter[] parameters)
        {
            DbCommand command = GetConnection().CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }
        
        public static DbDataReader GetData(string sql, params DbParameter[] parameters)
        {
            DbCommand command = null;
            DbDataReader reader = null;
            try
            {
                command = BuildCommand(sql, parameters);
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (reader != null)
                {
                    reader.Close();
                    if (command != null)
                    {
                        ReleaseConnection(command.Connection);
                    }
                }
                throw;
            }

        }
        
        public static int ModifyData(string sql, params DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = BuildCommand(sql, parameters);
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (command != null)
                {
                    ReleaseConnection(command.Connection);
                    return 0;
                }
                throw;
            }
        }
        
        public static DbParameter AddParameter(string name, object value)
        {
            DbParameter par = DbProviderFactories.GetFactory(ConnectionString.ProviderName).CreateParameter();
            par.ParameterName = name;
            par.Value = value;
            return par;
        }

        #region Transaction

        public static DbTransaction BeginTransaction()
        {
            DbConnection con = null;
            try
            {
                con = GetConnection();
                return con.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ReleaseConnection(con);
                throw;
            }
        }
        
        private static DbCommand BuildCommand(DbTransaction trans, string sql, params DbParameter[] parameters)
        {
            DbCommand command = trans.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }
        
        public static DbDataReader GetData(DbTransaction trans, string sql, params DbParameter[] parameters)
        {
            DbCommand command = null;
            DbDataReader reader = null;
            try
            {
                command = BuildCommand(trans, sql, parameters);
                command.Transaction = trans;
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (reader != null)
                {
                    reader.Close();
                    if (command != null)
                    {
                        ReleaseConnection(command.Connection);
                    }
                }
                throw;
            }

        }
        
        public static int ModifyData(DbTransaction trans, string sql, params DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = BuildCommand(trans, sql, parameters);
                command.Transaction = trans;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (command != null)
                {
                    ReleaseConnection(command.Connection);
                    return 0;
                }
                throw;
            }
        }

        #endregion
    }
}
