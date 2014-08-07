using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Diagnostics;

namespace Veis.Data
{
    public abstract class DataAccess : IDataAccess
    {
        private readonly DbProviderFactory _dataProviderFactory;

        public DataAccess(DbProviderFactory dataProviderFactory)
        {
            _dataProviderFactory = dataProviderFactory;
        }

        public DbCommand CreateCommand(string sql)
        {
            var cmd = _dataProviderFactory.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }

        public DbParameter CreateParameter(string name)
        {
            var param = _dataProviderFactory.CreateParameter();
            param.ParameterName = name;
            return param;
        }

        public DbParameter CreateParameter(string name, object value)
        {
            var param = CreateParameter(name);
            param.Value = value;
            return param;
        }

        public IList<T> Read<T>(DbCommand command, Func<IDataReader, T> transformRowToEntity)
        {
            using (var dbc = GetDbConnection(GetDbConfig()))
            {
                return Read(dbc, command, transformRowToEntity);
            }
        }

        private IList<T> Read<T>(DbConnection dbc, DbCommand command, Func<IDataReader, T> transformRowToEntity)
        {
            var list = new List<T>();

            command.Connection = dbc;
            dbc.Open();

            try
            {
                DebugCommand(command);

                using (var reader = command.ExecuteReader())
                {
                    int rows = 0;

                    while (reader.Read())
                    {
                        rows++;
                        var entity = transformRowToEntity(reader);
                        list.Add(entity);
                    }
                    if (rows > 0)
                    {
                        DebugMessage("{0} rows affected\n", rows);
                    }
                }
            }
            finally
            {
                dbc.Close();
            }

            return list;
        }

        public int Execute(DbCommand command)
        {
            var numRowsAffected = 0;

            using (var dbc = GetDbConnection(GetDbConfig()))
            {
                command.Connection = dbc;
                dbc.Open();

                try
                {
                    DebugCommand(command);
                    numRowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception in DataAccess.Execute():" + ex.Message);
                    throw;
                }
                finally
                {
                    dbc.Close();
                }

                DebugMessage("{0} rows affected\n", numRowsAffected);
            }

            return numRowsAffected;
        }

        protected struct DbConfig
        {
            public string Host { get; set; }
            public string Port { get; set; }
            public string Database { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
        }

        protected abstract DbConfig GetDbConfig();

        private string GetDbConnectionString(DbConfig config)
        {
            var builder = _dataProviderFactory.CreateConnectionStringBuilder();
            if (builder == null) throw new NullReferenceException("CreateConnectionStringBuilder failed!");
            builder.Add("Host", config.Host);
            builder.Add("Port", config.Port);
            builder.Add("Database", config.Database);
            builder.Add("User ID", config.User);
            builder.Add("Password", config.Pass);
            return builder.ConnectionString;
        }

        private DbConnection GetDbConnection(DbConfig config)
        {
            var connection = _dataProviderFactory.CreateConnection();
            connection.ConnectionString = GetDbConnectionString(config);
            return connection;
        }

        #region For Debugging

        /// <summary>
        /// Display formatted SQL that will execute - used for debugging display of generated SQL
        /// </summary>
        /// <param name="command"></param>
        private void DebugCommand(IDbCommand command)
        {
#if DEBUG
            var items = from x in command.Parameters.OfType<DbParameter>()
                        let value = ((x.Value == null || x.Value == DBNull.Value) ? null : x.Value)
                        let quote = ((value != null && (value.GetType() == typeof(string) || value.GetType() == typeof(char))) ? "'" : "")
                        select string.Format("{1}{0}{1}", (value ?? "null"), quote);

            string message = string.Format("SQL: {0}", command.CommandText);

            foreach (var item in items)
            {
                int index = message.IndexOf('?');

                if (index >= 0)
                {
                    message = message.Substring(0, index) + item + message.Substring(index + 1);
                }
            }

            DebugMessage(message);
#endif
        }

        public void DebugMessage(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Logging.Logger.BroadcastMessage(this, message);
        }

        #endregion

    }
}
