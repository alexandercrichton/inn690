using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Veis.Data
{
    public abstract class Repository<T> : IRepository<T>
    {
        protected IDataAccess DataAccess;

        protected Repository(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        private DbCommand CreateSelectCommand(string query, params Specification<T>[] specifications)
        {
            var joinString = query.Contains("WHERE") ? "AND" : "WHERE";
            foreach (var spec in specifications)
            {
                string condition = spec.Condition;
                if (!string.IsNullOrEmpty(condition))
                {
                    query = string.Format("{0} {1} {2}", query, joinString, condition);
                    joinString = "AND";
                }
            }

            var cmd = DataAccess.CreateCommand(query);

            foreach (var spec in specifications)
            {
                if (spec.Parameters != null)
                {
                    foreach (var key in spec.Parameters.Keys)
                    {
                        var param = DataAccess.CreateParameter(key);
                        param.Value = spec.Parameters[key];
                        cmd.Parameters.Add(param);
                    }
                }
            }

            return cmd;
        }

        /// <summary>
        /// Query the database
        /// </summary>
        protected IEnumerable<T> Select(string query, Func<IEnumerable<T>, IEnumerable<T>> orderAndPage,
                                                Func<IDataReader, T> converter, params Specification<T>[] specifications)
        {
            var cmd = CreateSelectCommand(query, specifications);
            var results = DataAccess.Read(cmd, converter);

            return orderAndPage(results);
        }

        protected int Delete(string query, params Specification<T>[] specifications)
        {
            return Update(query, null, specifications);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        protected int Update(string query, IDictionary<string, object> parameters, params Specification<T>[] specifications)
        {
            // Set up query string with specifications
            var joinString = "WHERE";
            if (query.Contains(joinString))
                joinString = "AND";

            foreach (var spec in specifications)
            {
                if (!string.IsNullOrEmpty(spec.Condition))
                {
                    query = string.Format("{0} {1} {2}", query, joinString, spec.Condition);
                    joinString = "AND";
                }
            }

            var cmd = DataAccess.CreateCommand(query);

            // Add all the update parameters
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    var param = DataAccess.CreateParameter(key);
                    param.Value = parameters[key];
                    cmd.Parameters.Add(param);
                }
            }

            // Add the specification parameters
            foreach (var spec in specifications)
            {
                if (spec.Parameters != null)
                {
                    foreach (var key in spec.Parameters.Keys)
                    {
                        var param = DataAccess.CreateParameter(key);
                        param.Value = spec.Parameters[key];
                        cmd.Parameters.Add(param);
                    }
                }
            }

            // Execute the update
            return DataAccess.Execute(cmd);
        }

        /// <summary>
        /// Insert into the database
        /// </summary>
        protected int Insert(string query, IEnumerable<T> items, IDictionary<string, object> parameters)
        {
            var cmd = DataAccess.CreateCommand(query);

            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    var param = DataAccess.CreateParameter(key);
                    param.Value = parameters[key];
                    cmd.Parameters.Add(param);
                }
            }

            return DataAccess.Execute(cmd);
        }

        protected void ExecuteProcedure(string procedure)
        {
            var command = DataAccess.CreateCommand("call " + procedure);
            DataAccess.Execute(command);
        }

        public abstract IEnumerable<T> Find(params Specification<T>[] specifications);
        public abstract int Insert(T item);
        public abstract int Insert(IEnumerable<T> items);
        public abstract int Delete(T item);
        public abstract int Delete(IEnumerable<T> items);
        public abstract int Update(T oldItem, T newItem, params Specification<T>[] specifications);
        public abstract int Update(IDictionary<T, T> updateMap, params Specification<T>[] specifications);
    }
}
