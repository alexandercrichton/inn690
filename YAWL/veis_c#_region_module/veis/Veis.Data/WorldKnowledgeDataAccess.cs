using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Xml.Linq;

namespace Veis.Data
{
    public class WorldKnowledgeDataAccess : DataAccess
    {
        public WorldKnowledgeDataAccess(DbProviderFactory dataProviderFactory)
            : base(dataProviderFactory)
        {
        }
        
        protected override DataAccess.DbConfig GetDbConfig()
        {
            //var appConfig = ConfigurationManager.OpenExeConfiguration("Veis.dll");
            //return new DataAccess.DbConfig
            //{
            //    Host = appConfig.AppSettings.Settings["KnowledgeHost"].Value,
            //    Port = appConfig.AppSettings.Settings["KnowledgePort"].Value,
            //    Database = appConfig.AppSettings.Settings["KnowledgeDatabase"].Value,
            //    User = appConfig.AppSettings.Settings["KnowledgeUser"].Value,
            //    Pass = appConfig.AppSettings.Settings["KnowledgePass"].Value
            //}; 
            try
            {
                XElement appConfig = XElement.Load("Assets/Veis/Veis/Veis.xml");
                var config = new DataAccess.DbConfig
                {
                    Host = appConfig.Element("KnowledgeHost").Value,
                    Port = appConfig.Element("KnowledgePort").Value,
                    Database = appConfig.Element("KnowledgeDatabase").Value,
                    User = appConfig.Element("KnowledgeUser").Value,
                    Pass = appConfig.Element("KnowledgePass").Value
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
