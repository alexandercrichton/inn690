using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark;
using Spark.FileSystem;
using System.IO;
//using System.Web.Mvc;

namespace Veis.WebInterface.Infrastructure
{
    public class DefaultPageBuilder : PageBuilder
    {
        private readonly ISparkViewEngine _engine;

        public DefaultPageBuilder()
        {
            SparkSettings settings = new SparkSettings();
            //settings.SetStatementMarker("#*");
            //settings.StatementMarker = "#*";
            settings.SetPrefix("s");
            settings.SetPageBaseType(typeof(TemplateBase));
            _engine = new SparkViewEngine(settings);
            //var sm = _engine.Settings.StatementMarker;
            
            var templateDirPath = Path.GetFullPath("./Veis/Views/");
            var viewFolder = new FileSystemViewFolder(templateDirPath);

            _engine.ViewFolder = viewFolder.Append(new SubViewFolder(viewFolder, "Shared"));

               //var viewFolder = new FileSystemViewFolder(templateDirPath);

            //// Create an engine using the templates path as the root location
            //// as well as the shared location
            //var engine = new SparkViewEngine
            //{
            //    DefaultPageBaseType = typeof(SparkView).FullName,
            //    ViewFolder = viewFolder.Append(new SubViewFolder(viewFolder, "Shared"))
            //};
        }

        public override void Transform(string templateName, object data, TextWriter output)
        {
            // The view template
            var descriptor = new SparkViewDescriptor().AddTemplate(templateName + ".spark");

            // The view that will contain view data
            //var view = (TemplateBase)_engine.CreateInstance(descriptor);
            //try
            //{
            //    view.ViewData = new ViewDataDictionary(data);
            //    view.RenderView(output);
            //}
            //finally
            //{
            //    _engine.ReleaseInstance(view);
            //}
        }
    }
}
