using Collector.Hubs;
using Collector.Util;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            var js = System.IO.File.ReadAllText(Server.MapPath("~/js/CollectorClient.min.js"));
            js = js.Replace("{session_key}", session);

            return Content(js, "application/javascript");
        }

        [OutputCache(Duration = 0)]
        [CorsEnabledAttribute]
        [HttpPost]
        public ActionResult Collect(string session)
        {
            Log(session, Request.Form.AllKeys.ToDictionary(
                    key => key,
                    key => Request.Form[key]));

            return Content(string.Empty, "text/plain");
        }

        void Log(string session, Dictionary<string, string> data)
        {
            var logEntry = new
            {
                t = DateTime.UtcNow.ToBinary().ToString("X"),
                s= session,
                d= data,
            };

            var jsonEntry = JsonConvert.SerializeObject(logEntry, Formatting.None);
            log.Info(jsonEntry);

            LogProcessor.instance.Process(session, data);
        }
    }
}