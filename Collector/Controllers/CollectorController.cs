using Collector.Hubs;
using Collector.Models;
using Collector.Util;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Collector.Controllers
{
    public class CollectorController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CollectorController));

        [OutputCache(Duration = 0)]
        [CorsEnabledAttribute]
        public ActionResult Init()
        {
            var session = Guid.NewGuid().ToString("N");

            var js = System.IO.File.ReadAllText(Server.MapPath("~/js/CollectorClient.js"));
            js = js.Replace("{session_key}", session);

            return Content(js, "application/javascript");
        }

        [OutputCache(Duration = 0)]
        [CorsEnabledAttribute]
        [HttpPost]
        public ActionResult Collect(string session)
        {
            JToken data;
            try
            {
                data = JToken.Parse(new StreamReader(Request.InputStream).ReadToEnd());
            }
            catch
            {
                data = null;
            }

            Log(session, data);

            return Content(string.Empty, "text/plain");
        }

        void Log(string session, JToken data)
        {
            var logEntry = new LogEntry(
                timeStamp: DateTime.UtcNow,
                session: session,
                data: data);

            var jsonEntry = JsonConvert.SerializeObject(logEntry, Formatting.None, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }  );
            log.Info(jsonEntry);
        }
    }
}