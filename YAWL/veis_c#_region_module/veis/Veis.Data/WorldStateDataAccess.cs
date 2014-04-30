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
            Logging.Logger.BroadcastMessage(this, "GetDbConfig()");
            try
            {
                //ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "Veis.dll" };
                //Configuration appConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                //var appConfig = ConfigurationManager.OpenExeConfiguration("Veis.dll");
                //Logging.Logger.BroadcastMessage(this, "GetDbConfig() appConfig");
                //Logging.Logger.BroadcastMessage(this,
                //    "Host = " + appConfig.AppSettings.Settings["StateHost"].Value);
                //Logging.Logger.BroadcastMessage(this,
                //    "Port = " + appConfig.AppSettings.Settings["StatePort"].Value);
                //Logging.Logger.BroadcastMessage(this,
                //    "Database = " + appConfig.AppSettings.Settings["StateDatabase"].Value);
                //Logging.Logger.BroadcastMessage(this,
                //    "User = " + appConfig.AppSettings.Settings["StateUser"].Value);
                //Logging.Logger.BroadcastMessage(this,
                //    "Pass = " + appConfig.AppSettings.Settings["StatePass"].Value);


                XElement appConfig = XElement.Load("Assets/Veis/Veis/Veis.xml");
                Logging.Logger.BroadcastMessage(this, appConfig.Element("StateHost").Value);

                var config = new DataAccess.DbConfig
                {
                    //Host = appConfig.AppSettings.Settings["StateHost"].Value,
                    //Port = appConfig.AppSettings.Settings["StatePort"].Value,
                    //Database = appConfig.AppSettings.Settings["StateDatabase"].Value,
                    //User = appConfig.AppSettings.Settings["StateUser"].Value,
                    //Pass = appConfig.AppSettings.Settings["StatePass"].Value
                    Host = appConfig.Element("StateHost").Value,
                    Port = appConfig.Element("StatePort").Value,
                    Database = appConfig.Element("StateDatabase").Value,
                    User = appConfig.Element("StateUser").Value,
                    Pass = appConfig.Element("StatePass").Value
                };
                Logging.Logger.BroadcastMessage(this, "GetDbConfig() return config");
                return config;
            }
            catch (Exception e)
            {
                Logging.Logger.BroadcastMessage(this, e.Message);
            }
            
            //var config =  new DataAccess.DbConfig
            //{
            //    Host = ConfigurationManager.AppSettings["StateHost"],
            //    Port = ConfigurationManager.AppSettings["StatePort"],
            //    Database = ConfigurationManager.AppSettings["StateDatabase"],
            //    User = ConfigurationManager.AppSettings["StateUser"],
            //    Pass = ConfigurationManager.AppSettings["StatePass"]
            //};
            return new DbConfig();
        }
    }
}
