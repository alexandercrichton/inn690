using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;

namespace Veis.Data
{
    public class WorldStateDataAccess : DataAccess
    {
        public WorldStateDataAccess(DbProviderFactory dataProviderFactory)
            : base(dataProviderFactory)
        {
        }

        protected override DataAccess.DbConfig GetDbConfig()
        {
            try
            {
                //XElement appConfig = XElement.Load("Assets/Veis/Veis/Veis.xml");
                var config = new DataAccess.DbConfig
                {
                    Host = "localhost",
                    Port = "3306",
                    Database = "veis_world_states",
                    User = "root",
                    Pass = ""
                };
                return config;
            }
            catch (Exception e)
            {
                Logging.Logger.BroadcastMessage(this, e.Message);
            }
            return new DbConfig();
        }
    }
}
