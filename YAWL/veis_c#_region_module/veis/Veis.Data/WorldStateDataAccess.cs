using System;
using System.Collections.Generic;
using System.Linq;
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
            var appConfig = ConfigurationManager.OpenExeConfiguration("Veis.dll");
            var config = new DataAccess.DbConfig
            {
                Host = appConfig.AppSettings.Settings["StateHost"].Value,
                Port = appConfig.AppSettings.Settings["StatePort"].Value,
                Database = appConfig.AppSettings.Settings["StateDatabase"].Value,
                User = appConfig.AppSettings.Settings["StateUser"].Value,
                Pass = appConfig.AppSettings.Settings["StatePass"].Value
            };
            //var config =  new DataAccess.DbConfig
            //{
            //    Host = ConfigurationManager.AppSettings["StateHost"],
            //    Port = ConfigurationManager.AppSettings["StatePort"],
            //    Database = ConfigurationManager.AppSettings["StateDatabase"],
            //    User = ConfigurationManager.AppSettings["StateUser"],
            //    Pass = ConfigurationManager.AppSettings["StatePass"]
            //};
            return config;
        }
    }
}
