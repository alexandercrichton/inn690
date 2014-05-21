using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Veis.Data
{
    public interface IDataAccess
    {
        IList<T> Read<T>(DbCommand command, Func<IDataReader, T> transformRowToEntity);
        int Execute(DbCommand command); 

        DbParameter CreateParameter(string name);
        DbParameter CreateParameter(string name, object value);

        DbCommand CreateCommand(string sql);

        void DebugMessage(string format, params object[] args);
    }
}
