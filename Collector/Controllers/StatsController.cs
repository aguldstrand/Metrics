using Collector.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace Collector.Controllers
{
    public class StatsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Sessions()
        {
            var outp = EachEntry()
                .Select(le => le.Session)
                .Distinct()
                .ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, outp, "application/json");
        }

        [HttpGet]
        public HttpResponseMessage LogActions(string session = null)
        {
            var outp = EachEntry()
               .Where(le => session == null || le.Session == session)
               .ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, outp, "application/json");
        }

        [HttpGet]
        public HttpResponseMessage NavigationPairs(string session = null)
        {
            var outp = EachEntry()
               .Where(le => session == null || le.Session == session)
               .Where(le => le.Data != null && le.Data["referer"] != null && le.Data["url"] != null)
               .GroupBy(le => new { Referer = le.Data["referer"], Url = le.Data["url"] })
               .Select(leg =>
                   new
                   {
                       From = leg.Key.Referer,
                       To = leg.Key.Url,
                       Count = leg.Count(),
                   })
               .ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, outp, "application/json");
        }

        private IEnumerable<LogEntry> EachEntry()
        {
            var outp = Directory.EnumerateFiles(HttpContext.Current.Server.MapPath("~/App_Data"), "*.txt", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => f)
                .SelectMany(f => System.IO.File.ReadLines(f)
                    .Select(LogEntry.FromJson))
                .OrderBy(le => le.TimeStamp);

            return outp;
        }
    }
}
